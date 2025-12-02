using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Conference / Fixed Group Members Edits (0x00C5).
/// Request all locally edited members of all Fixed groups or Conferences (partylines).
/// </summary>
public class RequestConferenceFixedGroupMembersEditsRequest : HCIRequest
{
    /// <summary>
    /// The edit type (Conference = 2, Group = 3).
    /// </summary>
    public ConferenceEditType EditType { get; set; } = ConferenceEditType.Conference;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestConferenceFixedGroupMembersEditsRequest"/> class.
    /// </summary>
    public RequestConferenceFixedGroupMembersEditsRequest()
        : base(HCIMessageID.RequestConferenceFixedGroupMembersEdits)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestConferenceFixedGroupMembersEditsRequest"/> class
    /// with a specified edit type.
    /// </summary>
    /// <param name="editType">The edit type to request.</param>
    public RequestConferenceFixedGroupMembersEditsRequest(ConferenceEditType editType)
        : base(HCIMessageID.RequestConferenceFixedGroupMembersEdits)
    {
        EditType = editType;
    }

    /// <summary>
    /// Generates the payload for the Request Conference / Fixed Group Members Edits message.
    /// </summary>
    /// <returns>The message payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload: EditType(2) + Unused(16) = 18 bytes
        var payload = new byte[18];

        // Edit Type: 2 bytes (big-endian)
        payload[0] = (byte)((ushort)EditType >> 8);
        payload[1] = (byte)((ushort)EditType & 0xFF);

        // Unused: 16 bytes - already initialized to 0

        return payload;
    }
}
