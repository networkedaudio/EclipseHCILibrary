using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using HCILibrary.Discovery;

namespace HCIExplorer.Services;

/// <summary>
/// Service that manages discovery of Eclipse HX matrices on the network
/// </summary>
public partial class MatrixDiscoveryService : ObservableObject, IDisposable
{
    private static MatrixDiscoveryService? _instance;
    public static MatrixDiscoveryService Instance => _instance ??= new MatrixDiscoveryService();

    /// <summary>
    /// Default host address used when no matrix is discovered
    /// </summary>
    public const string DefaultHostAddress = "192.168.1.100";

    private readonly DiscoveryListener _listener;

    [ObservableProperty]
    private ObservableCollection<DiscoveredMatrix> _discoveredMatrices = new();

    [ObservableProperty]
    private bool _isDiscoveryActive;

    private MatrixDiscoveryService()
    {
        _listener = new DiscoveryListener();
        _listener.BroadcastReceived += OnBroadcastReceived;
        _discoveredMatrices.CollectionChanged += OnDiscoveredMatricesChanged;
    }

    /// <summary>
    /// Starts listening for matrix broadcasts
    /// </summary>
    public void StartDiscovery()
    {
        if (IsDiscoveryActive)
            return;

        try
        {
            _listener.Start();
            IsDiscoveryActive = true;
            LogService.Instance.LogDebug("Matrix discovery started on port 42001");
        }
        catch (Exception ex)
        {
            LogService.Instance.LogError($"Failed to start discovery: {ex.Message}");
        }
    }

    /// <summary>
    /// Stops listening for matrix broadcasts
    /// </summary>
    public async Task StopDiscoveryAsync()
    {
        if (!IsDiscoveryActive)
            return;

        try
        {
            await _listener.StopAsync();
            IsDiscoveryActive = false;
            LogService.Instance.LogDebug("Matrix discovery stopped");
        }
        catch (Exception ex)
        {
            LogService.Instance.LogError($"Error stopping discovery: {ex.Message}");
        }
    }

    private void OnBroadcastReceived(object? sender, BroadcastReceivedEventArgs e)
    {
        if (e.ParsedData == null)
            return;

        var signature = e.ParsedData.MatrixSignature;

        // Update on UI thread
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            var existingMatrix = DiscoveredMatrices.FirstOrDefault(m =>
                m.IpAddress == e.ParsedData.SourceAddress);

            if (existingMatrix != null)
            {
                // Check if signature has changed
                bool signatureChanged = existingMatrix.HasSignatureChanged(signature);
                
                if (signatureChanged && signature != null)
                {
                    LogService.Instance.LogDebug($"Matrix signature changed for {e.ParsedData.SourceAddress}:");
                    LogSignatureDetails(signature);
                    existingMatrix.UpdateSignature(signature);
                }
                
                // Update existing entry
                existingMatrix.LastSeen = DateTime.Now;
                existingMatrix.BroadcastCount++;
            }
            else
            {
                // Add new matrix
                var newMatrix = new DiscoveredMatrix
                {
                    IpAddress = e.ParsedData.SourceAddress,
                    Port = e.ParsedData.SourcePort,
                    FirstSeen = DateTime.Now,
                    LastSeen = DateTime.Now,
                    BroadcastCount = 1,
                    RawData = e.ParsedData.ParsedContent ?? e.ParsedData.GetHexString()
                };

                if (signature != null)
                {
                    newMatrix.UpdateSignature(signature);
                }

                DiscoveredMatrices.Add(newMatrix);
                
                // Log new matrix discovery with signature
                LogService.Instance.LogDebug($"Discovered new matrix at {newMatrix.IpAddress}");
                if (signature != null)
                {
                    LogSignatureDetails(signature);
                }
            }

            // Remove stale entries (not seen in 60 seconds)
            RemoveStaleMatrices();
        });
    }

    private void LogSignatureDetails(HCILibrary.Discovery.MatrixSignature signature)
    {
        LogService.Instance.LogDebug($"  Frame Name: {signature.FrameName}");
        LogService.Instance.LogDebug($"  Configuration Name: {signature.Identity}");
        LogService.Instance.LogDebug($"  Type: {signature.MatrixType}");
        LogService.Instance.LogDebug($"  Version: {signature.MapVersion}");
        LogService.Instance.LogDebug($"  System #: {signature.SystemNumber}");
        if (signature.Created != default)
        {
            LogService.Instance.LogDebug($"  Created: {signature.Created:yyyy-MM-dd HH:mm:ss}");
        }
    }

    private void RemoveStaleMatrices()
    {
        var staleThreshold = DateTime.Now.AddSeconds(-60);
        var staleMatrices = DiscoveredMatrices
            .Where(m => m.LastSeen < staleThreshold)
            .ToList();

        foreach (var matrix in staleMatrices)
        {
            DiscoveredMatrices.Remove(matrix);
            LogService.Instance.LogDebug($"Removed stale matrix: {matrix.IpAddress}");
        }
    }

    public void Dispose()
    {
        StopDiscoveryAsync().GetAwaiter().GetResult();
        _listener.Dispose();
    }

    // Add this method implementation to match NotifyCollectionChangedEventHandler signature
    private void OnDiscoveredMatricesChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnDiscoveredMatricesChanged(e);
    }

    partial void OnDiscoveredMatricesChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e);
}

/// <summary>
/// Represents a discovered Eclipse HX matrix
/// </summary>
public partial class DiscoveredMatrix : ObservableObject
{
    [ObservableProperty]
    private string _ipAddress = string.Empty;

    [ObservableProperty]
    private int _port;

    [ObservableProperty]
    private DateTime _firstSeen;

    [ObservableProperty]
    private DateTime _lastSeen;

    [ObservableProperty]
    private int _broadcastCount;

    [ObservableProperty]
    private string _rawData = string.Empty;

    // Signature tracking properties
    private string _storedFrameName = string.Empty;
    private string _storedIdentity = string.Empty;
    private string _storedMapVersion = string.Empty;
    private HCILibrary.Discovery.MatrixType _storedMatrixType;
    private int _storedSystemNumber;

    public string DisplayName => $"{IpAddress} (Last seen: {GetTimeSinceLastSeen()})";

    private string GetTimeSinceLastSeen()
    {
        var timeSpan = DateTime.Now - LastSeen;
        if (timeSpan.TotalSeconds < 60)
            return $"{(int)timeSpan.TotalSeconds}s ago";
        return $"{(int)timeSpan.TotalMinutes}m ago";
    }

    /// <summary>
    /// Checks if the signature has changed compared to stored values
    /// </summary>
    public bool HasSignatureChanged(HCILibrary.Discovery.MatrixSignature? signature)
    {
        if (signature == null)
            return false;

        return _storedFrameName != signature.FrameName ||
               _storedIdentity != signature.Identity ||
               _storedMapVersion != signature.MapVersion ||
               _storedMatrixType != signature.MatrixType ||
               _storedSystemNumber != signature.SystemNumber;
    }

    /// <summary>
    /// Updates the stored signature values
    /// </summary>
    public void UpdateSignature(HCILibrary.Discovery.MatrixSignature signature)
    {
        _storedFrameName = signature.FrameName;
        _storedIdentity = signature.Identity;
        _storedMapVersion = signature.MapVersion;
        _storedMatrixType = signature.MatrixType;
        _storedSystemNumber = signature.SystemNumber;
    }

    public override string ToString() => IpAddress;
}