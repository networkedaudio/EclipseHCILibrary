using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a single assigned key entry.
/// </summary>
public class AssignedKey
{
    /// <summary>
    /// Region this key is on.
    /// </summary>
    public byte Region { get; set; }

    /// <summary>
    /// Key ID.
    /// </summary>
    public byte Key { get; set; }

    /// <summary>
    /// Page this key is on.
    /// </summary>
    public byte Page { get; set; }

    /// <summary>
    /// Entity type (e.g., EN_PORT, EN_GROUP).
    /// </summary>
    public ushort Entity { get; set; }

    /// <summary>
    /// Key status information.
    /// </summary>
    public LocalKeyStatus? KeyStatus { get; set; }

    /// <summary>
    /// Key operation settings.
    /// </summary>
    public KeyOperation? KeyOperation { get; set; }

    /// <summary>
    /// Key configuration.
    /// </summary>
    public KeyConfig? KeyConfig { get; set; }
}

/// <summary>
/// Reply Assigned Keys (0x00E8).
/// Returns the configuration of all assigned keys for a selected panel.
/// These assignments are the net result of map configuration downloads
/// plus any HCI, online, or panel-based assignments.
/// </summary>
public class ReplyAssignedKeys
{
    /// <summary>
    /// Protocol schema version (1 or 2).
    /// </summary>
    public AssignedKeysSchema Schema { get; set; }

    /// <summary>
    /// Card slot number.
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    /// Port offset from first port of the card.
    /// </summary>
    public byte Port { get; set; }

    /// <summary>
    /// Endpoint type (Schema 2 only).
    /// Only used when slot and port target a role.
    /// If not a role target, this field is 0.
    /// Example: 0x8200 signifies FS II beltpack.
    /// </summary>
    public ushort EndpointType { get; set; }

    /// <summary>
    /// Number of assigned keys on this panel.
    /// </summary>
    public ushort Count { get; set; }

    /// <summary>
    /// List of assigned key entries.
    /// </summary>
    public List<AssignedKey> Keys { get; set; } = new();

    /// <summary>
    /// Parses the payload of a Reply Assigned Keys message.
    /// </summary>
    /// <param name="payload">The message payload (after protocol tag).</param>
    /// <returns>The parsed ReplyAssignedKeys, or null if parsing fails.</returns>
    public static ReplyAssignedKeys? Parse(byte[] payload)
    {
        // Minimum payload: Schema(1) + Slot(1) + Port(1) + Count(2) = 5 bytes (Schema 1)
        // For Schema 2: Schema(1) + Slot(1) + Port(1) + EndpointType(2) + Count(2) = 7 bytes
        if (payload == null || payload.Length < 5)
        {
            return null;
        }

        var result = new ReplyAssignedKeys();
        int offset = 0;

        // Schema: 1 byte
        byte schemaValue = payload[offset++];
        result.Schema = schemaValue == 2 ? AssignedKeysSchema.Schema2 : AssignedKeysSchema.Schema1;

        // Slot: 1 byte
        result.Slot = payload[offset++];

        // Port: 1 byte
        result.Port = payload[offset++];

        // EndpointType: 2 bytes (Schema 2 only)
        if (result.Schema == AssignedKeysSchema.Schema2)
        {
            if (payload.Length < 7)
            {
                return null;
            }
            result.EndpointType = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Count: 2 bytes
        if (payload.Length < offset + 2)
        {
            return null;
        }
        result.Count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Each key entry:
        // Region(1) + Key(1) + Page(1) + Entity(2) + KeyStatus(1) + PotNumber(2) + 
        // KeyOperation(4) + KeyConfig(26) = 38 bytes
        const int keyEntrySize = 38;

        for (int i = 0; i < result.Count; i++)
        {
            if (payload.Length < offset + keyEntrySize)
            {
                break;
            }

            var key = new AssignedKey
            {
                Region = payload[offset],
                Key = payload[offset + 1],
                Page = payload[offset + 2],
                Entity = (ushort)((payload[offset + 3] << 8) | payload[offset + 4]),
                KeyStatus = LocalKeyStatus.Parse(payload, offset + 5),
                KeyOperation = KeyOperation.Parse(payload, offset + 8),
                KeyConfig = KeyConfig.Parse(payload, offset + 12)
            };

            result.Keys.Add(key);
            offset += keyEntrySize;
        }

        return result;
    }
}
