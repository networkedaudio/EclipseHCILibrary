using System.Text;
using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Set Proxy Display Data (HCIv2) - Message ID 0x013D (317).
/// This message specifies the display data currently used for the specified
/// third-party proxy display as a result of processing a Set Proxy Display Data Request.
/// These displays are third-party system proxy displays, that is the display data
/// is driven by a third-party system using HCI.
/// </summary>
public class ReplySetProxyDisplayData
{
    /// <summary>
    /// The schema version from the message.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// The list of proxy display data entries with their resultant states.
    /// </summary>
    public List<ProxyDisplayDataEntry> Entries { get; } = new();

    /// <summary>
    /// Decodes a Reply Set Proxy Display Data message from payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes to decode.</param>
    /// <returns>A new ReplySetProxyDisplayData instance.</returns>
    public static ReplySetProxyDisplayData Decode(byte[] payload)
    {
        var reply = new ReplySetProxyDisplayData();

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
