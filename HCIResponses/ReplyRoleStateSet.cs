using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Role State Set (HCIv2) - Message ID 0x0187 (391).
/// This message is used to reply to Request Role State Set message.
/// </summary>
public class ReplyRoleStateSet
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// Role number.
    /// </summary>
    public ushort Role { get; set; }

    /// <summary>
    /// Current allocation status of the role.
    /// 0 = Free, 1 = Allocated From Pool.
    /// </summary>
    public RoleAllocationStatus CurrentStatus { get; set; }

    /// <summary>
    /// Port number of hardware to which the current role is allocated.
    /// Set to 0xFFFF if not in use.
    /// </summary>
    public ushort PhysicalPort { get; set; }

    /// <summary>
    /// Whether the role state set operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets whether the physical port is valid (not 0xFFFF).
    /// </summary>
    public bool HasPhysicalPort => PhysicalPort != 0xFFFF;

    /// <summary>
    /// Gets whether the role is currently allocated.
    /// </summary>
    public bool IsAllocated => CurrentStatus != RoleAllocationStatus.Free;

    /// <summary>
    /// Decodes the payload into a ReplyRoleStateSet.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyRoleStateSet Decode(byte[] payload)
    {
        var reply = new ReplyRoleStateSet();

        if (payload.Length < 11)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Role: 2 bytes (big-endian)
        reply.Role = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Current in use status: 1 byte
        reply.CurrentStatus = (RoleAllocationStatus)payload[offset++];

        // Physical port: 2 bytes (big-endian)
        reply.PhysicalPort = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Success: 1 byte (1 = Success, 0 = Failure)
        reply.Success = payload[offset] == 1;

        return reply;
    }
}
