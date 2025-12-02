using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Assigned Keys (With Labels) (HCIv2) - Message ID 0x017D (381).
/// Returns the configuration of all assigned keys for selected panel with labels.
/// These assignments are the net result of the map configuration downloaded
/// baseline plus any HCI, online, panel-based assignments on top of this.
/// </summary>
public class ReplyAssignedKeysWithLabels
{
    /// <summary>
    /// Protocol schema version (1 or 2 supported).
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// Card slot number.
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    /// Port offset from first port of the card.
    /// </summary>
    public byte Port { get; set; }

    /// <summary>
    /// Endpoint type (only present if schema 2 was requested).
    /// Used when slot and offset target a role.
    /// </summary>
    public ushort EndpointType { get; set; }

    /// <summary>
    /// List of assigned key entries with labels.
    /// </summary>
    public List<AssignedKeyWithLabelEntry> Keys { get; set; } = new();

    /// <summary>
    /// Decodes the payload into a ReplyAssignedKeysWithLabels.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyAssignedKeysWithLabels Decode(byte[] payload)
    {
        var reply = new ReplyAssignedKeysWithLabels();

        if (payload.Length < 8)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Slot: 1 byte
        reply.Slot = payload[offset++];

        // Port: 1 byte
        reply.Port = payload[offset++];

        // Endpoint Type: 2 bytes (only if schema 2)
        if (reply.ProtocolSchema >= 2)
        {
            if (offset + 2 > payload.Length)
                return reply;
            reply.EndpointType = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Count: 1 byte
        if (offset >= payload.Length)
            return reply;
        byte count = payload[offset++];

        // Parse key entries
        for (int i = 0; i < count; i++)
        {
            var entry = new AssignedKeyWithLabelEntry();

            // Region: 1 byte
            if (offset >= payload.Length)
                break;
            entry.Region = payload[offset++];

            // Key: 1 byte
            if (offset >= payload.Length)
                break;
            entry.Key = payload[offset++];

            // Page: 1 byte
            if (offset >= payload.Length)
                break;
            entry.Page = payload[offset++];

            // Entity: 2 bytes (big-endian)
            if (offset + 2 > payload.Length)
                break;
            entry.Entity = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Key Status: 1 byte
            if (offset >= payload.Length)
                break;
            byte keyStatus = payload[offset++];
            entry.KeyState = (byte)((keyStatus >> 6) & 0x03);
            entry.ListenMode = (keyStatus & 0x20) != 0;
            entry.PotAssigned = (keyStatus & 0x10) != 0;
            entry.PotState = (byte)(keyStatus & 0x0F);

            // Pot Number: 2 bytes (big-endian)
            if (offset + 2 > payload.Length)
                break;
            entry.PotNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Key Operation: 4 bytes
            if (offset + 4 > payload.Length)
                break;

            byte opByte0 = payload[offset++];
            byte opByte1 = payload[offset++];
            byte opByte2 = payload[offset++];
            byte opByte3 = payload[offset++];

            // Byte 0: bits for various flags
            entry.LatchMode = (LatchMode)(opByte0 & 0x0F);
            entry.Group = (byte)((opByte0 >> 4) & 0x0F);

            // Byte 1: more flags
            entry.Unpaged = (opByte1 & 0x01) != 0;      // Bit 8 (bit 0 of byte 1)
            entry.TextMode = (opByte0 & 0x80) != 0;     // Bit 7
            entry.Dual = (opByte0 & 0x40) != 0;         // Bit 6
            entry.Dial = (opByte0 & 0x20) != 0;         // Bit 5

            // Byte 2
            entry.Deactivating = (opByte2 & 0x08) != 0; // Bit 3
            entry.MakeBreak = (opByte2 & 0x04) != 0;    // Bit 2
            entry.CrossPage = (opByte2 & 0x02) != 0;    // Bit 1
            entry.CmapsiSp1 = (opByte2 & 0x01) != 0;    // Bit 0

            // Byte 3
            entry.CmapsiSp2 = (opByte3 & 0x80) != 0;    // Bit 7
            entry.RegionValue = (byte)((opByte3 >> 4) & 0x07); // Bits 4-6
            entry.StackedGroup = (opByte3 & 0x08) != 0; // Bit 3

            // Page Value: 1 byte
            if (offset >= payload.Length)
                break;
            entry.PageValue = payload[offset++];

            // Key Config: 26 bytes
            if (offset + 26 > payload.Length)
                break;
            entry.KeyConfig = new byte[26];
            Array.Copy(payload, offset, entry.KeyConfig, 0, 26);
            offset += 26;

            // System Number is first byte of key config
            entry.SystemNumber = entry.KeyConfig[0];

            // Label: variable length, null-terminated or fixed size
            // Read until null or end of entry
            // Assuming label follows key config - read remaining bytes for this entry
            int labelStart = offset;
            while (offset < payload.Length && payload[offset] != 0)
            {
                offset++;
            }
            if (offset > labelStart)
            {
                entry.Label = System.Text.Encoding.UTF8.GetString(payload, labelStart, offset - labelStart);
            }
            // Skip null terminator if present
            if (offset < payload.Length && payload[offset] == 0)
            {
                offset++;
            }

            reply.Keys.Add(entry);
        }

        return reply;
    }
}
