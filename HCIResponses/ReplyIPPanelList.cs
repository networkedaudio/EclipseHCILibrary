using HCILibrary.Models;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply IP Panel List (HCIv2) - Message ID 0x00F8 (248), Sub ID 9.
/// This message is a reply to Request IP Panel List message. It contains the
/// contents of the discovered IP panel cache.
/// 
/// Note: If the total number of entries cannot fit in one message, multiple
/// messages will be sent. Each message will state the total number of entries
/// (spanning all messages).
/// </summary>
public class ReplyIPPanelList
{
    /// <summary>
    /// Sub ID for Panel Cache Reply.
    /// </summary>
    public const byte SubIdPanelCacheReply = 9;

    /// <summary>
    /// Protocol schema version (should be 1).
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The Sub ID for the reply. Should be 9 (Panel Cache Reply).
    /// </summary>
    public byte SubId { get; set; }

    /// <summary>
    /// Total number of entries in the current discovery cache.
    /// Note: If this number of entries cannot fit in one message, multiple
    /// messages will be sent. Each message will state the total number of
    /// entries (spanning all messages).
    /// </summary>
    public ushort TotalCount { get; set; }

    /// <summary>
    /// The list of IP panel entries in this message.
    /// </summary>
    public List<IPPanelEntry> Entries { get; set; } = new();

    /// <summary>
    /// Decodes a Reply IP Panel List message from the payload.
    /// </summary>
    /// <param name="payload">The message payload (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyIPPanelList Decode(byte[] payload)
    {
        var reply = new ReplyIPPanelList();

        if (payload == null || payload.Length < 8)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip validation, already checked
        offset += 4;

        // Protocol Schema: 1 byte
        if (offset < payload.Length)
            reply.ProtocolSchema = payload[offset++];

        // Sub ID: 1 byte (should be 9)
        if (offset < payload.Length)
            reply.SubId = payload[offset++];

        // Count: 2 bytes (big-endian) - total number of entries
        if (offset + 2 <= payload.Length)
        {
            reply.TotalCount = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Read entries until end of payload
        // Each entry is 55 bytes (1 byte length + 54 bytes data)
        while (offset < payload.Length)
        {
            // Check if we have at least the entry length byte
            if (offset >= payload.Length)
                break;

            // Peek at entry length to see if we have enough data
            byte entryLength = payload[offset];
            
            // Entry length should be 55 bytes as per protocol
            // If remaining payload is less than entry length + 1, stop
            if (offset + 1 + entryLength > payload.Length)
                break;

            var (entry, newOffset) = IPPanelEntry.Decode(payload, offset);
            reply.Entries.Add(entry);
            offset = newOffset;
        }

        return reply;
    }
}
