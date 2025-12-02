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
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyBeltpackDelete Decode(byte[] payload)
    {
        var reply = new ReplyBeltpackDelete();

        if (payload.Length < 7)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Reserved: 1 byte - skip
        offset++;

        // Success: 1 byte (0 = Success, 1 = Entry Not Found)
        reply.Success = payload[offset] == 0;

        return reply;
    }
}
