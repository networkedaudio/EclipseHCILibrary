using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Actions Status (HCIv2) - Message ID 0x000F.
/// Requests the current status of specified action types.
/// </summary>
public class RequestActionsStatusRequest : HCIRequest
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
    /// The action types to request status for.
    /// </summary>
    public ActionType ActionTypes { get; set; }

    /// <summary>
    /// Creates a new Request Actions Status request.
    /// </summary>
    /// <param name="actionTypes">The action types to request status for.</param>
    public RequestActionsStatusRequest(ActionType actionTypes = ActionType.None)
        : base(HCIMessageID.RequestActionsStatus)
    {
        ActionTypes = actionTypes;
    }

    /// <summary>
    /// Generates the payload for this request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload: ProtocolTag(4) + ProtocolSchema(1) + ActionType(2) = 7 bytes
        var payload = new byte[7];

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, 0, 4);

        // Protocol Schema: 1 byte
        payload[4] = ProtocolSchema;

        // Action Type: 16 bit word (big-endian)
        ushort actionTypeValue = (ushort)ActionTypes;
        payload[5] = (byte)(actionTypeValue >> 8);
        payload[6] = (byte)(actionTypeValue & 0xFF);

        return payload;
    }
}
