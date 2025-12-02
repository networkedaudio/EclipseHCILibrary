using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a single key group status entry.
/// </summary>
public class KeyGroupStatusEntry
{
    /// <summary>
    /// System number of the key group.
    /// </summary>
    public byte KeyGroupSystemNumber { get; set; }

    /// <summary>
    /// Instance number of the key group.
    /// </summary>
    public ushort KeyGroupId { get; set; }

    /// <summary>
    /// System number of the target entity (port, IFB, etc.).
    /// </summary>
    public byte TargetEntitySystemId { get; set; }

    /// <summary>
    /// Type ID of the target entity.
    /// </summary>
    public KeyEntityType EntityType { get; set; }

    /// <summary>
    /// Instance number of the target entity.
    /// </summary>
    public ushort EntityInstance { get; set; }
}

/// <summary>
/// Reply Key Group Status (HCIv2) - Message ID 0x00FC (252).
/// This message is used to reply to the Request Key Group Status message.
/// Contains the current assignment state of the specified key group(s).
/// </summary>
public class ReplyKeyGroupStatus
{
    /// <summary>
    /// Protocol schema version (should be 1).
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The list of key group status entries.
    /// </summary>
    public List<KeyGroupStatusEntry> Entries { get; set; } = new();

    /// <summary>
    /// Decodes a Reply Key Group Status message from the payload.
    /// </summary>
    /// <param name="payload">The message payload (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyKeyGroupStatus Decode(byte[] payload)
    {
        var reply = new ReplyKeyGroupStatus();

        if (payload == null || payload.Length < 5)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip validation, already checked
        offset += 4;

        // Protocol Schema: 1 byte
        if (offset < payload.Length)
            reply.ProtocolSchema = payload[offset++];

        // Read repeated entries until end of payload
        // Each entry is 7 bytes:
        //   Key Group System Number: 1 byte
        //   Key Group ID: 2 bytes
        //   Target Entity System ID: 1 byte
        //   Entity Type: 1 byte
        //   Entity Instance: 2 bytes
        const int entrySize = 7;

        while (offset + entrySize <= payload.Length)
        {
            var entry = new KeyGroupStatusEntry();

            // Key Group System Number: 1 byte
            entry.KeyGroupSystemNumber = payload[offset++];

            // Key Group ID: 2 bytes (big-endian)
            entry.KeyGroupId = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Target Entity System ID: 1 byte
            entry.TargetEntitySystemId = payload[offset++];

            // Entity Type: 1 byte
            byte entityTypeValue = payload[offset++];
            entry.EntityType = Enum.IsDefined(typeof(KeyEntityType), (ushort)entityTypeValue)
                ? (KeyEntityType)entityTypeValue
                : KeyEntityType.Null;

            // Entity Instance: 2 bytes (big-endian)
            entry.EntityInstance = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            reply.Entries.Add(entry);
        }

        return reply;
    }
}
