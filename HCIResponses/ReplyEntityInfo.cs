using System.Text;
using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents the dial-code of an entity instance.
/// </summary>
public class EntityDialCode
{
    /// <summary>
    /// System number (8 bits).
    /// </summary>
    public byte SystemNumber { get; set; }

    /// <summary>
    /// Entity type (8 bits).
    /// </summary>
    public byte EntityType { get; set; }

    /// <summary>
    /// Instance of entity (16 bits).
    /// </summary>
    public ushort Instance { get; set; }

    /// <summary>
    /// Gets the entity type as an EntityInfoType enum value.
    /// </summary>
    public EntityInfoType EntityInfoType => EntityType switch
    {
        2 => EntityInfoType.Conferences,
        3 => EntityInfoType.Groups,
        4 => EntityInfoType.Ifbs,
        _ => EntityInfoType.All
    };

    /// <summary>
    /// Gets the full 32-bit dial code value.
    /// </summary>
    public uint FullDialCode => (uint)((SystemNumber << 24) | (EntityType << 16) | Instance);

    /// <summary>
    /// Parses a dial code from 4 bytes.
    /// </summary>
    /// <param name="payload">The payload bytes.</param>
    /// <param name="offset">The offset to start reading from.</param>
    /// <returns>The parsed dial code.</returns>
    public static EntityDialCode Parse(byte[] payload, int offset)
    {
        return new EntityDialCode
        {
            SystemNumber = payload[offset],
            EntityType = payload[offset + 1],
            Instance = (ushort)((payload[offset + 2] << 8) | payload[offset + 3])
        };
    }
}

/// <summary>
/// Represents a single entity info entry.
/// </summary>
public class EntityInfoEntry
{
    /// <summary>
    /// The dial-code of the entity instance.
    /// </summary>
    public EntityDialCode DialCode { get; set; } = new();

    /// <summary>
    /// The Unicode name/label of the entity.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the system number from the dial code.
    /// </summary>
    public byte SystemNumber => DialCode.SystemNumber;

    /// <summary>
    /// Gets the entity type from the dial code.
    /// </summary>
    public EntityInfoType EntityType => DialCode.EntityInfoType;

    /// <summary>
    /// Gets the entity instance number from the dial code.
    /// </summary>
    public ushort Instance => DialCode.Instance;
}

/// <summary>
/// Reply Entity Info (0x00B0).
/// Response to Request Entity Info. Contains entity instance information for
/// the supported entity types (Conferences, Groups, IFBs) in the current matrix frame configuration.
/// May span multiple messages - check the continuation flag in the HCI flags.
/// </summary>
public class ReplyEntityInfo
{
    /// <summary>
    /// Protocol schema version from the response.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The entity type that was requested.
    /// </summary>
    public EntityInfoType RequestEntityType { get; set; }

    /// <summary>
    /// Number of entity entries in this message.
    /// </summary>
    public ushort Count { get; set; }

    /// <summary>
    /// List of entity info entries.
    /// </summary>
    public List<EntityInfoEntry> Entities { get; set; } = new();

    /// <summary>
    /// Parses a Reply Entity Info response from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags, starting at protocol schema).</param>
    /// <returns>The parsed response, or null if parsing fails.</returns>
    public static ReplyEntityInfo? Parse(byte[] payload)
    {
        // Minimum payload: ProtocolSchema(1) + RequestEntityType(1) + Count(2) = 4 bytes
        if (payload == null || payload.Length < 4)
        {
            return null;
        }

        int offset = 0;
        var result = new ReplyEntityInfo();

        // Protocol Schema (1 byte)
        result.ProtocolSchema = payload[offset++];

        // Request Entity Type (1 byte)
        byte entityTypeValue = payload[offset++];
        result.RequestEntityType = entityTypeValue switch
        {
            0 => EntityInfoType.All,
            2 => EntityInfoType.Conferences,
            3 => EntityInfoType.Groups,
            4 => EntityInfoType.Ifbs,
            _ => EntityInfoType.All
        };

        // Count (2 bytes, big-endian)
        result.Count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Parse each entity entry
        for (int i = 0; i < result.Count; i++)
        {
            var entry = ParseEntry(payload, ref offset);
            if (entry == null)
            {
                break;
            }
            result.Entities.Add(entry);
        }

        return result;
    }

    private static EntityInfoEntry? ParseEntry(byte[] payload, ref int offset)
    {
        // Minimum entry size: DialCode(4) + NameLength(2) = 6 bytes
        const int minEntrySize = 6;

        if (payload.Length < offset + minEntrySize)
        {
            return null;
        }

        var entry = new EntityInfoEntry();

        // Dial-code of entity instance (4 bytes)
        entry.DialCode = EntityDialCode.Parse(payload, offset);
        offset += 4;

        // Name Length (2 bytes, big-endian)
        ushort nameLength = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Name (variable length, Unicode/UTF-16BE)
        if (payload.Length < offset + nameLength)
        {
            // Not enough data for the name, but we can still return partial entry
            entry.Name = string.Empty;
            return entry;
        }

        if (nameLength > 0)
        {
            try
            {
                // Parse as UTF-16 Big Endian
                entry.Name = Encoding.BigEndianUnicode.GetString(payload, offset, nameLength);
                // Trim null characters
                int nullIndex = entry.Name.IndexOf('\0');
                if (nullIndex >= 0)
                {
                    entry.Name = entry.Name.Substring(0, nullIndex);
                }
            }
            catch
            {
                entry.Name = string.Empty;
            }
            offset += nameLength;
        }

        return entry;
    }

    /// <summary>
    /// Gets all conference entities from the response.
    /// </summary>
    public IEnumerable<EntityInfoEntry> Conferences => 
        Entities.Where(e => e.EntityType == EntityInfoType.Conferences);

    /// <summary>
    /// Gets all group entities from the response.
    /// </summary>
    public IEnumerable<EntityInfoEntry> Groups => 
        Entities.Where(e => e.EntityType == EntityInfoType.Groups);

    /// <summary>
    /// Gets all IFB entities from the response.
    /// </summary>
    public IEnumerable<EntityInfoEntry> Ifbs => 
        Entities.Where(e => e.EntityType == EntityInfoType.Ifbs);
}
