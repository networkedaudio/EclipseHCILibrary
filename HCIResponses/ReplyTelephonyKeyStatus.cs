using System.Text;
using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Telephony Key Status (HCIv2) - Message ID 0x015C (348).
/// This message is used to send unsolicited key pressed state information to
/// connected Host applications. This message is only transmitted from the
/// matrix hosting the panel if telephony key state transitions have been
/// enabled using the Request Telephony Key Status Enable message.
/// </summary>
public class ReplyTelephonyKeyStatus
{
    /// <summary>
    /// Protocol schema version (should be 1).
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// Port number of panel hosting the key (zero-based port number).
    /// </summary>
    public ushort PanelNumber { get; set; }

    /// <summary>
    /// Key page number.
    /// </summary>
    public byte Page { get; set; }

    /// <summary>
    /// Key region number.
    /// </summary>
    public byte Region { get; set; }

    /// <summary>
    /// Key ID.
    /// </summary>
    public byte Key { get; set; }

    /// <summary>
    /// Unicode text buffer associated with the key (up to 40 bytes).
    /// </summary>
    public string KeyText { get; set; } = string.Empty;

    /// <summary>
    /// The current state of the key.
    /// </summary>
    public TelephonyKeyState State { get; set; }

    /// <summary>
    /// Decodes a Reply Telephony Key Status message from the payload.
    /// </summary>
    /// <param name="payload">The message payload (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyTelephonyKeyStatus Decode(byte[] payload)
    {
        var reply = new ReplyTelephonyKeyStatus();

        if (payload == null || payload.Length < 6)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip validation, already checked
        offset += 4;

        // Protocol Schema: 1 byte
        if (offset < payload.Length)
            reply.ProtocolSchema = payload[offset++];

        // Panel Number: 2 bytes (big-endian)
        if (offset + 2 <= payload.Length)
        {
            reply.PanelNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Page: 1 byte
        if (offset < payload.Length)
            reply.Page = payload[offset++];

        // Region: 1 byte
        if (offset < payload.Length)
            reply.Region = payload[offset++];

        // Key: 1 byte
        if (offset < payload.Length)
            reply.Key = payload[offset++];

        // Key Text: 40 bytes (Unicode)
        if (offset + 40 <= payload.Length)
        {
            // Unicode is typically UTF-16LE in Windows environments
            reply.KeyText = Encoding.Unicode.GetString(payload, offset, 40).TrimEnd('\0');
            offset += 40;
        }

        // State: 1 byte
        if (offset < payload.Length)
            reply.State = (TelephonyKeyState)payload[offset++];

        return reply;
    }
}
