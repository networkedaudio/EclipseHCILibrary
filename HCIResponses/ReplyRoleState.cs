using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Role State (HCIv2) - Message ID 0x0185 (389).
/// This message is used to send the current role status for one or more roles
/// to the host. This message can be requested or sent unsolicited on a state transition.
/// </summary>
public class ReplyRoleState
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// List of role state entries.
    /// </summary>
    public List<RoleStateEntry> Roles { get; set; } = new();

    /// <summary>
    /// Decodes the payload into a ReplyRoleState.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyRoleState Decode(byte[] payload)
    {
        var reply = new ReplyRoleState();

        if (payload.Length < 7)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Count: 2 bytes (big-endian)
        if (offset + 2 > payload.Length)
            return reply;
        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Parse role entries (9 bytes each)
        for (int i = 0; i < count && offset + 9 <= payload.Length; i++)
        {
            // Role Number: 2 bytes (big-endian)
            ushort roleNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Current in use status: 1 byte
            RoleAllocationStatus allocationStatus = (RoleAllocationStatus)payload[offset++];

            // Physical port: 2 bytes (big-endian)
            ushort physicalPort = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Configured Endpoint type: 2 bytes (big-endian)
            ushort configuredEndpointType = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Physical Endpoint type: 2 bytes (big-endian)
            ushort physicalEndpointType = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            reply.Roles.Add(new RoleStateEntry(roleNumber, allocationStatus, 
                physicalPort, configuredEndpointType, physicalEndpointType));
        }

        return reply;
    }
}
