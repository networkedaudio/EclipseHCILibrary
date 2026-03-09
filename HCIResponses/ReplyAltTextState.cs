namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a single Alt Text state entry for a panel.
/// </summary>
public class AltTextStateEntry
{
    /// <summary>
    /// 0-based port number of the panel.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Alt Text state: true if on, false if off.
    /// </summary>
    public bool State { get; set; }
}

/// <summary>
/// Reply Alt Text State (HCIv2) - Message ID 0x0179 (377).
/// Reports the state of the Alt Text feature for one or more panels.
/// Sent in response to Request Alt Text State, or unsolicited when toggled at the panel.
/// If sent in response to a get-all (port == 0xFFFF), only panels with Alt Text active are included.
/// </summary>
public class ReplyAltTextState
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// Number of entries in this message.
    /// </summary>
    public ushort Count { get; set; }

    /// <summary>
    /// List of Alt Text state entries.
    /// </summary>
    public List<AltTextStateEntry> Entries { get; set; } = new();

    /// <summary>
    /// Decodes the payload into a ReplyAltTextState.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyAltTextState Decode(byte[] payload)
    {
        var reply = new ReplyAltTextState();

        if (payload.Length < 8)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Count: 2 bytes (big-endian)
        if (offset + 2 > payload.Length)
            return reply;
        reply.Count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Parse entries: Port(2) + State(1) = 3 bytes each
        for (int i = 0; i < reply.Count && offset + 3 <= payload.Length; i++)
        {
            var entry = new AltTextStateEntry
            {
                // Port Number: 2 bytes (big-endian)
                PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]),
            };
            offset += 2;

            // State: 1 byte (0 = Off, 1 = On)
            entry.State = payload[offset++] == 1;

            reply.Entries.Add(entry);
        }

        return reply;
    }
}
