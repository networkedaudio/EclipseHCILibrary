using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a single forced listen edit entry.
/// </summary>
public class ForcedListenEdit
{
    /// <summary>
    /// Port number of the source.
    /// </summary>
    public ushort SourcePort { get; set; }

    /// <summary>
    /// System number of the source.
    /// </summary>
    public byte SourceSystem { get; set; }

    /// <summary>
    /// Port number of the destination.
    /// </summary>
    public ushort DestinationPort { get; set; }

    /// <summary>
    /// System number of the destination.
    /// </summary>
    public byte DestinationSystem { get; set; }

    /// <summary>
    /// Edit status (Added or Deleted).
    /// </summary>
    public ForcedListenEditStatus EditStatus { get; set; }
}

/// <summary>
/// Reply Forced Listen Edits (0x00CA).
/// Returns all forced listen edits.
/// </summary>
public class ReplyForcedListenEdits
{
    /// <summary>
    /// Count of number of edits returned.
    /// </summary>
    public ushort Count { get; set; }

    /// <summary>
    /// List of forced listen edit entries.
    /// </summary>
    public List<ForcedListenEdit> Edits { get; set; } = new();

    /// <summary>
    /// Parses the payload of a Reply Forced Listen Edits message.
    /// </summary>
    /// <param name="payload">The message payload (after protocol schema byte).</param>
    /// <returns>The parsed ReplyForcedListenEdits, or null if parsing fails.</returns>
    public static ReplyForcedListenEdits? Parse(byte[] payload)
    {
        // Minimum payload: Count(2) = 2 bytes
        if (payload == null || payload.Length < 2)
        {
            return null;
        }

        var result = new ReplyForcedListenEdits();

        // Count: 16 bit word (big-endian)
        result.Count = (ushort)((payload[0] << 8) | payload[1]);

        // Each entry:
        // SourcePort(2) + SourceSystem(1) + DestinationPort(2) + DestinationSystem(1) + EditStatus(1) = 7 bytes
        const int entrySize = 7;
        int offset = 2;

        for (int i = 0; i < result.Count; i++)
        {
            if (payload.Length < offset + entrySize)
            {
                break;
            }

            var edit = new ForcedListenEdit();

            // Source port: 2 bytes (big-endian)
            edit.SourcePort = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Source system: 1 byte
            edit.SourceSystem = payload[offset++];

            // Destination port: 2 bytes (big-endian)
            edit.DestinationPort = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Destination system: 1 byte
            edit.DestinationSystem = payload[offset++];

            // Edit status: 1 byte
            edit.EditStatus = payload[offset] == 1 
                ? ForcedListenEditStatus.Added 
                : ForcedListenEditStatus.Deleted;
            offset++;

            result.Edits.Add(edit);
        }

        return result;
    }
}
