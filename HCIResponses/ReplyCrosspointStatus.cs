namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a connected port's status for a monitored port.
/// </summary>
public class CrosspointPortStatus
{
    /// <summary>
    /// The connected port number (0-1023).
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Whether this port is a listener.
    /// </summary>
    public bool IsListener { get; set; }

    /// <summary>
    /// Whether this port is a talker.
    /// </summary>
    public bool IsTalker { get; set; }

    public override string ToString()
    {
        var role = (IsListener, IsTalker) switch
        {
            (true, true) => "Listener+Talker",
            (true, false) => "Listener",
            (false, true) => "Talker",
            _ => "None"
        };
        return $"Port {PortNumber}: {role}";
    }
}

/// <summary>
/// Represents a monitored port and its connected ports.
/// </summary>
public class MonitoredPortStatus
{
    /// <summary>
    /// The monitored port number (0-1023).
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// The list of ports connected to this monitored port.
    /// </summary>
    public List<CrosspointPortStatus> ConnectedPorts { get; } = new();

    public override string ToString()
    {
        return $"Monitored Port {PortNumber}: {ConnectedPorts.Count} connection(s)";
    }
}

/// <summary>
/// Reply Crosspoint Status (HCIv2) - Message ID 0x000E.
/// Contains information on the status of the crosspoints that are made to ports.
/// Generated in response to Request Crosspoint Status message or automatically
/// when crosspoint status changes.
/// </summary>
public class ReplyCrosspointStatus
{
    /// <summary>
    /// The list of monitored ports and their connections.
    /// </summary>
    public List<MonitoredPortStatus> MonitoredPorts { get; } = new();

    /// <summary>
    /// Decodes a Reply Crosspoint Status from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes (after protocol tag and schema).</param>
    /// <returns>The decoded reply, or null if invalid.</returns>
    public static ReplyCrosspointStatus? Decode(byte[] payload)
    {
        // Minimum: Count(2) = 2 bytes
        if (payload == null || payload.Length < 2)
        {
            return null;
        }

        var reply = new ReplyCrosspointStatus();
        int offset = 0;

        // Count: 16 bit word (big-endian) - total number of port data entries
        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        MonitoredPortStatus? currentMonitored = null;

        // Port data entries
        for (int i = 0; i < count && offset + 2 <= payload.Length; i++)
        {
            ushort portData = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Check bit 15 to determine if this is a monitored port or connected port
            bool isMonitoredPort = (portData & 0x8000) != 0;

            if (isMonitoredPort)
            {
                // Monitored Port:
                // bits 0-12: port number
                // bits 13,14: set to 0
                // bit 15: monitored port bit, set to 1
                currentMonitored = new MonitoredPortStatus
                {
                    PortNumber = (ushort)(portData & 0x1FFF)
                };
                reply.MonitoredPorts.Add(currentMonitored);
            }
            else if (currentMonitored != null)
            {
                // Connected Port:
                // bits 0-12: port number
                // bit 13: listener flag
                // bit 14: talker flag
                // bit 15: connected port bit, set to 0
                var connectedPort = new CrosspointPortStatus
                {
                    PortNumber = (ushort)(portData & 0x1FFF),
                    IsListener = (portData & 0x2000) != 0,
                    IsTalker = (portData & 0x4000) != 0
                };
                currentMonitored.ConnectedPorts.Add(connectedPort);
            }
        }

        return reply;
    }

    public override string ToString()
    {
        return $"Crosspoint Status: {MonitoredPorts.Count} monitored port(s)";
    }
}
