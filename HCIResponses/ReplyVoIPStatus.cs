using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a single VoIP channel status entry.
/// </summary>
public class VoIPStatusEntry
{
    /// <summary>
    /// Slot number of the IVC32/E-IPA card (first CPU slot is 'Slot 1').
    /// </summary>
    public byte SlotNumber { get; set; }

    /// <summary>
    /// VoIP channel (port offset into card).
    /// </summary>
    public byte Channel { get; set; }

    /// <summary>
    /// Absolute physical port number assigned in the EHX map for this channel.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Status of the VoIP channel.
    /// </summary>
    public VoIPChannelStatus Status { get; set; }

    /// <summary>
    /// Reason for channel being down. Set to NotSet (27) when channel is not down.
    /// </summary>
    public CallEndReason DownReason { get; set; }

    /// <summary>
    /// Indicates whether call establishment has been blocked due to repeated remote refusals.
    /// </summary>
    public bool IsBlocked { get; set; }
}

/// <summary>
/// Reply VoIP Status (0x00F8, Sub Message ID 0x17).
/// Response to Request VoIP Status, or sent as unsolicited message when VoIP status changes.
/// </summary>
public class ReplyVoIPStatus
{
    /// <summary>
    /// Sub message ID for VoIP status reply.
    /// </summary>
    public const byte SubMessageId = 0x17;

    /// <summary>
    /// Number of VoIP status entries in this message.
    /// </summary>
    public ushort Count { get; set; }

    /// <summary>
    /// List of VoIP status entries.
    /// </summary>
    public List<VoIPStatusEntry> Statuses { get; set; } = new();

    /// <summary>
    /// Parses the payload of a Reply VoIP Status message.
    /// </summary>
    /// <param name="payload">The message payload (after protocol schema byte).</param>
    /// <returns>The parsed ReplyVoIPStatus, or null if parsing fails.</returns>
    public static ReplyVoIPStatus? Parse(byte[] payload)
    {
        // Minimum payload: SubMessageId(1) + Count(2) = 3 bytes
        if (payload == null || payload.Length < 3)
        {
            return null;
        }

        int offset = 0;

        // Sub Message ID: 1 byte (should be 0x17)
        byte subMessageId = payload[offset++];
        if (subMessageId != SubMessageId)
        {
            return null;
        }

        var result = new ReplyVoIPStatus();

        // Count: 2 bytes (big-endian)
        result.Count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Each status entry:
        // SlotNumber(1) + Channel(1) + PortNumber(2) + Status(1) + DownReason(1) + BlockedFlag(1) = 7 bytes
        const int entrySize = 7;

        for (int i = 0; i < result.Count; i++)
        {
            if (payload.Length < offset + entrySize)
            {
                break;
            }

            var entry = new VoIPStatusEntry();

            // Slot Number: 1 byte
            entry.SlotNumber = payload[offset++];

            // Channel: 1 byte
            entry.Channel = payload[offset++];

            // Port Number: 2 bytes (big-endian)
            entry.PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Status: 1 byte
            byte statusValue = payload[offset++];
            entry.Status = Enum.IsDefined(typeof(VoIPChannelStatus), statusValue)
                ? (VoIPChannelStatus)statusValue
                : VoIPChannelStatus.Unknown;

            // Down Reason: 1 byte
            byte reasonValue = payload[offset++];
            entry.DownReason = Enum.IsDefined(typeof(CallEndReason), reasonValue)
                ? (CallEndReason)reasonValue
                : CallEndReason.NotSet;

            // Blocked Flag: 1 byte (0 = True, 1 = False)
            entry.IsBlocked = payload[offset++] == 0;

            result.Statuses.Add(entry);
        }

        return result;
    }
}
