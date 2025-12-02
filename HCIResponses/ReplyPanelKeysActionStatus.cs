namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Panel Keys Action Status (0x00B4).
/// Returns the status of all keys toggled on/off by the Request Panel Keys Action.
/// </summary>
public class ReplyPanelKeysActionStatus
{
    /// <summary>
    /// Card slot number.
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    /// Port offset from first port of the card.
    /// </summary>
    public byte Port { get; set; }

    /// <summary>
    /// Number of key statuses in this reply.
    /// </summary>
    public ushort Count { get; set; }

    /// <summary>
    /// List of key status entries.
    /// </summary>
    public List<PanelKeyStatus> Keys { get; set; } = new();

    /// <summary>
    /// Parses the payload of a Reply Panel Keys Action Status message.
    /// </summary>
    /// <param name="payload">The message payload (after protocol schema byte).</param>
    /// <returns>The parsed ReplyPanelKeysActionStatus, or null if parsing fails.</returns>
    public static ReplyPanelKeysActionStatus? Parse(byte[] payload)
    {
        // Minimum payload: Slot(1) + Port(1) + Count(2) = 4 bytes
        if (payload == null || payload.Length < 4)
        {
            return null;
        }

        var result = new ReplyPanelKeysActionStatus
        {
            Slot = payload[0],
            Port = payload[1],
            Count = (ushort)((payload[2] << 8) | payload[3])
        };

        // Parse each key status entry (5 bytes each: Region + Key + Page + DakState + Color)
        int offset = 4;
        for (int i = 0; i < result.Count; i++)
        {
            var keyStatus = PanelKeyStatus.Parse(payload, offset);
            if (keyStatus == null)
            {
                break;
            }
            result.Keys.Add(keyStatus);
            offset += 5;
        }

        return result;
    }
}
