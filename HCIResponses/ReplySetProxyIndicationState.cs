using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Set Proxy Indication State (HCIv2) - Message ID 0x0139 (313).
/// This message details the resultant state of a third-party proxy key
/// indication after a Set Proxy Keys State Request. These keys are third-party
/// system proxy keys, that is keys that are driven by a third party system using HCI.
/// </summary>
public class ReplySetProxyIndicationState
{
    /// <summary>
    /// The schema version from the message.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// The list of proxy key indication entries with their resultant states.
    /// </summary>
    public List<ProxyKeyIndicationEntry> Entries { get; } = new();

    /// <summary>
    /// Decodes a Reply Set Proxy Indication State message from payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes to decode.</param>
    /// <returns>A new ReplySetProxyIndicationState instance.</returns>
    public static ReplySetProxyIndicationState Decode(byte[] payload)
    {
        var reply = new ReplySetProxyIndicationState();

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
