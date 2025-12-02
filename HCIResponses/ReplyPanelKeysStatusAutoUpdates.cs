namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Panel Keys Status Auto Updates (HCIv2) - Message ID 0x013F (319).
/// This message is used to acknowledge the Host application of the received
/// Panel Keys Status Auto Updates Request message.
/// </summary>
public class ReplyPanelKeysStatusAutoUpdates
{
    /// <summary>
    /// The schema version from the message.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// The first port number the setting was applied to.
    /// </summary>
    public ushort PortNumberStart { get; set; }

    /// <summary>
    /// The last port number the setting was applied to.
    /// </summary>
    public ushort PortNumberEnd { get; set; }

    /// <summary>
    /// Whether auto updates are enabled for the specified port range.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Decodes a Reply Panel Keys Status Auto Updates message from payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes to decode.</param>
    /// <returns>A new ReplyPanelKeysStatusAutoUpdates instance.</returns>
    public static ReplyPanelKeysStatusAutoUpdates Decode(byte[] payload)
    {
        var reply = new ReplyPanelKeysStatusAutoUpdates();

        if (payload == null || payload.Length < 10)
        {
            return reply;
        }

        int offset = 0;

        // Check for HCIv2 marker (0xAB 0xBA 0xCE 0xDE)
        if (payload.Length >= 4 &&
            payload[0] == 0xAB && payload[1] == 0xBA &&
            payload[2] == 0xCE && payload[3] == 0xDE)
        {
            offset = 4;
        }

        // Protocol Schema (1 byte)
        if (offset < payload.Length)
        {
            reply.Schema = payload[offset++];
        }

        // Port Number Start (2 bytes, big-endian)
        if (offset + 2 <= payload.Length)
        {
            reply.PortNumberStart = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Port Number End (2 bytes, big-endian)
        if (offset + 2 <= payload.Length)
        {
            reply.PortNumberEnd = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Enabled/Disabled (1 byte)
        if (offset < payload.Length)
        {
            reply.Enabled = payload[offset] != 0;
        }

        return reply;
    }
}
