using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a single proxy key indication entry in the reply.
/// </summary>
public class ProxyKeyIndicationEntry
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
    /// The key number.
    /// </summary>
    public byte Key { get; set; }

    /// <summary>
    /// The colour and brightness flags for the key LED.
    /// </summary>
    public ProxyKeyColour ColourBrightness { get; set; }

    /// <summary>
    /// Gets whether the LED is in bright mode.
    /// </summary>
    public bool IsBright => (ColourBrightness & ProxyKeyColour.Bright) != 0;

    /// <summary>
    /// Gets whether the red LED is enabled.
    /// </summary>
    public bool IsRed => (ColourBrightness & ProxyKeyColour.Red) != 0;

    /// <summary>
    /// Gets whether the green LED is enabled.
    /// </summary>
    public bool IsGreen => (ColourBrightness & ProxyKeyColour.Green) != 0;

    /// <summary>
    /// Gets whether the blue LED is enabled.
    /// </summary>
    public bool IsBlue => (ColourBrightness & ProxyKeyColour.Blue) != 0;

    /// <summary>
    /// The flash rate of the LED.
    /// </summary>
    public ProxyKeyRate Rate { get; set; }

    /// <summary>
    /// Gets whether the LED is off (rate == 0).
    /// </summary>
    public bool IsOff => Rate == ProxyKeyRate.Off;

    /// <summary>
    /// Whether Auto MIC on is enabled.
    /// </summary>
    public bool AutoMicOnEnabled { get; set; }
}

/// <summary>
/// Reply Get Proxy Indication State (HCIv2) - Message ID 0x0137 (311).
/// This message contains the current state of the proxy key indications of one or
/// more panels in the local matrix. These keys are third-party system proxy keys,
/// that is keys that are driven by a third-party system using HCI.
/// </summary>
public class ReplyGetProxyIndicationState
{
    /// <summary>
    /// The schema version from the message.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// The list of proxy key indication entries.
    /// </summary>
    public List<ProxyKeyIndicationEntry> Entries { get; } = new();

    /// <summary>
    /// Decodes a Reply Get Proxy Indication State message from payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes to decode.</param>
    /// <returns>A new ReplyGetProxyIndicationState instance.</returns>
    public static ReplyGetProxyIndicationState Decode(byte[] payload)
    {
        var reply = new ReplyGetProxyIndicationState();

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

        // Each entry is 8 bytes:
        // Port Number (2) + Region (1) + Page (1) + Key (1) + Colour/Brightness (1) + Rate (1) + AutoMicOn (1)
        const int entrySize = 8;

        for (int i = 0; i < count && offset + entrySize <= payload.Length; i++)
        {
            var entry = new ProxyKeyIndicationEntry
            {
                PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]),
                Region = payload[offset + 2],
                Page = payload[offset + 3],
                Key = payload[offset + 4],
                ColourBrightness = (ProxyKeyColour)payload[offset + 5],
                Rate = (ProxyKeyRate)payload[offset + 6],
                AutoMicOnEnabled = payload[offset + 7] != 0
            };

            reply.Entries.Add(entry);
            offset += entrySize;
        }

        return reply;
    }
}
