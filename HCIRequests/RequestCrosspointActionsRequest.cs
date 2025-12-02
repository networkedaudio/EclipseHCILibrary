using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Crosspoint Actions (HCIv2) - Message ID 0x0011.
/// This message is used by the host to change the state of one or more crosspoints in the matrix.
/// </summary>
public class RequestCrosspointActionsRequest : HCIRequest
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
    /// Action type for crosspoint actions (0x0001).
    /// </summary>
    private const ushort ActionTypeCrosspoint = 0x0001;

    /// <summary>
    /// The list of crosspoint actions to request.
    /// </summary>
    public List<CrosspointAction> Actions { get; } = new();

    /// <summary>
    /// Creates a new Request Crosspoint Actions request.
    /// </summary>
    public RequestCrosspointActionsRequest()
        : base(HCIMessageID.RequestConferenceActions) // Same message ID as conference actions (0x0011)
    {
    }

    /// <summary>
    /// Adds a crosspoint action to the request.
    /// </summary>
    /// <param name="isAdd">true to add, false to delete.</param>
    /// <param name="sourcePort">The source port number (0-1023).</param>
    /// <param name="destinationPort">The destination port number (0-1023).</param>
    /// <param name="isInhibit">true to inhibit, false to enable.</param>
    /// <param name="priority">The crosspoint priority.</param>
    public void AddAction(bool isAdd, ushort sourcePort, ushort destinationPort, 
        bool isInhibit = false, CrosspointPriority priority = CrosspointPriority.Normal)
    {
        Actions.Add(new CrosspointAction(isAdd, sourcePort, destinationPort, isInhibit, priority));
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
            // Action Type: 16 bit word (0x0001 for crosspoint)
            payload[offset++] = (byte)(ActionTypeCrosspoint >> 8);
            payload[offset++] = (byte)(ActionTypeCrosspoint & 0xFF);

            // Action: 4 x 16 bit words (8 bytes)
            var actionBytes = action.ToBytes();
            Array.Copy(actionBytes, 0, payload, offset, 8);
            offset += 8;
        }

        return payload;
    }
}
