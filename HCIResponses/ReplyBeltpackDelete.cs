using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Beltpack Delete (HCIv2) - Message ID 0x0196 (406).
/// This message is used to reply to the Request Beltpack Delete message.
/// </summary>
public class ReplyBeltpackDelete
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// Whether the beltpack was successfully deleted.
    /// True if success (0), false if entry not found (1).
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Whether the beltpack entry was not found.
    /// </summary>
    public bool EntryNotFound => !Success;

    /// <summary>
    /// Decodes the payload into a ReplyBeltpackDelete.
    /// </summary>
    /// <param name="payload">The payload bytes (after protocol tag and schema have been stripped).</param>
    /// <param name="schema">The protocol schema version from the message header.</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyBeltpackDelete Decode(byte[] payload, byte schema)
    {
        var reply = new ReplyBeltpackDelete
        {
            ProtocolSchema = schema
        };

        // Note: The protocol tag (AB BA CE DE) and schema byte have already been
        // stripped by HCIResponse.cs, so the payload starts with the reserved byte.

        if (payload.Length < 2)
            return reply;

        int offset = 0;

        // Reserved: 1 byte - skip
        offset++;

        // Success: 1 byte (0 = Success, 1 = Entry Not Found)
        reply.Success = payload[offset] == 0;

        return reply;
    }
}
