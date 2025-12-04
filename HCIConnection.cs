using HCILibrary.Enums;
using HCILibrary.Helpers;
using HCILibrary.Models;
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
    public HCIConnection(string ipAddress, int startPort = 52020, int endPort = 52001, int connectionTimeoutMs = 5000)
    {
        _ipAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
        _startPort = startPort;
        _endPort = endPort;
        _connectionTimeoutMs = connectionTimeoutMs;
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
                await DisconnectAsync();

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
                int bytesRead = await _stream.ReadAsync(readBuffer, cancellationToken);
                
                if (bytesRead == 0)
                {
                    // Connection closed
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
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex);
                break;
            }
        }
    }

    /// <summary>
    /// Processes the buffer to extract complete messages.
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

                // Find end marker
                int endIndex = FindSequence(_buffer, EndMarker, StartMarker.Length);
                if (endIndex == -1)
                {
                    // No complete message yet
                    break;
                }

                // Extract complete message
                int messageLength = endIndex + EndMarker.Length;
                var message = _buffer.Take(messageLength).ToArray();
                _buffer.RemoveRange(0, messageLength);

                // Validate length field before decoding
                if (message.Length >= 4)
                {
                    ushort expectedLength = (ushort)((message[2] << 8) | message[3]);
                    
                    // Total message should be: start(2) + length field value
                    if (message.Length == expectedLength + 2)
                    {
                        // Hand off to HCIResponse for decoding
                        var reply = HCIResponse.Decode(message);
                        if (reply != null)
                        {
                            HandleReply(reply);
                        }
                    }
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
