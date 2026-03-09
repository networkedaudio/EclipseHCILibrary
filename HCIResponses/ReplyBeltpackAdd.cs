using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Beltpack Add result status.
/// </summary>
public enum BeltpackAddResult : byte
{
    /// <summary>
    /// Beltpack was successfully added.
    /// </summary>
    Success = 0,

    /// <summary>
    /// The beltpack registration store is full.
    /// </summary>
    StoreFull = 1,

    /// <summary>
    /// The request was invalid.
    /// </summary>
    InvalidRequest = 2
}

/// <summary>
/// Reply Beltpack Add (HCIv2) - Message ID 0x0194 (404).
/// This message is used to reply to the Request Beltpack Add message.
/// </summary>
public class ReplyBeltpackAdd
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// Serial number of the beltpack (4 bytes).
    /// </summary>
    public uint SerialNumber { get; set; }

    /// <summary>
    /// Reserved byte.
    /// </summary>
    public byte Reserved { get; set; }

    /// <summary>
    /// PMID of the beltpack (3 bytes, stored as uint).
    /// </summary>
    public uint Pmid { get; set; }

    /// <summary>
    /// Result of the add operation.
    /// </summary>
    public BeltpackAddResult Result { get; set; }

    /// <summary>
    /// Gets whether the beltpack was successfully added.
    /// </summary>
    public bool IsSuccess => Result == BeltpackAddResult.Success;

    /// <summary>
    /// Decodes the payload into a ReplyBeltpackAdd.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyBeltpackAdd Decode(byte[] payload)
    {
        var reply = new ReplyBeltpackAdd();

        if (payload.Length < 14)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Serial Number: 4 bytes (big-endian)
        reply.SerialNumber = (uint)((payload[offset] << 24) | (payload[offset + 1] << 16) |
                                    (payload[offset + 2] << 8) | payload[offset + 3]);
        offset += 4;

        // Reserved: 1 byte
        reply.Reserved = payload[offset++];

        // PMID: 3 bytes (big-endian)
        reply.Pmid = (uint)((payload[offset] << 16) | (payload[offset + 1] << 8) | payload[offset + 2]);
        offset += 3;

        // Success: 1 byte (0 = Success, 1 = Store Full, 2 = Invalid Request)
        reply.Result = (BeltpackAddResult)payload[offset];

        return reply;
    }
}
