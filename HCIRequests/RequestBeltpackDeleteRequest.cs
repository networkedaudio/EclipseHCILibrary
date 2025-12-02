using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Beltpack Delete (HCIv2) - Message ID 0x0195 (405).
/// This message is used to request the deletion of a wireless beltpack from the matrix.
/// If the associated beltpack is connected it will be disconnected.
/// </summary>
public class RequestBeltpackDeleteRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version (set to 1).
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// PMID (Physical Module ID) of the beltpack to delete.
    /// Note: Only the lower 3 bytes are used in this message.
    /// </summary>
    public uint Pmid { get; set; }

    /// <summary>
    /// Creates a new Request Beltpack Delete request.
    /// </summary>
    public RequestBeltpackDeleteRequest() 
        : base(HCIMessageID.RequestBeltpackDelete)
    {
        ExpectedReplyMessageID = HCIMessageID.ReplyBeltpackDelete;
    }

    /// <summary>
    /// Creates a new Request Beltpack Delete request with specified PMID.
    /// </summary>
    /// <param name="pmid">The PMID of the beltpack to delete (lower 3 bytes used).</param>
    public RequestBeltpackDeleteRequest(uint pmid) 
        : base(HCIMessageID.RequestBeltpackDelete)
    {
        Pmid = pmid;
        ExpectedReplyMessageID = HCIMessageID.ReplyBeltpackDelete;
    }

    /// <summary>
    /// Creates a request to delete a beltpack by PMID.
    /// </summary>
    /// <param name="pmid">The PMID of the beltpack to delete.</param>
    /// <returns>A configured request to delete the beltpack.</returns>
    public static RequestBeltpackDeleteRequest ByPmid(uint pmid)
    {
        return new RequestBeltpackDeleteRequest(pmid);
    }

    /// <inheritdoc/>
    protected override byte[] GeneratePayload()
    {
        // Payload: Protocol Tag (4) + Protocol Schema (1) + Reserved (0) + PMID (3) = 8 bytes
        var payload = new byte[8];
        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Reserved: 0 bytes (nothing to add)

        // PMID: 3 bytes (big-endian, lower 3 bytes of the 4-byte PMID)
        payload[offset++] = (byte)((Pmid >> 16) & 0xFF);
        payload[offset++] = (byte)((Pmid >> 8) & 0xFF);
        payload[offset++] = (byte)(Pmid & 0xFF);

        return payload;
    }
}
