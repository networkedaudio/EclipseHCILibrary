using System.Net;
using System.Net.Sockets;

namespace HCILibrary.Discovery;

/// <summary>
/// Listens for UDP broadcast messages on port 42001 and forwards them for parsing
/// </summary>
public class DiscoveryListener : IDisposable
{
    private const int DISCOVERY_PORT = 42001;
    private UdpClient? _udpClient;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _listenerTask;
    private bool _isListening;

    /// <summary>
    /// Event raised when a broadcast message is received
    /// </summary>
    public event EventHandler<BroadcastReceivedEventArgs>? BroadcastReceived;

    /// <summary>
    /// Gets whether the listener is currently active
    /// </summary>
    public bool IsListening => _isListening;

    /// <summary>
    /// Starts listening for UDP broadcast messages
    /// </summary>
    public void Start()
    {
        if (_isListening)
        {
            throw new InvalidOperationException("Listener is already running");
        }

        _cancellationTokenSource = new CancellationTokenSource();
        _udpClient = new UdpClient();
        
        // Set socket options to allow address reuse
        _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, DISCOVERY_PORT));
        
        _isListening = true;

        _listenerTask = Task.Run(() => ListenAsync(_cancellationTokenSource.Token));
    }

    /// <summary>
    /// Stops listening for UDP broadcast messages
    /// </summary>
    public async Task StopAsync()
    {
        if (!_isListening)
        {
            return;
        }

        _isListening = false;
        _cancellationTokenSource?.Cancel();

        if (_listenerTask != null)
        {
            await _listenerTask;
        }

        _udpClient?.Close();
        _udpClient?.Dispose();
        _udpClient = null;
    }

    private async Task ListenAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested && _udpClient != null)
            {
                try
                {
                    var result = await _udpClient.ReceiveAsync(cancellationToken);
                    
                    // Forward to parser
                    var parsedData = BroadcastParser.Parse(result.Buffer, result.RemoteEndPoint);
                    
                    // Raise event
                    BroadcastReceived?.Invoke(this, new BroadcastReceivedEventArgs(
                        result.Buffer,
                        result.RemoteEndPoint,
                        parsedData
                    ));
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (SocketException ex)
                {
                    // Log socket errors but continue listening
                    Console.WriteLine($"Socket error in DiscoveryListener: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Log other errors but continue listening
                    Console.WriteLine($"Error in DiscoveryListener: {ex.Message}");
                }
            }
        }
        finally
        {
            _isListening = false;
        }
    }

    public void Dispose()
    {
        StopAsync().GetAwaiter().GetResult();
        _cancellationTokenSource?.Dispose();
    }
}

/// <summary>
/// Event args for broadcast received events
/// </summary>
public class BroadcastReceivedEventArgs : EventArgs
{
    public byte[] RawData { get; }
    public IPEndPoint RemoteEndPoint { get; }
    public BroadcastData? ParsedData { get; }
    public DateTime Timestamp { get; }

    public BroadcastReceivedEventArgs(byte[] rawData, IPEndPoint remoteEndPoint, BroadcastData? parsedData)
    {
        RawData = rawData;
        RemoteEndPoint = remoteEndPoint;
        ParsedData = parsedData;
        Timestamp = DateTime.Now;
    }
}