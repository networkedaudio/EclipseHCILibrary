using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Network Redundancy Endpoint Status (HCIv2) - Message ID 0x0189 (393).
/// This message is used to reply to Request Network Redundancy Endpoint Status message.
/// Contains the network redundancy status for one or more endpoints.
/// </summary>
public class ReplyNetworkRedundancyEndpointStatus
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// List of endpoint network redundancy status entries.
    /// </summary>
    public List<NetworkRedundancyEndpointEntry> Endpoints { get; set; } = new();

    /// <summary>
    /// Decodes the payload into a ReplyNetworkRedundancyEndpointStatus.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyNetworkRedundancyEndpointStatus Decode(byte[] payload)
    {
        var reply = new ReplyNetworkRedundancyEndpointStatus();

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

        // Parse endpoint entries (5 bytes each: port 2 + active lan 1 + switch count 2)
        for (int i = 0; i < count && offset + 5 <= payload.Length; i++)
        {
            // Physical port: 2 bytes (big-endian)
            ushort physicalPort = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Active Lan Port: 1 byte
            ActiveLanPort activeLanPort = (ActiveLanPort)payload[offset++];

            // Switch Count: 2 bytes (big-endian)
            ushort switchCount = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            reply.Endpoints.Add(new NetworkRedundancyEndpointEntry(physicalPort, activeLanPort, switchCount));
        }

        return reply;
    }
}
