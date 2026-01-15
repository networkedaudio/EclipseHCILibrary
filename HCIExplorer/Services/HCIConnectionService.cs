using CommunityToolkit.Mvvm.ComponentModel;
using HCILibrary;
using HCILibrary.Models;

namespace HCIExplorer.Services;

public partial class HCIConnectionService : ObservableObject
{
    private static HCIConnectionService? _instance;
    public static HCIConnectionService Instance => _instance ??= new HCIConnectionService();
    
    private HCIConnection? _connection;
    private CancellationTokenSource? _connectionCts;
    
    [ObservableProperty]
    private bool _isConnected;
    
    [ObservableProperty]
    private string _hostAddress = MatrixDiscoveryService.DefaultHostAddress;
    
    [ObservableProperty]
    private string _connectionStatus = "Disconnected";
    
    [ObservableProperty]
    private int _connectedPort;
    
    private HCIConnectionService() { }
    
    public event EventHandler<HCIReply>? ReplyReceived;
    
    public async Task<bool> ConnectAsync(string host)
    {
        if (IsConnected && _connection != null)
        {
            await DisconnectAsync();
        }
        
        try
        {
            HostAddress = host;
            ConnectionStatus = "Connecting...";
            LogService.Instance.LogDebug($"Attempting to connect to {host}");
            
            _connection = new HCIConnection(host);
            _connectionCts = new CancellationTokenSource();
            
            // Subscribe to events
            _connection.MessageReceived += OnMessageReceived;
            _connection.ConnectionStateChanged += OnConnectionStateChanged;
            _connection.ErrorOccurred += OnErrorOccurred;
            
            bool success = await _connection.ConnectAsync(_connectionCts.Token);
            
            if (success)
            {
                IsConnected = true;
                ConnectedPort = _connection.CurrentPort;
                ConnectionStatus = $"Connected to {host}:{ConnectedPort}";
                LogService.Instance.LogDebug($"Connected successfully on port {ConnectedPort}");
            }
            else
            {
                ConnectionStatus = "Connection failed";
                LogService.Instance.LogError("Failed to connect to matrix");
            }
            
            return success;
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Error: {ex.Message}";
            LogService.Instance.LogError($"Connection error: {ex.Message}");
            return false;
        }
    }
    
    public async Task DisconnectAsync()
    {
        if (_connection != null)
        {
            _connectionCts?.Cancel();
            
            _connection.MessageReceived -= OnMessageReceived;
            _connection.ConnectionStateChanged -= OnConnectionStateChanged;
            _connection.ErrorOccurred -= OnErrorOccurred;
            
            await _connection.DisconnectAsync();
            _connection.Dispose();
            _connection = null;
        }
        
        IsConnected = false;
        ConnectionStatus = "Disconnected";
        ConnectedPort = 0;
        LogService.Instance.LogDebug("Disconnected from matrix");
    }
    
    public Task<bool> SendRequestAsync(HCIRequest request)
    {
        if (_connection == null || !IsConnected)
        {
            LogService.Instance.LogError("Cannot send request: Not connected");
            return Task.FromResult(false);
        }
        
        try
        {
            byte[] data = request.BuildMessage();
            string hexData = BitConverter.ToString(data).Replace("-", " ");
            LogService.Instance.LogRequest($"Sending {request.GetType().Name}", hexData);
            
            if (_connection.RequestQueue != null)
            {
                _connection.RequestQueue.Enqueue(request);
            }
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            LogService.Instance.LogError($"Error sending request: {ex.Message}");
            return Task.FromResult(false);
        }
    }
    
    private void OnMessageReceived(object? sender, HCIReply reply)
    {
        string hexData = reply.RawMessage != null ? BitConverter.ToString(reply.RawMessage).Replace("-", " ") : "";
        LogService.Instance.LogResponse($"Received {reply.MessageID} (Flags: {reply.Flags})", hexData);
        
        ReplyReceived?.Invoke(this, reply);
    }
    
    private void OnConnectionStateChanged(object? sender, bool connected)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            IsConnected = connected;
            if (!connected)
            {
                ConnectionStatus = "Disconnected (connection lost)";
                LogService.Instance.LogError("Connection to matrix lost");
            }
        });
    }
    
    private void OnErrorOccurred(object? sender, Exception ex)
    {
        LogService.Instance.LogError($"Connection error: {ex.Message}");
    }
}
