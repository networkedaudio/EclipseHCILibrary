using HCILibrary.Enums;

namespace HCILibrary.Models;

/// <summary>
/// Represents an endpoint's network redundancy status entry.
/// </summary>
public class NetworkRedundancyEndpointEntry
{
    /// <summary>
    /// Port number associated with the endpoint.
    /// </summary>
    public ushort PhysicalPort { get; set; }

    /// <summary>
    /// The currently active LAN port for this endpoint.
    /// </summary>
    public ActiveLanPort ActiveLanPort { get; set; }

    /// <summary>
    /// Number of times the endpoint has switched LAN ports since boot up.
    /// </summary>
    public ushort SwitchCount { get; set; }

    /// <summary>
    /// Gets whether the endpoint is using the main LAN port.
    /// </summary>
    public bool IsUsingMain => ActiveLanPort == ActiveLanPort.Main;

    /// <summary>
    /// Gets whether the endpoint is using the backup LAN port.
    /// </summary>
    public bool IsUsingBackup => ActiveLanPort == ActiveLanPort.Backup;

    /// <summary>
    /// Gets whether the active LAN port is unknown.
    /// </summary>
    public bool IsUnknown => ActiveLanPort == ActiveLanPort.Unknown;

    /// <summary>
    /// Creates a new NetworkRedundancyEndpointEntry.
    /// </summary>
    public NetworkRedundancyEndpointEntry()
    {
    }

    /// <summary>
    /// Creates a new NetworkRedundancyEndpointEntry with specified values.
    /// </summary>
    /// <param name="physicalPort">The port number.</param>
    /// <param name="activeLanPort">The active LAN port.</param>
    /// <param name="switchCount">The switch count.</param>
    public NetworkRedundancyEndpointEntry(ushort physicalPort, ActiveLanPort activeLanPort, ushort switchCount)
    {
        PhysicalPort = physicalPort;
        ActiveLanPort = activeLanPort;
        SwitchCount = switchCount;
    }
}
