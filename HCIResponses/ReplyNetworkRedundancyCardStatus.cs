using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Network Redundancy Card Status (HCIv2) - Message ID 0x018B (395).
/// This message is used to reply to Request Network Redundancy Card Status message.
/// Contains the main/standby state for one or more cards.
/// </summary>
public class ReplyNetworkRedundancyCardStatus
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// List of card network redundancy status entries.
    /// </summary>
    public List<NetworkRedundancyCardEntry> Cards { get; set; } = new();

    /// <summary>
    /// Decodes the payload into a ReplyNetworkRedundancyCardStatus.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyNetworkRedundancyCardStatus Decode(byte[] payload)
    {
        var reply = new ReplyNetworkRedundancyCardStatus();

        if (payload.Length < 6)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Count: 1 byte
        if (offset >= payload.Length)
            return reply;
        byte count = payload[offset++];

        // Parse card entries (7 bytes each: slot 1 + ivc state 1 + ivc count 2 + aoip state 1 + aoip count 2)
        for (int i = 0; i < count && offset + 7 <= payload.Length; i++)
        {
            // Physical slot: 1 byte
            byte physicalSlot = payload[offset++];

            // IVC State: 1 byte
            CardRedundancyState ivcState = (CardRedundancyState)payload[offset++];

            // IVC Switchover Count: 2 bytes (big-endian)
            ushort ivcSwitchoverCount = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // AoIP State: 1 byte
            CardRedundancyState aoipState = (CardRedundancyState)payload[offset++];

            // AoIP Switchover Count: 2 bytes (big-endian)
            ushort aoipSwitchoverCount = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            reply.Cards.Add(new NetworkRedundancyCardEntry(physicalSlot, ivcState, ivcSwitchoverCount,
                aoipState, aoipSwitchoverCount));
        }

        return reply;
    }
}
