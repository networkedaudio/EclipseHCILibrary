using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Role State Set (HCIv2) - Message ID 0x0186 (390).
/// This message is used to request a change to the current role assignment state.
/// </summary>
public class RequestRoleStateSetRequest : HCIRequest
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
    /// Role number to modify.
    /// </summary>
    public ushort Role { get; set; }

    /// <summary>
    /// New allocation status for the role.
    /// 0 = Free, 1 = Allocated From Pool.
    /// </summary>
    public RoleAllocationStatus NewStatus { get; set; }

    /// <summary>
    /// Port number of hardware to which the role should be allocated.
    /// Set to 0xFFFF if not in use (e.g., when freeing the role).
    /// </summary>
    public ushort PhysicalPort { get; set; } = 0xFFFF;

    /// <summary>
    /// Creates a new Request Role State Set request.
    /// </summary>
    public RequestRoleStateSetRequest() : base(HCIMessageID.RequestRoleStateSet)
    {
        ExpectedReplyMessageID = HCIMessageID.ReplyRoleState;
    }

    /// <summary>
    /// Creates a new Request Role State Set request with specified parameters.
    /// </summary>
    /// <param name="role">The role number to modify.</param>
    /// <param name="newStatus">The new allocation status.</param>
    /// <param name="physicalPort">The physical port number (0xFFFF if not applicable).</param>
    public RequestRoleStateSetRequest(ushort role, RoleAllocationStatus newStatus, ushort physicalPort = 0xFFFF)
        : base(HCIMessageID.RequestRoleStateSet)
    {
        Role = role;
        NewStatus = newStatus;
        PhysicalPort = physicalPort;
        ExpectedReplyMessageID = HCIMessageID.ReplyRoleState;
    }

    /// <summary>
    /// Creates a request to free a role.
    /// </summary>
    /// <param name="role">The role number to free.</param>
    /// <returns>A configured request to free the specified role.</returns>
    public static RequestRoleStateSetRequest FreeRole(ushort role)
    {
        return new RequestRoleStateSetRequest(role, RoleAllocationStatus.Free, 0xFFFF);
    }

    /// <summary>
    /// Creates a request to allocate a role from the pool to a physical port.
    /// </summary>
    /// <param name="role">The role number to allocate.</param>
    /// <param name="physicalPort">The physical port to allocate to.</param>
    /// <returns>A configured request to allocate the specified role.</returns>
    public static RequestRoleStateSetRequest AllocateFromPool(ushort role, ushort physicalPort)
    {
        return new RequestRoleStateSetRequest(role, RoleAllocationStatus.AllocatedFromPool, physicalPort);
    }

    /// <inheritdoc/>
    protected override byte[] GeneratePayload()
    {
        // Payload: Protocol Tag (4) + Protocol Schema (1) + Role (2) + New Status (1) + Physical Port (2) = 10 bytes
        var payload = new byte[10];
        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Role: 2 bytes (big-endian)
        payload[offset++] = (byte)(Role >> 8);
        payload[offset++] = (byte)(Role & 0xFF);

        // New Status: 1 byte
        payload[offset++] = (byte)NewStatus;

        // Physical Port: 2 bytes (big-endian)
        payload[offset++] = (byte)(PhysicalPort >> 8);
        payload[offset++] = (byte)(PhysicalPort & 0xFF);

        return payload;
    }
}
