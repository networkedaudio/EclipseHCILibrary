using HCILibrary.Enums;
using HCILibrary.HCIResponses;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Set Proxy Indication State (HCIv2) - Message ID 0x0138 (312).
/// This message sets the current state of the key indication of one or more panel
/// key indications in the local matrix. These keys are 3rd party system proxy keys
/// i.e. keys that are driven by a third party system via HCI.
/// </summary>
public class RequestSetProxyIndicationStateRequest : HCIRequest
{
    /// <summary>
    /// The list of proxy key indication entries to set.
    /// </summary>
    public List<ProxyKeyIndicationEntry> Entries { get; } = new();

    /// <summary>
    /// Creates a new Request Set Proxy Indication State with no entries.
    /// </summary>
    public RequestSetProxyIndicationStateRequest()
        : base(HCIMessageID.RequestSetProxyIndicationState)
    {
    }

    /// <summary>
    /// Creates a new Request Set Proxy Indication State with the specified entries.
    /// </summary>
    /// <param name="entries">The proxy key indication entries to set.</param>
    public RequestSetProxyIndicationStateRequest(IEnumerable<ProxyKeyIndicationEntry> entries)
        : base(HCIMessageID.RequestSetProxyIndicationState)
    {
        Entries.AddRange(entries);
    }

    /// <summary>
    /// Adds a proxy key indication entry to the request.
    /// </summary>
    /// <param name="portNumber">The panel port number.</param>
    /// <param name="region">The region number of the key.</param>
    /// <param name="page">The page number of the key.</param>
    /// <param name="key">The key number.</param>
    /// <param name="colourBrightness">The colour and brightness flags.</param>
    /// <param name="rate">The LED flash rate.</param>
    /// <param name="autoMicOnEnabled">Whether Auto MIC on is enabled.</param>
    /// <returns>This request instance for method chaining.</returns>
    public RequestSetProxyIndicationStateRequest AddEntry(
        ushort portNumber,
        byte region,
        byte page,
        byte key,
        ProxyKeyColour colourBrightness,
        ProxyKeyRate rate,
        bool autoMicOnEnabled = false)
    {
        Entries.Add(new ProxyKeyIndicationEntry
        {
            PortNumber = portNumber,
            Region = region,
            Page = page,
            Key = key,
            ColourBrightness = colourBrightness,
            Rate = rate,
            AutoMicOnEnabled = autoMicOnEnabled
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
        // For each entry (8 bytes each):
        //   Port Number: 2 bytes (big-endian)
        //   Region: 1 byte
        //   Page: 1 byte
        //   Key: 1 byte
        //   Colour/Brightness: 1 byte
        //   Rate: 1 byte
        //   Auto MIC on Enabled: 1 byte

        const int headerSize = 7; // 4 (tag) + 1 (schema) + 2 (count)
        const int entrySize = 8;
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

            // Key
            payload[offset++] = entry.Key;

            // Colour/Brightness
            payload[offset++] = (byte)entry.ColourBrightness;

            // Rate
            payload[offset++] = (byte)entry.Rate;

            // Auto MIC on Enabled
            payload[offset++] = (byte)(entry.AutoMicOnEnabled ? 1 : 0);
        }

        return payload;
    }
}
