namespace HCILibrary.HCIResponses;

/// <summary>
/// Get Panel Audio Front End State Reply (HCIv2) - Message ID 0x0164 (356).
/// This message is sent in response to the Get Panel Audio Front End State Request
/// acknowledging the successful receipt and processing of the request.
/// The message contains the current state of the HCI requested panel audio front end
/// of the specified panel.
/// </summary>
public class ReplyGetPanelAudioFrontEndState
{
    /// <summary>
    /// Protocol schema version (should be 1).
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The panel port number.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// The enabled state of the panel audio front end.
    /// True if enabled (mic, speaker, sidetone active as if talk key pressed), false if disabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Decodes a Get Panel Audio Front End State Reply message from the payload.
    /// </summary>
    /// <param name="payload">The message payload (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyGetPanelAudioFrontEndState Decode(byte[] payload)
    {
        var reply = new ReplyGetPanelAudioFrontEndState();

        if (payload == null || payload.Length < 8)
            return reply;

        int offset = 0;

        // Protocol Tag (Magic Number): 4 bytes (0xABBACEDE) - skip validation, already checked
        offset += 4;

        // Schema Number: 1 byte
        if (offset < payload.Length)
            reply.ProtocolSchema = payload[offset++];

        // Port Number: 2 bytes (big-endian)
        if (offset + 2 <= payload.Length)
        {
            reply.PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Enabled State: 1 byte (0 = disabled, 1 = enabled)
        if (offset < payload.Length)
            reply.Enabled = payload[offset++] != 0;

        return reply;
    }
}
