using System.Text;
using HCILibrary.Enums;
using HCILibrary.HCIResponses;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Set Proxy Display Data (HCIv2) - Message ID 0x013C (316).
/// This message sets the display data currently used for the specified third-party
/// proxy displays. Display data is the information shown on the physical display
/// associated with the panel keys (e.g., OLED display on V-Series panels).
/// </summary>
public class RequestSetProxyDisplayDataRequest : HCIRequest
{
    /// <summary>
    /// The list of proxy display data entries to set.
    /// </summary>
    public List<ProxyDisplayDataEntry> Entries { get; } = new();

    /// <summary>
    /// Creates a new Request Set Proxy Display Data with no entries.
    /// </summary>
    public RequestSetProxyDisplayDataRequest()
        : base(HCIMessageID.RequestSetProxyDisplayData)
    {
    }

    /// <summary>
    /// Creates a new Request Set Proxy Display Data with the specified entries.
    /// </summary>
    /// <param name="entries">The proxy display data entries to set.</param>
    public RequestSetProxyDisplayDataRequest(IEnumerable<ProxyDisplayDataEntry> entries)
        : base(HCIMessageID.RequestSetProxyDisplayData)
    {
        Entries.AddRange(entries);
    }

    /// <summary>
    /// Adds a proxy display data entry to the request.
    /// </summary>
    /// <param name="portNumber">The panel port number.</param>
    /// <param name="region">The region number of the key.</param>
    /// <param name="page">The page number of the key.</param>
    /// <param name="associatedKey">The key number of the talk key associated with this display.</param>
    /// <param name="displayText">Primary display text (up to 10 characters).</param>
    /// <param name="altDisplayText">Alternative display text for EHX Alt text feature (up to 10 characters).</param>
    /// <param name="gainLevelIndicator">Gain level (0-15) or 255 to hide gain bar.</param>
    /// <param name="icons">Icons to display.</param>
    /// <returns>This request instance for method chaining.</returns>
    public RequestSetProxyDisplayDataRequest AddEntry(
        ushort portNumber,
        byte region,
        byte page,
        byte associatedKey,
        string displayText,
        string altDisplayText = "",
        byte gainLevelIndicator = 255,
        ProxyDisplayIcons icons = ProxyDisplayIcons.None)
    {
        Entries.Add(new ProxyDisplayDataEntry
        {
            PortNumber = portNumber,
            Region = region,
            Page = page,
            AssociatedKey = associatedKey,
            DisplayText = displayText,
            AltDisplayText = altDisplayText,
            GainLevelIndicator = gainLevelIndicator,
            Icons = icons
        });
        return this;
    }

    /// <summary>
    /// Generates the payload for the request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // Protocol Tag: 4 bytes (0xABBACEDE)
        // Protocol Schema: 1 byte (set to 1)
        // Count: 2 bytes (big-endian)
        // For each entry (48 bytes each):
        //   Port Number: 2 bytes (big-endian)
        //   Region: 1 byte
        //   Page: 1 byte
        //   Key (associated): 1 byte
        //   Display Text: 20 bytes (UTF-16)
        //   Alt Display Text: 20 bytes (UTF-16)
        //   Gain Level Indicator: 1 byte
        //   Icons: 2 bytes (big-endian)

        const int headerSize = 7; // 4 (tag) + 1 (schema) + 2 (count)
        const int entrySize = 48;
        var payload = new byte[headerSize + (Entries.Count * entrySize)];
        int offset = 0;

        // Protocol Tag (HCIv2 marker)
        payload[offset++] = 0xAB;
        payload[offset++] = 0xBA;
        payload[offset++] = 0xCE;
        payload[offset++] = 0xDE;

        // Protocol Schema
        payload[offset++] = 0x01;

        // Count (big-endian)
        ushort count = (ushort)Entries.Count;
        payload[offset++] = (byte)(count >> 8);
        payload[offset++] = (byte)(count & 0xFF);

        // Entries
        foreach (var entry in Entries)
        {
            // Port Number (big-endian)
            payload[offset++] = (byte)(entry.PortNumber >> 8);
            payload[offset++] = (byte)(entry.PortNumber & 0xFF);

            // Region
            payload[offset++] = entry.Region;

            // Page
            payload[offset++] = entry.Page;

            // Key (associated)
            payload[offset++] = entry.AssociatedKey;

            // Display Text (20 bytes, UTF-16)
            EncodeUtf16String(entry.DisplayText, payload, offset, 20);
            offset += 20;

            // Alt Display Text (20 bytes, UTF-16)
            EncodeUtf16String(entry.AltDisplayText, payload, offset, 20);
            offset += 20;

            // Gain Level Indicator
            payload[offset++] = entry.GainLevelIndicator;

            // Icons (big-endian)
            ushort icons = (ushort)entry.Icons;
            payload[offset++] = (byte)(icons >> 8);
            payload[offset++] = (byte)(icons & 0xFF);
        }

        return payload;
    }

    /// <summary>
    /// Encodes a string as UTF-16 into the payload buffer.
    /// </summary>
    private static void EncodeUtf16String(string text, byte[] buffer, int offset, int maxLength)
    {
        if (string.IsNullOrEmpty(text))
        {
            // Fill with zeros
            Array.Clear(buffer, offset, maxLength);
            return;
        }

        // Truncate to max characters (maxLength / 2 since UTF-16 is 2 bytes per char)
        int maxChars = maxLength / 2;
        if (text.Length > maxChars)
        {
            text = text.Substring(0, maxChars);
        }

        byte[] encoded = Encoding.Unicode.GetBytes(text);
        Array.Copy(encoded, 0, buffer, offset, Math.Min(encoded.Length, maxLength));

        // Zero-fill remainder
        if (encoded.Length < maxLength)
        {
            Array.Clear(buffer, offset + encoded.Length, maxLength - encoded.Length);
        }
    }
}
