namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a single panel key public state entry.
/// </summary>
public class PanelKeyPublicStateEntry
{
    /// <summary>
    /// The panel port number.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// The panel region of the key.
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
    /// The pressed state of the key.
    /// </summary>
    public bool IsPressed { get; set; }
}

/// <summary>
/// Reply Panel Keys Public State (HCIv2) - Message ID 0x0141 (321).
/// This message is either sent in response to a Panel Keys Public State Request
/// or it can be sent out unsolicited if auto updates have been enabled for a panel.
/// </summary>
public class ReplyPanelKeysPublicState
{
    /// <summary>
    /// The schema version from the message.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// The list of panel key public state entries.
    /// </summary>
    public List<PanelKeyPublicStateEntry> Entries { get; } = new();

    /// <summary>
    /// Decodes a Reply Panel Keys Public State message from payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes to decode.</param>
    /// <returns>A new ReplyPanelKeysPublicState instance.</returns>
    public static ReplyPanelKeysPublicState Decode(byte[] payload)
    {
        var reply = new ReplyPanelKeysPublicState();

        if (payload == null || payload.Length < 6)
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

        // Count (1 byte)
        if (offset >= payload.Length)
        {
            return reply;
        }

        byte count = payload[offset++];

        // Each entry is 6 bytes:
        // Port Number (2) + Region (1) + Page (1) + Key (1) + Pressed State (1)
        const int entrySize = 6;

        for (int i = 0; i < count && offset + entrySize <= payload.Length; i++)
        {
            var entry = new PanelKeyPublicStateEntry
            {
                PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]),
                Region = payload[offset + 2],
                Page = payload[offset + 3],
                Key = payload[offset + 4],
                IsPressed = payload[offset + 5] != 0
            };

            reply.Entries.Add(entry);
            offset += entrySize;
        }

        return reply;
    }
}
