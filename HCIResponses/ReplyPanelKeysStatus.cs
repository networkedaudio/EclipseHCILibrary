using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents the status of a single key on a panel.
/// </summary>
public class PanelKeyStatus
{
    /// <summary>
    /// Region on this panel.
    /// </summary>
    public byte Region { get; set; }

    /// <summary>
    /// Key on this region.
    /// </summary>
    public byte Key { get; set; }

    /// <summary>
    /// Page on this region.
    /// </summary>
    public byte Page { get; set; }

    /// <summary>
    /// Raw DAK state byte.
    /// </summary>
    public byte RawDakState { get; set; }

    /// <summary>
    /// Key state (bits 0-1): 0 = OFF, 1 = ON.
    /// </summary>
    public KeyState KeyState { get; set; }

    /// <summary>
    /// Listen mode flag (bit 2): Comb Key on istation set to listen mode.
    /// </summary>
    public bool ListenMode { get; set; }

    /// <summary>
    /// Pot position (bits 4-7).
    /// </summary>
    public byte PotState { get; set; }

    /// <summary>
    /// Color index as set in EHX.
    /// </summary>
    public byte Color { get; set; }

    /// <summary>
    /// Parses raw bytes into a PanelKeyStatus object.
    /// </summary>
    /// <param name="payload">The payload bytes.</param>
    /// <param name="offset">The offset to start reading from.</param>
    /// <returns>The parsed PanelKeyStatus, or null if insufficient data.</returns>
    public static PanelKeyStatus? Parse(byte[] payload, int offset)
    {
        // Each key status entry: Region(1) + Key(1) + Page(1) + DakState(1) + Color(1) = 5 bytes
        if (payload.Length < offset + 5)
        {
            return null;
        }

        byte dakState = payload[offset + 3];

        return new PanelKeyStatus
        {
            Region = payload[offset],
            Key = payload[offset + 1],
            Page = payload[offset + 2],
            RawDakState = dakState,
            KeyState = (KeyState)(dakState & 0x03),
            ListenMode = (dakState & 0x04) != 0,
            PotState = (byte)((dakState >> 4) & 0x0F),
            Color = payload[offset + 4]
        };
    }
}

/// <summary>
/// Reply Panel Keys Status (0x00B2).
/// Returns the status of all keys available on the requested panel or role.
/// </summary>
public class ReplyPanelKeysStatus
{
    /// <summary>
    /// Card slot number.
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    /// Port offset from first port of the card.
    /// </summary>
    public byte PortOffset { get; set; }

    /// <summary>
    /// Number of key statuses for this panel.
    /// This includes all key status held even paged (shift page) statuses.
    /// </summary>
    public ushort Count { get; set; }

    /// <summary>
    /// List of key status entries.
    /// </summary>
    public List<PanelKeyStatus> Keys { get; set; } = new();

    /// <summary>
    /// Parses the payload of a Reply Panel Keys Status message.
    /// </summary>
    /// <param name="payload">The message payload (after protocol schema byte).</param>
    /// <returns>The parsed ReplyPanelKeysStatus, or null if parsing fails.</returns>
    public static ReplyPanelKeysStatus? Parse(byte[] payload)
    {
        // Minimum payload: Slot(1) + PortOffset(1) + Count(2) = 4 bytes
        if (payload == null || payload.Length < 4)
        {
            return null;
        }

        var result = new ReplyPanelKeysStatus
        {
            Slot = payload[0],
            PortOffset = payload[1],
            Count = (ushort)((payload[2] << 8) | payload[3])
        };

        // Parse each key status entry (5 bytes each)
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
