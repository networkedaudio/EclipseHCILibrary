using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Panel Keys Public Set State (HCIv2) - Message ID 0x0156 (342).
/// This message is used to reply to the Public Set State Request.
/// </summary>
public class ReplyPanelKeysPublicSetState
{
    /// <summary>
    /// Protocol schema version (should be 1).
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// List of key set state results.
    /// </summary>
    public List<PanelKeySetStateResult> Results { get; set; } = new();

    /// <summary>
    /// Represents the result of a key set state operation.
    /// </summary>
    public class PanelKeySetStateResult
    {
        /// <summary>
        /// Port number. 65535 (0xFFFF) indicates all panels.
        /// </summary>
        public ushort PortNumber { get; set; }

        /// <summary>
        /// Region number of the key.
        /// </summary>
        public byte Region { get; set; }

        /// <summary>
        /// Key ID within the region.
        /// </summary>
        public byte KeyId { get; set; }

        /// <summary>
        /// The key event that was applied.
        /// </summary>
        public KeyEvent KeyEvent { get; set; }

        /// <summary>
        /// Indicates if the port number represents all panels.
        /// </summary>
        public bool IsAllPanels => PortNumber == 0xFFFF;
    }

    /// <summary>
    /// Decodes a Reply Panel Keys Public Set State message from the payload.
    /// </summary>
    /// <param name="payload">The message payload (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyPanelKeysPublicSetState Decode(byte[] payload)
    {
        var reply = new ReplyPanelKeysPublicSetState();

        if (payload.Length < 6)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip validation, already checked
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Count: 1 byte
        byte count = payload[offset++];

        // Parse each entry (5 bytes each: 2 port + 1 region + 1 key + 1 event)
        for (int i = 0; i < count && offset + 5 <= payload.Length; i++)
        {
            var result = new PanelKeySetStateResult
            {
                // Port Number: 2 bytes (big-endian)
                PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]),
                // Region: 1 byte
                Region = payload[offset + 2],
                // Key ID: 1 byte
                KeyId = payload[offset + 3],
                // Key Event: 1 byte
                KeyEvent = (KeyEvent)payload[offset + 4]
            };

            reply.Results.Add(result);
            offset += 5;
        }

        return reply;
    }
}
