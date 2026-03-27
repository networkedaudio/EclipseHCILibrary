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
    /// Gets the count of beltpack entries.
    /// </summary>
    public int Count => Beltpacks.Count;

    /// <summary>
    /// Decodes the payload into a ReplyBeltpackInformation.
    /// </summary>
    /// <param name="payload">The payload bytes (after protocol tag and schema have been stripped).</param>
    /// <param name="schema">The protocol schema version from the message header.</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyBeltpackInformation Decode(byte[] payload, byte schema)
    {
        var reply = new ReplyBeltpackInformation
        {   
            ProtocolSchema = schema
        };

        // Output raw payload data
        Console.WriteLine($"[ReplyBeltpackInformation] RAW PAYLOAD ({payload.Length} bytes), Schema={schema}:");
        Console.WriteLine($"  Hex: {BitConverter.ToString(payload)}");

        // Note: The protocol tag (AB BA CE DE) and schema byte have already been
        // stripped by HCIResponse.cs, so the payload starts with the count field.

        if (payload.Length < 2)
        {
            Console.WriteLine($"[ReplyBeltpackInformation] Payload too short: {payload.Length} bytes");
            return reply;
        }

        int offset = 0;

        // Count: 2 bytes (big-endian)
        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        Console.WriteLine($"[ReplyBeltpackInformation] Parsing {count} beltpack entries, payload length={payload.Length}, data offset={offset}");

        // Parse beltpack entries (11 bytes each: serial 4 + pmid 4 + default role 2 + mode 1)
        int entriesParsed = 0;
        for (int i = 0; i < count; i++)
        {
            if (offset + 11 > payload.Length)
            {
                Console.WriteLine($"[ReplyBeltpackInformation] Stopping at entry {i}, not enough bytes. Need {offset + 11}, have {payload.Length}");
                break;
            }

            // Serial number: 4 bytes (big-endian)
            uint serialNumber = (uint)((payload[offset] << 24) | (payload[offset + 1] << 16) | 
                                       (payload[offset + 2] << 8) | payload[offset + 3]);
            offset += 4;

            // PMID: 4 bytes (big-endian)
            Console.WriteLine($"[ReplyBeltpackInformation] PMID bytes at offset {offset}: {payload[offset]:X2}-{payload[offset+1]:X2}-{payload[offset+2]:X2}-{payload[offset+3]:X2}");
            uint pmid = (uint)((payload[offset] << 24) | (payload[offset + 1] << 16) | 
                               (payload[offset + 2] << 8) | payload[offset + 3]);
            Console.WriteLine($"[ReplyBeltpackInformation] Parsed PMID: 0x{pmid:X8} ({pmid})");
            offset += 4;

            // Default Role: 2 bytes (big-endian)
            ushort defaultRole = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Mode: 1 byte
            BeltpackConfigMode mode = (BeltpackConfigMode)payload[offset++];

            reply.Beltpacks.Add(new BeltpackInformationEntry(serialNumber, pmid, defaultRole, mode));
            entriesParsed++;
        }

        Console.WriteLine($"[ReplyBeltpackInformation] Successfully parsed {entriesParsed} of {count} entries");

        return reply;
    }
}
