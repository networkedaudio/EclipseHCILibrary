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
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

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
    /// Handles both cases: payload with or without protocol tag prefix.
    /// </summary>
    /// <param name="payload">The payload bytes.</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyRoleState Decode(byte[] payload)
    {
        var reply = new ReplyRoleState();

        Console.WriteLine($"[ReplyRoleState] RAW PAYLOAD ({payload.Length} bytes): {BitConverter.ToString(payload)}");

        if (payload.Length < 2)
        {
            Console.WriteLine($"[ReplyRoleState] Payload too short: {payload.Length} bytes, need at least 2");
            return reply;
        }

        int offset = 0;

        // Check if payload starts with protocol tag (AB BA CE DE)
        if (payload.Length >= 7 &&
            payload[0] == ProtocolTag[0] &&
            payload[1] == ProtocolTag[1] &&
            payload[2] == ProtocolTag[2] &&
            payload[3] == ProtocolTag[3])
        {
            Console.WriteLine($"[ReplyRoleState] Protocol tag detected, skipping 5 bytes (tag + schema)");
            offset = 4; // Skip protocol tag
            reply.ProtocolSchema = payload[offset++]; // Read schema
        }
        else
        {
            Console.WriteLine($"[ReplyRoleState] No protocol tag, starting at offset 0");
        }

        // Count: 2 bytes (big-endian)
        if (offset + 2 > payload.Length)
        {
            Console.WriteLine($"[ReplyRoleState] Not enough bytes for count at offset {offset}");
            return reply;
        }

        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        Console.WriteLine($"[ReplyRoleState] Parsing {count} role entries");

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

        Console.WriteLine($"[ReplyRoleState] Decoded {reply.Roles.Count} roles");

        return reply;
    }
}
