namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents port information in a System Crosspoint message.
/// </summary>
public class SystemCrosspointPortInfo
{
    /// <summary>
    /// The port number (bits 0-10 of the port info field).
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Whether this port is an audio source (bit 13).
    /// </summary>
    public bool IsAudioSource { get; set; }

    /// <summary>
    /// Whether this port is an audio destination (bit 14).
    /// </summary>
    public bool IsAudioDestination { get; set; }

    /// <summary>
    /// Whether this is a monitored port (bit 15).
    /// Specified once at the beginning of list of port info elements
    /// to which it is being talked or listened to.
    /// </summary>
    public bool IsMonitoredPort { get; set; }

    /// <summary>
    /// System number associated with this port.
    /// 0 = local matrix, otherwise linked set matrix ID.
    /// </summary>
    public byte SystemNumber { get; set; }
}

/// <summary>
/// Reply System Crosspoint (HCIv2) - Message ID 0x00C8 (200).
/// This message is used to either reply to the System Crosspoint Request
/// (and detail all remote routes to or from this matrix), or will be sent
/// unsolicited from the matrix for an individual route when a route is made or broken.
/// 
/// The unsolicited route transition version of this message will be sent from
/// both matrices that are at the end of the intelligent linking routing.
/// </summary>
public class ReplySystemCrosspoint
{
    /// <summary>
    /// Protocol schema version (should be 1).
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// List of port information entries with their associated system numbers.
    /// </summary>
    public List<SystemCrosspointPortInfo> Ports { get; set; } = new();

    /// <summary>
    /// Decodes a Reply System Crosspoint message from the payload.
    /// </summary>
    /// <param name="payload">The message payload (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplySystemCrosspoint Decode(byte[] payload)
    {
        var reply = new ReplySystemCrosspoint();

        if (payload == null || payload.Length < 7)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip validation, already checked
        offset += 4;

        // Protocol Schema: 1 byte
        if (offset < payload.Length)
            reply.ProtocolSchema = payload[offset++];

        // Count: 2 bytes (big-endian)
        if (offset + 2 > payload.Length)
            return reply;

        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Parse each entry (3 bytes each: 2 port info + 1 system number)
        for (int i = 0; i < count && offset + 3 <= payload.Length; i++)
        {
            // Port Info: 2 bytes (big-endian)
            ushort portInfo = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // System Number: 1 byte
            byte systemNumber = payload[offset++];

            var portEntry = new SystemCrosspointPortInfo
            {
                // Bits 0-10: Port number
                PortNumber = (ushort)(portInfo & 0x07FF),
                // Bit 13: Is audio source
                IsAudioSource = (portInfo & 0x2000) != 0,
                // Bit 14: Is audio destination
                IsAudioDestination = (portInfo & 0x4000) != 0,
                // Bit 15: Is monitored port
                IsMonitoredPort = (portInfo & 0x8000) != 0,
                // System number
                SystemNumber = systemNumber
            };

            reply.Ports.Add(portEntry);
        }

        return reply;
    }
}
