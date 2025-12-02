using HCILibrary.Enums;

namespace HCILibrary.Models;

/// <summary>
/// Request Conference Actions (HCIv2) - Message ID 0x0011.
/// This message is used to connect or disconnect specified ports to specified conferences.
/// </summary>
public class RequestConferenceActionsRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version.
    /// </summary>
    private const byte ProtocolSchema = 0x01;

    /// <summary>
    /// Action type for conference actions (0x0020).
    /// </summary>
    private const ushort ActionTypeConference = 0x0020;

    /// <summary>
    /// The list of conference actions to request.
    /// </summary>
    public List<ConferenceAction> Actions { get; } = new();

    /// <summary>
    /// Creates a new Request Conference Actions request.
    /// </summary>
    public RequestConferenceActionsRequest()
        : base(HCIMessageID.RequestConferenceActions)
    {
    }

    /// <summary>
    /// Adds a conference action to the request.
    /// </summary>
    /// <param name="isAdd">true to add, false to delete.</param>
    /// <param name="portNumber">The port number (0-1023).</param>
    /// <param name="isTalk">true for talk, false for listen.</param>
    /// <param name="conferenceNumber">The conference number (0-1023).</param>
    public void AddAction(bool isAdd, ushort portNumber, bool isTalk, ushort conferenceNumber)
    {
        Actions.Add(new ConferenceAction(isAdd, portNumber, isTalk, conferenceNumber));
    }

    /// <summary>
    /// Generates the payload for this request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // ProtocolTag(4) + ProtocolSchema(1) + Count(2) + [ActionType(2) + ActionData(8)] * Count
        int actionDataSize = Actions.Count * 10; // Each action: ActionType(2) + ActionData(8)
        var payload = new byte[4 + 1 + 2 + actionDataSize];

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Count: 16 bit word (big-endian)
        ushort count = (ushort)Actions.Count;
        payload[offset++] = (byte)(count >> 8);
        payload[offset++] = (byte)(count & 0xFF);

        // Action Data for each action
        foreach (var action in Actions)
        {
            // Action Type: 16 bit word (0x0020 for conference)
            payload[offset++] = (byte)(ActionTypeConference >> 8);
            payload[offset++] = (byte)(ActionTypeConference & 0xFF);

            // Action: 4 x 16 bit words (8 bytes)
            var actionBytes = action.ToBytes();
            Array.Copy(actionBytes, 0, payload, offset, 8);
            offset += 8;
        }

        return payload;
    }
}
