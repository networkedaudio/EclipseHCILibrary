using EclipseHXSNMP.Models;
using HCILibrary;
using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using HCILibrary.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EclipseHXSNMP.Services;

/// <summary>
/// Background service that connects to configured matrices via HCI (TCP),
/// polls for card/port/PSU status, and feeds the data into the SNMP object store.
/// </summary>
public class HciPollingService : BackgroundService
{
    private readonly ConfigurationService _configService;
    private readonly EclipseHxMatrixStatus _matrixStatus;
    private readonly EclipseHxSnmpAgent _snmpAgent;
    private readonly ILogger<HciPollingService> _logger;
    private readonly Dictionary<string, HCIConnection> _connections = new();
    private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(30);

    public HciPollingService(
        ConfigurationService configService,
        EclipseHxMatrixStatus matrixStatus,
        EclipseHxSnmpAgent snmpAgent,
        ILogger<HciPollingService> logger)
    {
        _configService = configService;
        _matrixStatus = matrixStatus;
        _snmpAgent = snmpAgent;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HCI Polling Service starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PollAllMatricesAsync(stoppingToken);
                _snmpAgent.RefreshStore();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during HCI polling cycle");
            }

            await Task.Delay(_pollInterval, stoppingToken);
        }
    }

    private async Task PollAllMatricesAsync(CancellationToken ct)
    {
        var matrices = _configService.Configuration.Matrices
            .Where(m => m.Enabled)
            .ToList();

        foreach (var matrix in matrices)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                var connection = await GetOrCreateConnectionAsync(matrix, ct);
                if (connection == null || !connection.IsConnected)
                {
                    _logger.LogWarning("Cannot connect to {Name} ({Ip})", matrix.Name, matrix.IpAddress);
                    continue;
                }

                await PollMatrixAsync(connection, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error polling {Name} ({Ip})", matrix.Name, matrix.IpAddress);
            }
        }
    }

    private async Task<HCIConnection?> GetOrCreateConnectionAsync(MatrixConnection matrix, CancellationToken ct)
    {
        var key = matrix.IpAddress;

        if (_connections.TryGetValue(key, out var existing) && existing.IsConnected)
        {
            return existing;
        }

        // Clean up stale connection
        if (existing != null)
        {
            existing.Dispose();
            _connections.Remove(key);
        }

        var connection = new HCIConnection(matrix.IpAddress);
        connection.MessageReceived += OnMessageReceived;

        var connected = await connection.ConnectAsync(ct);
        if (connected)
        {
            _connections[key] = connection;
            _logger.LogInformation("Connected to {Name} ({Ip}) on port {Port}",
                matrix.Name, matrix.IpAddress, connection.CurrentPort);
            return connection;
        }

        connection.Dispose();
        return null;
    }

    private async Task PollMatrixAsync(HCIConnection connection, CancellationToken ct)
    {
        if (connection.RequestQueue == null) return;

        // Request System Card Status
        var sysRequest = new RequestSystemStatusRequest();
        connection.RequestQueue.Enqueue(sysRequest);

        // Request Frame Status (PSU data)
        var frameRequest = new RequestFrameStatusRequest();
        connection.RequestQueue.Enqueue(frameRequest);

        // Request Panel Status (port data)
        var panelRequest = new RequestPanelStatusRequest();
        connection.RequestQueue.Enqueue(panelRequest);

        // Allow time for responses to arrive
        await Task.Delay(3000, ct);
    }

    private void OnMessageReceived(object? sender, HCIReply reply)
    {
        try
        {
            if (reply.SystemCardStatus is { } cardStatus)
            {
                var entries = cardStatus.Cards.Select((c, i) => new SnmpCardEntry
                {
                    Index = i + 1,
                    CardType = c.CardType,
                    Condition = c.Condition,
                    IsSlotZero = c.IsSlotZero,
                    RawStatus = c.RawStatus
                }).ToList();

                _matrixStatus.UpdateCards(entries);
                _logger.LogDebug("Updated {Count} cards", entries.Count);
            }

            if (reply.FrameStatus is { } frameStatus)
            {
                _matrixStatus.UpdatePsuStatus(new SnmpPsuStatus
                {
                    CpuTemperature = frameStatus.CpuCardTemperature,
                    ExtPsu1Failed = frameStatus.IsExtPsu1Failed,
                    ExtPsu2Failed = frameStatus.IsExtPsu2Failed,
                    IntPsu1Failed = frameStatus.IsIntPsu1Failed,
                    IntPsu2Failed = frameStatus.IsIntPsu2Failed,
                    Fan1Failed = frameStatus.IsFan1Failed,
                    Fan2Failed = frameStatus.IsFan2Failed,
                    ConfigFailed = frameStatus.IsConfigFailed,
                    ExtAlarmActive = frameStatus.IsExtAlarmActive,
                    Overtemp = frameStatus.IsOvertemp
                });
                _logger.LogDebug("Updated PSU status: CPU={Temp}°C", frameStatus.CpuCardTemperature);
            }

            if (reply.PanelStatus is { } panelStatus)
            {
                var entries = panelStatus.Panels.Select((p, i) => new SnmpPortEntry
                {
                    Index = i + 1,
                    PortNumber = p.PanelNumber,
                    PanelType = p.PanelType,
                    State = p.State,
                    IsAoipDevice = p.IsAoipDevice
                }).ToList();

                _matrixStatus.UpdatePorts(entries);
                _logger.LogDebug("Updated {Count} ports", entries.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing HCI reply");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("HCI Polling Service stopping");

        foreach (var connection in _connections.Values)
        {
            connection.MessageReceived -= OnMessageReceived;
            connection.Dispose();
        }
        _connections.Clear();

        await base.StopAsync(cancellationToken);
    }
}
