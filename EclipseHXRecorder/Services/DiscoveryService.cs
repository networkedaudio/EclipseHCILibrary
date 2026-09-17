using HCILibrary.Discovery;
using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;

namespace EclipseHXRecorder.Services;

/// <summary>
/// Represents a matrix discovered via multicast/broadcast, or manually added by IP.
/// </summary>
public class DiscoveredMatrix
{
    public string IpAddress { get; set; } = string.Empty;
    public string FrameName { get; set; } = string.Empty;
    public string Identity { get; set; } = string.Empty;
    public MatrixType MatrixType { get; set; }
    public string MapVersion { get; set; } = string.Empty;
    public int SystemNumber { get; set; }
    public DateTime LastSeen { get; set; }
    public bool ManuallyAdded { get; set; }
}

/// <summary>
/// Hosted singleton service that listens for EclipseHX matrix discovery broadcasts
/// and maintains a list of discovered matrices, and allows manual addition by IP.
/// </summary>
public class DiscoveryService : IHostedService, IDisposable
{
    private readonly DiscoveryListener _listener = new();
    private readonly ConcurrentDictionary<string, DiscoveredMatrix> _matrices = new();

    /// <summary>
    /// Event raised when a matrix is discovered or updated.
    /// </summary>
    public event EventHandler<DiscoveredMatrix>? MatrixDiscovered;

    /// <summary>
    /// Gets the currently known matrices (discovered or manually added).
    /// </summary>
    public IReadOnlyCollection<DiscoveredMatrix> Matrices => _matrices.Values.ToList();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _listener.BroadcastReceived += OnBroadcastReceived;

        try
        {
            _listener.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DiscoveryService] Failed to start discovery listener: {ex.Message}");
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _listener.BroadcastReceived -= OnBroadcastReceived;
        await _listener.StopAsync();
    }

    private void OnBroadcastReceived(object? sender, BroadcastReceivedEventArgs e)
    {
        var signature = e.ParsedData?.MatrixSignature;
        if (signature?.PrimaryAddress == null)
        {
            return;
        }

        var ip = signature.PrimaryAddress.ToString();

        var matrix = new DiscoveredMatrix
        {
            IpAddress = ip,
            FrameName = signature.FrameName,
            Identity = signature.Identity,
            MatrixType = signature.MatrixType,
            MapVersion = signature.MapVersion,
            SystemNumber = signature.SystemNumber,
            LastSeen = DateTime.Now,
            ManuallyAdded = false
        };

        _matrices.AddOrUpdate(ip, matrix, (_, existing) =>
        {
            matrix.ManuallyAdded = existing.ManuallyAdded;
            return matrix;
        });

        MatrixDiscovered?.Invoke(this, matrix);
    }

    /// <summary>
    /// Adds a matrix manually by IP address, if not already known.
    /// </summary>
    public DiscoveredMatrix AddManualMatrix(string ipAddress)
    {
        var matrix = _matrices.GetOrAdd(ipAddress, ip => new DiscoveredMatrix
        {
            IpAddress = ip,
            FrameName = ip,
            LastSeen = DateTime.Now,
            ManuallyAdded = true
        });

        MatrixDiscovered?.Invoke(this, matrix);
        return matrix;
    }

    public void Dispose()
    {
        _listener.Dispose();
    }
}
