using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a single remote key action entry in the reply.
/// </summary>
public class ReplyRemoteKeyActionEntry
{
    /// <summary>
    /// Region of panel where key is located.
    /// </summary>
    public byte Region { get; set; }

    /// <summary>
    /// Page of panel where key is located.
    /// </summary>
    public byte Page { get; set; }

    /// <summary>
    /// Key position number.
    /// </summary>
    public byte Key { get; set; }

    /// <summary>
    /// Reserved byte.
    /// </summary>
    public byte Reserved { get; set; }

    /// <summary>
    /// Entity type for the key assignment.
    /// </summary>
    public ReplyKeyEntityType EntityType { get; set; }

    /// <summary>
    /// System number of the entity.
    /// </summary>
    public byte SystemNumber { get; set; }

    /// <summary>
    /// Reserved byte in entity reference.
    /// </summary>
    public byte EntityReserved { get; set; }

    /// <summary>
    /// Specific reference (e.g., port number, conference number, etc.).
    /// </summary>
    public ushort SpecificReference { get; set; }

    /// <summary>
    /// Key activation type.
    /// </summary>
    public KeyActivationType KeyActivation { get; set; }

    /// <summary>
    /// Latch mode (not currently supported).
    /// </summary>
    public KeyLatchMode LatchMode { get; set; }
}

/// <summary>
/// Reply Remote Key Actions (0x00EC).
/// Response to Request Remote Key Actions.
/// </summary>
public class ReplyRemoteKeyActions
{
    /// <summary>
    /// Assignment type.
    /// </summary>
    public RemoteKeyAssignmentType AssignmentType { get; set; }

    /// <summary>
    /// Number of key assignments in message.
    /// </summary>
    public ushort Count { get; set; }

    /// <summary>
    /// Port number of target panel.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// List of remote key action entries.
    /// </summary>
    public List<ReplyRemoteKeyActionEntry> Actions { get; set; } = new();

    /// <summary>
    /// Parses the payload of a Reply Remote Key Actions message.
    /// </summary>
    /// <param name="payload">The message payload (after protocol schema byte).</param>
    /// <returns>The parsed ReplyRemoteKeyActions, or null if parsing fails.</returns>
    public static ReplyRemoteKeyActions? Parse(byte[] payload)
    {
        // Minimum payload: Type(1) + Count(2) + PortNumber(2) = 5 bytes
        if (payload == null || payload.Length < 5)
        {
            return null;
        }

        var result = new ReplyRemoteKeyActions();
        int offset = 0;

        // Type: 1 byte
        result.AssignmentType = (RemoteKeyAssignmentType)payload[offset++];

        // Count: 2 bytes (big-endian)
        result.Count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Port Number: 2 bytes (big-endian)
        result.PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Each action entry:
        // Region(1) + Page(1) + Key(1) + Reserved(1) + EntityType(2) + EntityRef(4) + KeyActivation(1) + LatchMode(1) = 12 bytes
        const int entrySize = 12;

        for (int i = 0; i < result.Count; i++)
        {
            if (payload.Length < offset + entrySize)
            {
                break;
            }

            var entry = new ReplyRemoteKeyActionEntry();

            // Region: 1 byte
            entry.Region = payload[offset++];

            // Page: 1 byte
            entry.Page = payload[offset++];

            // Key: 1 byte
            entry.Key = payload[offset++];

            // Reserved: 1 byte
            entry.Reserved = payload[offset++];

            // Entity Type: 2 bytes (big-endian)
            ushort entityTypeValue = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            entry.EntityType = Enum.IsDefined(typeof(ReplyKeyEntityType), entityTypeValue)
                ? (ReplyKeyEntityType)entityTypeValue
                : ReplyKeyEntityType.Null;
            offset += 2;

            // Entity Reference: 4 bytes
            entry.SystemNumber = payload[offset++];
            entry.EntityReserved = payload[offset++];
            entry.SpecificReference = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Key Activation: 1 byte
            byte activationValue = payload[offset++];
            entry.KeyActivation = Enum.IsDefined(typeof(KeyActivationType), activationValue)
                ? (KeyActivationType)activationValue
                : KeyActivationType.TalkAndListen;

            // Latch Mode: 1 byte
            byte latchValue = payload[offset++];
            entry.LatchMode = Enum.IsDefined(typeof(KeyLatchMode), latchValue)
                ? (KeyLatchMode)latchValue
                : KeyLatchMode.LatchNonLatch;

            result.Actions.Add(entry);
        }

        return result;
    }
}
