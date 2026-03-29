using HCILibrary.Enums;
using HCILibrary.Helpers;
using HCILibrary.Models;
using System.Diagnostics;
using System.Net.Sockets;

namespace HCILibrary;

/// <summary>
/// Manages TCP connection to an HCI device with automatic port failover.
/// </summary>
public class HCIConnection : IDisposable
{
    /// <summary>
    /// Start marker for HCI messages.
    /// </summary>
    private static readonly byte[] StartMarker = { 0x5A, 0x0F };

    /// <summary>
    /// End marker for HCI messages.
    /// </summary>
    private static readonly byte[] EndMarker = { 0x2E, 0x8D };

    private readonly string _ipAddress;
    private readonly int _startPort;
    private readonly int _endPort;
    private readonly int _connectionTimeoutMs;
    private readonly int _readTimeoutMs;
    
    private TcpClient? _client;
    private NetworkStream? _stream;
    private CancellationTokenSource? _readCancellationTokenSource;
    private Task? _readTask;
    private readonly List<byte> _buffer = new();
    private readonly object _bufferLock = new();
    private bool _disposed;
    private int _currentPort;

    private HCIRequestQueue? _requestQueue;
    private readonly Dictionary<HCIMessageID, HCIRequest> _pendingRequests = new();
    private readonly object _pendingRequestsLock = new();

    /// <summary>
    /// Event raised when a complete message is received and decoded.
    /// </summary>
    public event EventHandler<HCIReply>? MessageReceived;

    /// <summary>
    /// Event raised when the connection state changes.
    /// </summary>
    public event EventHandler<bool>? ConnectionStateChanged;

    /// <summary>
    /// Event raised when an error occurs.
    /// </summary>
    public event EventHandler<Exception>? ErrorOccurred;

    /// <summary>
    /// Gets whether the connection is currently active.
    /// </summary>
    public bool IsConnected => _client?.Connected ?? false;

    /// <summary>
    /// Gets the current port number.
    /// </summary>
    public int CurrentPort => _currentPort;

    /// <summary>
    /// Gets the request queue for sending messages.
    /// </summary>
    public HCIRequestQueue? RequestQueue => _requestQueue;

    /// <summary>
    /// Creates a new HCI connection handler.
    /// </summary>
    /// <param name="ipAddress">The IP address to connect to.</param>
    /// <param name="startPort">The starting port number (default: 52020).</param>
    /// <param name="endPort">The ending port number (default: 52001).</param>
    /// <param name="connectionTimeoutMs">Connection timeout in milliseconds (default: 5000).</param>
    /// <param name="readTimeoutMs">Read timeout in milliseconds for detecting inactive connections (default: 30000).</param>
    public HCIConnection(string ipAddress, int startPort = 52020, int endPort = 52001, int connectionTimeoutMs = 5000, int readTimeoutMs = 30000)
    {
        _ipAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
        _startPort = startPort;
        _endPort = endPort;
        _connectionTimeoutMs = connectionTimeoutMs;
        _readTimeoutMs = readTimeoutMs;
        _currentPort = startPort;
    }

    /// <summary>
    /// Attempts to connect to the HCI device, trying each port in sequence on failure.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if connection succeeded, false otherwise.</returns>
    public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
    {
        int port = _startPort;
        int direction = _startPort > _endPort ? -1 : 1;
        int portCount = Math.Abs(_startPort - _endPort) + 1;

        for (int attempt = 0; attempt < portCount; attempt++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                if (IsConnected)
                {
                    await DisconnectAsync();
                }

                _client = new TcpClient();
                _currentPort = port;

                using var timeoutCts = new CancellationTokenSource(_connectionTimeoutMs);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                await _client.ConnectAsync(_ipAddress, port, linkedCts.Token);

                if (_client.Connected)
                {
                    _stream = _client.GetStream();
                    StartReading();
                    InitializeRequestQueue();
                    ConnectionStateChanged?.Invoke(this, true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex);
                await DisconnectAsync();
            }

            // Move to next port
            port += direction;
        }

        return false;
    }

    /// <summary>
    /// Disconnects from the HCI device.
    /// </summary>
    public async Task DisconnectAsync()
    {
        _readCancellationTokenSource?.Cancel();

        if (_readTask != null)
        {
            try
            {
                await _readTask;
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelling
            }
        }

        if (_requestQueue != null)
        {
            await _requestQueue.StopAsync();
            _requestQueue.Dispose();
            _requestQueue = null;
        }

        _stream?.Dispose();
        _stream = null;

        _client?.Dispose();
        _client = null;

        lock (_bufferLock)
        {
            _buffer.Clear();
        }

        ConnectionStateChanged?.Invoke(this, false);
    }

    /// <summary>
    /// Initializes the request queue for sending messages.
    /// </summary>
    /// <param name="messagesPerSecond">Maximum messages per second.</param>
    private void InitializeRequestQueue(int messagesPerSecond = 10)
    {
        _requestQueue = new HCIRequestQueue(SendRequestAsync, messagesPerSecond);
        _requestQueue.Start();
    }

    /// <summary>
    /// Sends a request through the TCP connection.
    /// </summary>
    private async Task SendRequestAsync(HCIRequest request)
    {
        if (_stream == null || !IsConnected)
        {
            throw new InvalidOperationException("Not connected.");
        }

        var message = request.BuildMessage();

        DebugHelper.WriteBytes($"TX [{request.MessageID}]", message);
        Console.WriteLine($"TX [{request.MessageID}] ({message.Length} bytes): {BitConverter.ToString(message)}");

        await _stream.WriteAsync(message);

        // Track pending request if it expects a response
        if (request.ExpectedReplyMessageID.HasValue)
        {
            lock (_pendingRequestsLock)
            {
                _pendingRequests[request.ExpectedReplyMessageID.Value] = request;
            }
        }
    }

    /// <summary>
    /// Starts the background task for reading data from the TCP stream.
    /// </summary>
    private void StartReading()
    {
        _readCancellationTokenSource = new CancellationTokenSource();
        _readTask = Task.Run(() => ReadLoopAsync(_readCancellationTokenSource.Token));
    }

    /// <summary>
    /// Continuously reads data from the TCP stream and processes complete messages.
    /// </summary>
    private async Task ReadLoopAsync(CancellationToken cancellationToken)
    {
        var readBuffer = new byte[4096];

        while (!cancellationToken.IsCancellationRequested && _stream != null)
        {
            try
            {
                using var readTimeoutCts = new CancellationTokenSource(_readTimeoutMs);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, readTimeoutCts.Token);

                int bytesRead = await _stream.ReadAsync(readBuffer, linkedCts.Token);
                
                if (bytesRead == 0)
                {
                    // Connection closed gracefully by remote host
                    break;
                }

                lock (_bufferLock)
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        _buffer.Add(readBuffer[i]);
                    }
                }

                ProcessBuffer();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Expected when manually disconnecting
                break;
            }
            catch (OperationCanceledException)
            {
                // Read timeout - no data received within the timeout period
                ErrorOccurred?.Invoke(this, new TimeoutException($"No data received within {_readTimeoutMs}ms. Connection appears inactive."));
                break;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex);
                break;
            }
        }

        // Handle unexpected disconnection
        if (!cancellationToken.IsCancellationRequested)
        {
            await DisconnectAsync();
        }
    }

    /// <summary>
    /// Processes the buffer to extract complete messages.
    /// Uses the length field to determine message boundaries rather than scanning
    /// for end markers, since end marker bytes (0x2E 0x8D) can appear in payload data.
    /// </summary>
    private void ProcessBuffer()
    {
        lock (_bufferLock)
        {
            while (true)
            {
                // Find start marker
                int startIndex = FindSequence(_buffer, StartMarker);
                if (startIndex == -1)
                {
                    // No start marker found, clear buffer up to last byte
                    // (in case partial start marker is at end)
                    if (_buffer.Count > 1)
                    {
                        _buffer.RemoveRange(0, _buffer.Count - 1);
                    }
                    break;
                }

                // Remove any bytes before start marker
                if (startIndex > 0)
                {
                    _buffer.RemoveRange(0, startIndex);
                }

                // Need at least 4 bytes to read start marker + length field
                if (_buffer.Count < 4)
                {
                    break;
                }

                // Read length field (2 bytes big-endian, after start marker)
                // Total message length = start(2) + length field value
                ushort expectedLength = (ushort)((_buffer[2] << 8) | _buffer[3]);

                // Sanity check: length must be at least enough for header + end marker
                if (expectedLength < 9) // start(2) + length(2) + msgId(2) + flags(1) + end(2)
                {
                    // Invalid length — skip this start marker and look for the next one
                    _buffer.RemoveRange(0, StartMarker.Length);
                    continue;
                }

                // Wait until we have the complete message in the buffer
                if (_buffer.Count < expectedLength)
                {
                    break;
                }

                // Extract the complete message using the length field
                var message = _buffer.Take(expectedLength).ToArray();
                _buffer.RemoveRange(0, expectedLength);

                // Validate end marker is at the expected position
                int endMarkerPos = expectedLength - EndMarker.Length;
                if (message[endMarkerPos] == EndMarker[0] && message[endMarkerPos + 1] == EndMarker[1])
                {
                    // Log RX message summary
                    ushort msgId = (ushort)((message[4] << 8) | message[5]);
                    byte flags = message[6];
                    var rxLog = $"RX [0x{msgId:X4}] ({expectedLength} bytes) Flags: E={((flags & 0x01) != 0)}, M={((flags & 0x02) != 0)}, U={((flags & 0x04) != 0)}, G={((flags & 0x08) != 0)}, S={((flags & 0x10) != 0)}, N={((flags & 0x20) != 0)}";
                    Console.WriteLine(rxLog);
                    Debug.WriteLine(rxLog);

                    // Hand off to HCIResponse for decoding
                    var reply = HCIResponse.Decode(message);
                    if (reply != null)
                    {
                        HandleReply(reply);
                    }
                }
                else
                {
                    ushort msgId = (ushort)((message[4] << 8) | message[5]);
                    var dropLog = $"RX [0x{msgId:X4}] ({expectedLength} bytes) DROPPED — end marker mismatch at offset {endMarkerPos}: got 0x{message[endMarkerPos]:X2} 0x{message[endMarkerPos + 1]:X2}, expected 0x{EndMarker[0]:X2} 0x{EndMarker[1]:X2}";
                    Console.WriteLine(dropLog);
                    Debug.WriteLine(dropLog);
                    Debug.WriteLine($"  Full message: {BitConverter.ToString(message)}");
                }
            }
        }
    }

    /// <summary>
    /// Handles a decoded reply, matching it to pending requests if applicable.
    /// </summary>
    private void HandleReply(HCIReply reply)
    {
        // Check if this reply matches a pending request
        HCIRequest? matchingRequest = null;
        lock (_pendingRequestsLock)
        {
            if (_pendingRequests.TryGetValue(reply.MessageID, out matchingRequest))
            {
                _pendingRequests.Remove(reply.MessageID);
            }
        }

        // Complete the pending request if found
        matchingRequest?.ResponseCompletionSource?.TrySetResult(reply);

        // Raise event for all received messages
        MessageReceived?.Invoke(this, reply);
    }

    /// <summary>
    /// Finds a byte sequence within a list of bytes.
    /// </summary>
    /// <param name="buffer">The buffer to search in.</param>
    /// <param name="sequence">The sequence to find.</param>
    /// <param name="startFrom">Index to start searching from.</param>
    /// <returns>The index of the sequence, or -1 if not found.</returns>
    private static int FindSequence(List<byte> buffer, byte[] sequence, int startFrom = 0)
    {
        for (int i = startFrom; i <= buffer.Count - sequence.Length; i++)
        {
            bool found = true;
            for (int j = 0; j < sequence.Length; j++)
            {
                if (buffer[i + j] != sequence[j])
                {
                    found = false;
                    break;
                }
            }
            if (found)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Disposes of the connection and its resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        DisconnectAsync().GetAwaiter().GetResult();
        _readCancellationTokenSource?.Dispose();
    }
}
