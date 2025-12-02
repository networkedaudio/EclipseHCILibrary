using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Panel Connection Management Action (HCIv2) - Message ID 0x018F (399).
/// This message is used to reply to the Request Panel Connection Management Action message.
/// </summary>
public class ReplyPanelConnectionManagementAction
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The action type that was performed.
    /// Action 1 (Disconnect) is the only currently supported action.
    /// </summary>
    public PanelConnectionActionType ActionType { get; set; }

    /// <summary>
    /// Port number from the request.
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    /// Whether the action was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Decodes the payload into a ReplyPanelConnectionManagementAction.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyPanelConnectionManagementAction Decode(byte[] payload)
    {
        var reply = new ReplyPanelConnectionManagementAction();

        if (payload.Length < 9)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Action Type: 1 byte
        reply.ActionType = (PanelConnectionActionType)payload[offset++];

        // Port: 2 bytes (big-endian)
        reply.Port = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Success: 1 byte (1 = Success, 0 = Failure)
        reply.Success = payload[offset] == 1;

        return reply;
    }
}
