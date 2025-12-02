namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Panel Discovery (HCIv2) - Message ID 0x00F8 (248).
/// This message is a reply to the IP panel discovery type request.
/// </summary>
public class ReplyPanelDiscovery
{
    /// <summary>
    /// Sub ID for Discovery Reply.
    /// </summary>
    public const byte SubIdDiscoveryReply = 1;

    /// <summary>
    /// Protocol schema version (should be 1).
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The Sub ID for the reply. Should be 1 (Discovery Reply).
    /// </summary>
    public byte SubId { get; set; }

    /// <summary>
    /// Decodes a Reply Panel Discovery message from the payload.
    /// </summary>
    /// <param name="payload">The message payload (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyPanelDiscovery Decode(byte[] payload)
    {
        var reply = new ReplyPanelDiscovery();

        if (payload == null || payload.Length < 6)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip validation, already checked
        offset += 4;

        // Protocol Schema: 1 byte
        if (offset < payload.Length)
            reply.ProtocolSchema = payload[offset++];

        // Sub ID: 1 byte
        if (offset < payload.Length)
            reply.SubId = payload[offset++];

        return reply;
    }
}
