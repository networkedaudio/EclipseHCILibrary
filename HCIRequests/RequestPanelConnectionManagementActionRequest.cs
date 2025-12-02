using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Panel Connection Management Action (HCIv2) - Message ID 0x018E (398).
/// This message is used to request actions against a matrix panel connection.
/// </summary>
public class RequestPanelConnectionManagementActionRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version (set to 1).
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// The action type to perform.
    /// </summary>
    public PanelConnectionActionType ActionType { get; set; } = PanelConnectionActionType.DisconnectSession;

    /// <summary>
    /// Port number of the physical panel port.
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    /// Creates a new Request Panel Connection Management Action request.
    /// </summary>
    public RequestPanelConnectionManagementActionRequest() 
        : base(HCIMessageID.RequestPanelConnectionManagementAction)
    {
        ExpectedReplyMessageID = HCIMessageID.ReplyPanelConnectionManagementAction;
    }

    /// <summary>
    /// Creates a new Request Panel Connection Management Action request with specified parameters.
    /// </summary>
    /// <param name="actionType">The action type to perform.</param>
    /// <param name="port">The port number of the physical panel port.</param>
    public RequestPanelConnectionManagementActionRequest(PanelConnectionActionType actionType, ushort port) 
        : base(HCIMessageID.RequestPanelConnectionManagementAction)
    {
        ActionType = actionType;
        Port = port;
        ExpectedReplyMessageID = HCIMessageID.ReplyPanelConnectionManagementAction;
    }

    /// <summary>
    /// Creates a request to disconnect a panel session.
    /// </summary>
    /// <param name="port">The port number of the panel to disconnect.</param>
    /// <returns>A configured request to disconnect the panel session.</returns>
    public static RequestPanelConnectionManagementActionRequest DisconnectSession(ushort port)
    {
        return new RequestPanelConnectionManagementActionRequest(PanelConnectionActionType.DisconnectSession, port);
    }

    /// <inheritdoc/>
    protected override byte[] GeneratePayload()
    {
        // Payload: Protocol Tag (4) + Protocol Schema (1) + Action Type (1) + Port (2) = 8 bytes
        var payload = new byte[8];
        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Action Type: 1 byte
        payload[offset++] = (byte)ActionType;

        // Port: 2 bytes (big-endian)
        payload[offset++] = (byte)(Port >> 8);
        payload[offset++] = (byte)(Port & 0xFF);

        return payload;
    }
}
