using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Beltpack Information (HCIv2) - Message ID 0x0102 (258).
/// This message is used to reply to the Request Beltpack Information message.
/// The matrix always responds with all beltpack records, spanning multiple HCI messages if required.
/// </summary>
public class ReplyBeltpackInformation
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// List of beltpack information entries.
    /// </summary>
    public List<BeltpackInformationEntry> Beltpacks { get; set; } = new();

    /// <summary>
    /// Decodes the payload into a ReplyBeltpackInformation.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyBeltpackInformation Decode(byte[] payload)
    {
        var reply = new ReplyBeltpackInformation();

        if (payload.Length < 7)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Count: 2 bytes (big-endian)
        if (offset + 2 > payload.Length)
            return reply;
        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Parse beltpack entries (11 bytes each: serial 4 + pmid 4 + default role 2 + mode 1)
        for (int i = 0; i < count && offset + 11 <= payload.Length; i++)
        {
            // Serial number: 4 bytes (big-endian)
            uint serialNumber = (uint)((payload[offset] << 24) | (payload[offset + 1] << 16) | 
                                       (payload[offset + 2] << 8) | payload[offset + 3]);
            offset += 4;

            // PMID: 4 bytes (big-endian)
            uint pmid = (uint)((payload[offset] << 24) | (payload[offset + 1] << 16) | 
                               (payload[offset + 2] << 8) | payload[offset + 3]);
            offset += 4;

            // Default Role: 2 bytes (big-endian)
            ushort defaultRole = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Mode: 1 byte
            BeltpackConfigMode mode = (BeltpackConfigMode)payload[offset++];

            reply.Beltpacks.Add(new BeltpackInformationEntry(serialNumber, pmid, defaultRole, mode));
        }

        return reply;
    }
}
