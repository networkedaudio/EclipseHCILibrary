using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a single conference member edit entry.
/// </summary>
public class ConferenceMemberEdit
{
    /// <summary>
    /// Conference number (starting at 1).
    /// </summary>
    public ushort ConferenceNumber { get; set; }

    /// <summary>
    /// Edit type flags (Talk, Listen, LocalDeleted, LocalAssigned, MapAssigned, LocalOverride).
    /// </summary>
    public ConferenceEditFlags EditType { get; set; }

    /// <summary>
    /// System number (1 to 15).
    /// </summary>
    public byte SystemNumber { get; set; }

    /// <summary>
    /// Port number (1 to 1023).
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Gets whether the member can talk in the conference.
    /// </summary>
    public bool CanTalk => (EditType & ConferenceEditFlags.Talk) != 0;

    /// <summary>
    /// Gets whether the member can listen in the conference.
    /// </summary>
    public bool CanListen => (EditType & ConferenceEditFlags.Listen) != 0;

    /// <summary>
    /// Gets whether the member has been locally deleted.
    /// </summary>
    public bool IsLocalDeleted => (EditType & ConferenceEditFlags.LocalDeleted) != 0;

    /// <summary>
    /// Gets whether the member has been locally assigned.
    /// </summary>
    public bool IsLocalAssigned => (EditType & ConferenceEditFlags.LocalAssigned) != 0;

    /// <summary>
    /// Gets whether the member has been map assigned.
    /// </summary>
    public bool IsMapAssigned => (EditType & ConferenceEditFlags.MapAssigned) != 0;

    /// <summary>
    /// Gets whether the member has a local override.
    /// </summary>
    public bool HasLocalOverride => (EditType & ConferenceEditFlags.LocalOverride) != 0;
}

/// <summary>
/// Reply Conference Assignments (0x00C6).
/// Returns members of all conferences specifying whether it is a talk, listen
/// and whether it has been edited locally.
/// </summary>
public class ReplyConferenceAssignments
{
    /// <summary>
    /// Count of items returned.
    /// </summary>
    public ushort Count { get; set; }

    /// <summary>
    /// List of conference member edit entries.
    /// </summary>
    public List<ConferenceMemberEdit> Members { get; set; } = new();

    /// <summary>
    /// Parses the payload of a Reply Conference Assignments message.
    /// </summary>
    /// <param name="payload">The message payload (after protocol schema byte).</param>
    /// <returns>The parsed ReplyConferenceAssignments, or null if parsing fails.</returns>
    public static ReplyConferenceAssignments? Parse(byte[] payload)
    {
        // Minimum payload: Count(2) = 2 bytes
        if (payload == null || payload.Length < 2)
        {
            return null;
        }

        var result = new ReplyConferenceAssignments();

        // Count: 16 bit word (big-endian)
        result.Count = (ushort)((payload[0] << 8) | payload[1]);

        // Each member edit entry:
        // ConferenceNumber(2) + EditType(1) + SystemNumber(1) + PortNumber(2) = 6 bytes
        const int memberEditSize = 6;
        int offset = 2;

        for (int i = 0; i < result.Count; i++)
        {
            if (payload.Length < offset + memberEditSize)
            {
                break;
            }

            var member = new ConferenceMemberEdit();

            // Conference number: 2 bytes (big-endian)
            member.ConferenceNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Edit type: 1 byte
            member.EditType = (ConferenceEditFlags)payload[offset];
            offset++;

            // System number: 1 byte
            member.SystemNumber = payload[offset];
            offset++;

            // Port number: 2 bytes (big-endian)
            member.PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            result.Members.Add(member);
        }

        return result;
    }
}
