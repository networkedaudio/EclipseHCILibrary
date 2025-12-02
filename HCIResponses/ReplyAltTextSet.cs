namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Alt Text Set (HCIv2) - Message ID 0x0181 (379).
/// This message is used to reply to the Request Alt Text Set message.
/// </summary>
public class ReplyAltTextSet
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// 0-based port number of the panel.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Alt Text state: true if on, false if off.
    /// </summary>
    public bool State { get; set; }

    /// <summary>
    /// Gets whether Alt Text is enabled for this panel.
    /// </summary>
    public bool IsAltTextEnabled => State;

    /// <summary>
    /// Decodes the payload into a ReplyAltTextSet.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyAltTextSet Decode(byte[] payload)
    {
        var reply = new ReplyAltTextSet();

        if (payload.Length < 8)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Port Number: 2 bytes (big-endian)
        reply.PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // State: 1 byte (0 = Off, 1 = On)
        reply.State = payload[offset++] == 1;

        return reply;
    }
}
