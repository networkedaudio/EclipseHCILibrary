namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Telephony Key Status Enable (HCIv2) - Message ID 0x015B (347).
/// This message is used to reply to the Telephony Key Status Enable Request.
/// </summary>
public class ReplyTelephonyKeyStatusEnable
{
    /// <summary>
    /// Protocol schema version (should be 1).
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// Whether telephony key status messages are enabled.
    /// When true, the matrix will send unsolicited Reply Telephony Key Status messages
    /// when telephony key state transitions occur.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Decodes a Reply Telephony Key Status Enable message from the payload.
    /// </summary>
    /// <param name="payload">The message payload (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyTelephonyKeyStatusEnable Decode(byte[] payload)
    {
        var reply = new ReplyTelephonyKeyStatusEnable();

        if (payload == null || payload.Length < 6)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip validation, already checked
        offset += 4;

        // Protocol Schema: 1 byte
        if (offset < payload.Length)
            reply.ProtocolSchema = payload[offset++];

        // Enabled: 1 byte (1 = enabled, 0 = disabled)
        if (offset < payload.Length)
            reply.Enabled = payload[offset++] != 0;

        return reply;
    }
}
