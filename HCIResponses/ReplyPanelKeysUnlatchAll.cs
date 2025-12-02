namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Panel Keys Unlatch All (HCIv2) - Message ID 0x014F (335).
/// This message is used to reply to the Panel Keys Unlatch All Request.
/// </summary>
public class ReplyPanelKeysUnlatchAll
{
    /// <summary>
    /// Protocol schema version (should be 1).
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// Card slot number.
    /// </summary>
    public byte CardSlot { get; set; }

    /// <summary>
    /// Zero-based offset into panel card.
    /// </summary>
    public byte PortOffsetOnCard { get; set; }

    /// <summary>
    /// Decodes a Reply Panel Keys Unlatch All message from the payload.
    /// </summary>
    /// <param name="payload">The message payload (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyPanelKeysUnlatchAll Decode(byte[] payload)
    {
        var reply = new ReplyPanelKeysUnlatchAll();

        if (payload == null || payload.Length < 7)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip validation, already checked
        offset += 4;

        // Protocol Schema: 1 byte
        if (offset < payload.Length)
            reply.ProtocolSchema = payload[offset++];

        // Card Slot: 1 byte
        if (offset < payload.Length)
            reply.CardSlot = payload[offset++];

        // Port Offset On Card: 1 byte
        if (offset < payload.Length)
            reply.PortOffsetOnCard = payload[offset++];

        return reply;
    }
}
