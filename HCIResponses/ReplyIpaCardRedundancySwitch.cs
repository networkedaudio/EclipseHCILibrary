using HCILibrary.Models;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply To Enable or Disable IPA Card Redundancy Switch (HCIv2) - Message ID 0x0183 (387).
/// This message is the reply to the request for enabling or disabling IPA card redundancy switch.
/// </summary>
public class ReplyIpaCardRedundancySwitch
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// List of IPA card switch state entries.
    /// </summary>
    public List<IpaCardSwitchStateEntry> CardSwitchStates { get; set; } = new();

    /// <summary>
    /// Decodes the payload into a ReplyIpaCardRedundancySwitch.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyIpaCardRedundancySwitch Decode(byte[] payload)
    {
        var reply = new ReplyIpaCardRedundancySwitch();

        if (payload.Length < 6)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Number of IPA Cards: 1 byte
        if (offset >= payload.Length)
            return reply;
        byte count = payload[offset++];

        // Switch States: 2 bytes per card (slot number, switch state)
        for (int i = 0; i < count && offset + 2 <= payload.Length; i++)
        {
            byte slotNumber = payload[offset++];
            byte switchState = payload[offset++];

            // Switch state: 1 = disabled, 0 = enabled
            reply.CardSwitchStates.Add(new IpaCardSwitchStateEntry(slotNumber, switchState == 1));
        }

        return reply;
    }
}
