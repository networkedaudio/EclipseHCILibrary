using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Role State Set (HCIv2) - Message ID 0x0187 (391).
/// This message is used to reply to Request Role State Set message.
/// </summary>
public class ReplyRoleStateSet
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// Role number.
    /// </summary>
    public ushort Role { get; set; }

    /// <summary>
    /// The requested allocation status that was sent in the request.
    /// Used to determine the failure reason.
    /// </summary>
    public RoleAllocationStatus RequestedStatus { get; set; }

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
    /// Gets a human-readable error message explaining why the operation failed.
    /// </summary>
    public string GetErrorMessage()
    {
        if (Success)
            return string.Empty;

        // Determine the likely reason for failure based on the current state vs requested state
        if (RequestedStatus == RoleAllocationStatus.Free && CurrentStatus != RoleAllocationStatus.Free)
        {
            return $"Cannot free role {Role}: The role is currently in use (allocated to port {PhysicalPort}). " +
                   "A device may be actively using this role. Disconnect the device first or reassign it to a different role.";
        }

        if (RequestedStatus == RoleAllocationStatus.AllocatedFromPool && CurrentStatus == RoleAllocationStatus.Free)
        {
            return $"Cannot allocate role {Role}: The role could not be allocated to the specified port. " +
                   "The port may not exist or may already be in use by another role.";
        }

        if (RequestedStatus == RoleAllocationStatus.AllocatedFromPool && CurrentStatus == RoleAllocationStatus.AllocatedFromPool)
        {
            return $"Cannot reallocate role {Role}: The role is already allocated to port {PhysicalPort}. " +
                   "Free the role first before reassigning it to a different port.";
        }

        if (CurrentStatus == RoleAllocationStatus.AllocatedFixed)
        {
            return $"Cannot modify role {Role}: The role has a fixed allocation and cannot be changed via HCI.";
        }

        return $"Failed to update role {Role}. The matrix rejected the operation.";
    }

    /// <summary>
    /// Decodes the payload into a ReplyRoleStateSet.
    /// Handles both cases: payload with or without protocol tag prefix.
    /// </summary>
    /// <param name="payload">The payload bytes.</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyRoleStateSet Decode(byte[] payload)
    {
        var reply = new ReplyRoleStateSet();

        Console.WriteLine($"[ReplyRoleStateSet] RAW PAYLOAD ({payload.Length} bytes): {BitConverter.ToString(payload)}");

        if (payload.Length < 6)
        {
            Console.WriteLine($"[ReplyRoleStateSet] Payload too short: {payload.Length} bytes, need at least 6");
            return reply;
        }

        int offset = 0;

        // Check if payload starts with protocol tag (AB BA CE DE)
        // Some messages include it, some don't depending on how HCIResponse.cs processed it
        if (payload.Length >= 11 &&
            payload[0] == ProtocolTag[0] &&
            payload[1] == ProtocolTag[1] &&
            payload[2] == ProtocolTag[2] &&
            payload[3] == ProtocolTag[3])
        {
            Console.WriteLine($"[ReplyRoleStateSet] Protocol tag detected, skipping 5 bytes (tag + schema)");
            offset = 4; // Skip protocol tag
            reply.ProtocolSchema = payload[offset++]; // Read schema
        }
        else
        {
            Console.WriteLine($"[ReplyRoleStateSet] No protocol tag, starting at offset 0");
        }

        // Payload structure:
        // Role: 2 bytes (big-endian)
        // Current Status: 1 byte
        // Physical Port: 2 bytes (big-endian)
        // Success: 1 byte

        if (offset + 6 > payload.Length)
        {
            Console.WriteLine($"[ReplyRoleStateSet] Not enough bytes after offset {offset}");
            return reply;
        }

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

        Console.WriteLine($"[ReplyRoleStateSet] Decoded: Role={reply.Role}, Status={reply.CurrentStatus}, Port={reply.PhysicalPort}, Success={reply.Success}");

        return reply;
    }
}
