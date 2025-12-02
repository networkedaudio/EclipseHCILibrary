using HCILibrary.Models;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Xpt and Level Status (HCIv2) - Message ID 0x0097 (151).
/// Delivers the source, destination, and level information for crosspoints
/// currently present in the system.
/// This can be requested by the host (all ports only) or is sent unsolicited
/// from the matrix when a crosspoint is made, broken, or level adjusted.
/// </summary>
public class ReplyXptAndLevelStatus
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// List of crosspoint and level entries.
    /// </summary>
    public List<XptAndLevelEntry> Entries { get; set; } = new();

    /// <summary>
    /// Decodes the payload into a ReplyXptAndLevelStatus.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyXptAndLevelStatus Decode(byte[] payload)
    {
        var reply = new ReplyXptAndLevelStatus();

        if (payload.Length < 7)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Count: 2 bytes (big-endian)
        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Parse entries
        // Each entry is 4 bytes:
        //   Byte 0-1: Flags (6 bits) + Port (10 bits)
        //     Bit 15: Monitored
        //     Bit 14: Talk Xpt
        //     Bit 13: Listen Xpt
        //     Bits 12-10: Reserved
        //     Bits 9-0: Port
        //   Byte 2-3: Level (2 bytes, big-endian)
        for (int i = 0; i < count && offset + 4 <= payload.Length; i++)
        {
            ushort flagsAndPort = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            ushort level = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Extract flags and port from the 16-bit value
            bool monitored = (flagsAndPort & 0x8000) != 0;  // Bit 15
            bool talkXpt = (flagsAndPort & 0x4000) != 0;    // Bit 14
            bool listenXpt = (flagsAndPort & 0x2000) != 0;  // Bit 13
            // Bits 12-10 are reserved
            ushort port = (ushort)(flagsAndPort & 0x03FF);  // Bits 9-0

            reply.Entries.Add(new XptAndLevelEntry(monitored, talkXpt, listenXpt, port, level));
        }

        return reply;
    }
}
