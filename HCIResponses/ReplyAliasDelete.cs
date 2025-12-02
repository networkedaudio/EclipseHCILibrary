using HCILibrary.HCIRequests;
using HCILibrary.Models;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Alias Delete (0x0085).
/// Sent in response to Request Alias Delete.
/// Contains the list of alias dialcodes that were processed.
/// </summary>
public class ReplyAliasDelete
{
    /// <summary>
    /// Protocol schema version from the response.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The list of alias dialcodes in the response.
    /// </summary>
    public List<AliasDialcode> Dialcodes { get; } = new();

    /// <summary>
    /// Parses a Reply Alias Delete response from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags, starting at protocol tag).</param>
    /// <returns>The parsed response.</returns>
    public static ReplyAliasDelete Parse(byte[] payload)
    {
        var result = new ReplyAliasDelete();
        int offset = 0;

        // Skip protocol tag (4 bytes) - already validated by caller
        offset += 4;

        // Protocol schema (1 byte)
        result.ProtocolSchema = payload[offset++];

        // Count (16-bit, big-endian)
        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Alias data (4 bytes each)
        for (int i = 0; i < count; i++)
        {
            if (offset + 4 <= payload.Length)
            {
                result.Dialcodes.Add(AliasDialcode.FromBytes(payload, offset));
                offset += 4;
            }
        }

        return result;
    }
}
