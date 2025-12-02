using System.Text;
using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a single proxy display data entry.
/// </summary>
public class ProxyDisplayDataEntry
{
    /// <summary>
    /// The panel port number.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// The region number of the key.
    /// </summary>
    public byte Region { get; set; }

    /// <summary>
    /// The page number of the key.
    /// </summary>
    public byte Page { get; set; }

    /// <summary>
    /// The key number of the talk key associated with this display.
    /// </summary>
    public byte AssociatedKey { get; set; }

    /// <summary>
    /// Primary display text (up to 10 characters, UTF-16 encoded).
    /// </summary>
    public string DisplayText { get; set; } = string.Empty;

    /// <summary>
    /// Alternative display text for use with EHX 'Alt text' feature (up to 10 characters, UTF-16 encoded).
    /// </summary>
    public string AltDisplayText { get; set; } = string.Empty;

    /// <summary>
    /// Gain level indicator value.
    /// 0 = Min gain, 15 = Max gain, intermediate values = intermediate gains, 255 = Hide gain bar.
    /// </summary>
    public byte GainLevelIndicator { get; set; }

    /// <summary>
    /// Gets whether the gain bar should be hidden.
    /// </summary>
    public bool IsGainBarHidden => GainLevelIndicator == 255;

    /// <summary>
    /// Gets the gain level as a percentage (0-100), or null if hidden.
    /// </summary>
    public int? GainLevelPercent => IsGainBarHidden ? null : (int)Math.Round(GainLevelIndicator / 15.0 * 100);

    /// <summary>
    /// Icons to display on the key.
    /// </summary>
    public ProxyDisplayIcons Icons { get; set; }

    /// <summary>
    /// Gets whether the VOX icon is displayed.
    /// </summary>
    public bool HasVoxIcon => (Icons & ProxyDisplayIcons.Vox) != 0;

    /// <summary>
    /// Gets whether the Monitored icon is displayed.
    /// </summary>
    public bool HasMonitoredIcon => (Icons & ProxyDisplayIcons.Monitored) != 0;

    /// <summary>
    /// Gets whether the Telos/SIP icon is displayed.
    /// </summary>
    public bool HasTelosSipIcon => (Icons & ProxyDisplayIcons.TelosSip) != 0;

    /// <summary>
    /// Gets whether the Relay icon is displayed.
    /// </summary>
    public bool HasRelayIcon => (Icons & ProxyDisplayIcons.Relay) != 0;

    /// <summary>
    /// Gets whether the Up Arrow icon is displayed.
    /// </summary>
    public bool HasUpArrowIcon => (Icons & ProxyDisplayIcons.UpArrow) != 0;

    /// <summary>
    /// Gets whether the Down Arrow icon is displayed.
    /// </summary>
    public bool HasDownArrowIcon => (Icons & ProxyDisplayIcons.DownArrow) == ProxyDisplayIcons.DownArrow;
}

/// <summary>
/// Reply Get Proxy Display Data (HCIv2) - Message ID 0x013B (315).
/// This message details the display data currently set for the specified third-party
/// proxy displays. Display data is the information shown on the physical display
/// associated with the key (e.g., OLED display on V-Series panels).
/// </summary>
public class ReplyGetProxyDisplayData
{
    /// <summary>
    /// The schema version from the message.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// The list of proxy display data entries.
    /// </summary>
    public List<ProxyDisplayDataEntry> Entries { get; } = new();

    /// <summary>
    /// Decodes a Reply Get Proxy Display Data message from payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes to decode.</param>
    /// <returns>A new ReplyGetProxyDisplayData instance.</returns>
    public static ReplyGetProxyDisplayData Decode(byte[] payload)
    {
        var reply = new ReplyGetProxyDisplayData();

        if (payload == null || payload.Length < 7)
        {
            return reply;
        }

        int offset = 0;

        // Check for HCIv2 marker (0xAB 0xBA 0xCE 0xDE)
        if (payload.Length >= 4 &&
            payload[0] == 0xAB && payload[1] == 0xBA &&
            payload[2] == 0xCE && payload[3] == 0xDE)
        {
            offset = 4;
        }

        // Protocol Schema (1 byte)
        if (offset < payload.Length)
        {
            reply.Schema = payload[offset++];
        }

        // Count (2 bytes, big-endian)
        if (offset + 2 > payload.Length)
        {
            return reply;
        }

        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Each entry is 48 bytes:
        // Port Number (2) + Region (1) + Page (1) + Key (1) + 
        // Display Text (20) + Alt Display Text (20) + Gain Level (1) + Icons (2)
        const int entrySize = 48;

        for (int i = 0; i < count && offset + entrySize <= payload.Length; i++)
        {
            var entry = new ProxyDisplayDataEntry
            {
                PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]),
                Region = payload[offset + 2],
                Page = payload[offset + 3],
                AssociatedKey = payload[offset + 4]
            };

            // Display Text (20 bytes, UTF-16)
            entry.DisplayText = DecodeUtf16String(payload, offset + 5, 20);

            // Alt Display Text (20 bytes, UTF-16)
            entry.AltDisplayText = DecodeUtf16String(payload, offset + 25, 20);

            // Gain Level Indicator (1 byte)
            entry.GainLevelIndicator = payload[offset + 45];

            // Icons (2 bytes, big-endian)
            entry.Icons = (ProxyDisplayIcons)((payload[offset + 46] << 8) | payload[offset + 47]);

            reply.Entries.Add(entry);
            offset += entrySize;
        }

        return reply;
    }

    /// <summary>
    /// Decodes a UTF-16 string from the payload.
    /// </summary>
    private static string DecodeUtf16String(byte[] payload, int offset, int length)
    {
        if (offset + length > payload.Length)
        {
            return string.Empty;
        }

        // Find null terminator (two zero bytes for UTF-16)
        int actualLength = length;
        for (int i = 0; i < length - 1; i += 2)
        {
            if (payload[offset + i] == 0 && payload[offset + i + 1] == 0)
            {
                actualLength = i;
                break;
            }
        }

        return Encoding.Unicode.GetString(payload, offset, actualLength).TrimEnd('\0');
    }
}
