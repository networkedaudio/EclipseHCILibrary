using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Represents a single panel key set state entry.
/// </summary>
public class PanelKeySetStateEntry
{
    /// <summary>
    /// The panel port number. Use 65535 for all panels.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// The region number of the key.
    /// </summary>
    public byte Region { get; set; }

    /// <summary>
    /// The key ID.
    /// </summary>
    public byte Key { get; set; }

    /// <summary>
    /// The key event type.
    /// </summary>
    public KeyEvent KeyEvent { get; set; }
}

/// <summary>
/// Request Panel Keys Public Set State (HCIv2) - Message ID 0x0155 (341).
/// This message is used to request the change of state of the keys for a
/// specific panel or all panels.
/// </summary>
public class RequestPanelKeysPublicSetStateRequest : HCIRequest
{
    /// <summary>
    /// Value representing all panels.
    /// </summary>
    public const ushort AllPanels = 65535;

    /// <summary>
    /// The list of panel key set state entries.
    /// </summary>
    public List<PanelKeySetStateEntry> Entries { get; } = new();

    /// <summary>
    /// Creates a new Request Panel Keys Public Set State with no entries.
    /// </summary>
    public RequestPanelKeysPublicSetStateRequest()
        : base(HCIMessageID.RequestPanelKeysPublicSetState)
    {
    }

    /// <summary>
    /// Creates a new Request Panel Keys Public Set State with the specified entries.
    /// </summary>
    /// <param name="entries">The panel key set state entries.</param>
    public RequestPanelKeysPublicSetStateRequest(IEnumerable<PanelKeySetStateEntry> entries)
        : base(HCIMessageID.RequestPanelKeysPublicSetState)
    {
        Entries.AddRange(entries);
    }

    /// <summary>
    /// Adds a panel key set state entry to the request.
    /// </summary>
    /// <param name="portNumber">The panel port number, or AllPanels (65535) for all panels.</param>
    /// <param name="region">The region number of the key.</param>
    /// <param name="key">The key ID.</param>
    /// <param name="keyEvent">The key event type.</param>
    /// <returns>This request instance for method chaining.</returns>
    public RequestPanelKeysPublicSetStateRequest AddEntry(
        ushort portNumber,
        byte region,
        byte key,
        KeyEvent keyEvent)
    {
        Entries.Add(new PanelKeySetStateEntry
        {
            PortNumber = portNumber,
            Region = region,
            Key = key,
            KeyEvent = keyEvent
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
        // Count: 1 byte
        // For each entry (5 bytes each):
        //   Port Number: 2 bytes (big-endian)
        //   Region: 1 byte
        //   Key: 1 byte
        //   Key Event: 1 byte

        const int headerSize = 6; // 4 (tag) + 1 (schema) + 1 (count)
        const int entrySize = 5;
        var payload = new byte[headerSize + (Entries.Count * entrySize)];
        int offset = 0;

        // Protocol Tag (HCIv2 marker)
        payload[offset++] = 0xAB;
        payload[offset++] = 0xBA;
        payload[offset++] = 0xCE;
        payload[offset++] = 0xDE;

        // Protocol Schema
        payload[offset++] = 0x01;

        // Count (1 byte, max 255 entries)
        payload[offset++] = (byte)Math.Min(Entries.Count, 255);

        // Entries
        foreach (var entry in Entries.Take(255))
        {
            // Port Number (big-endian)
            payload[offset++] = (byte)(entry.PortNumber >> 8);
            payload[offset++] = (byte)(entry.PortNumber & 0xFF);

            // Region
            payload[offset++] = entry.Region;

            // Key
            payload[offset++] = entry.Key;

            // Key Event
            payload[offset++] = (byte)entry.KeyEvent;
        }

        return payload;
    }
}
