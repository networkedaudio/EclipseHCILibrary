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
    /// <param name="pmid">The PMID of the beltpack to delete.</param>
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
        Console.WriteLine($"[RequestBeltpackDelete] Generating payload for PMID: 0x{Pmid:X8} ({Pmid})");

        // Payload: Protocol Tag (4) + Protocol Schema (1) + PMID (4) = 9 bytes
        var payload = new byte[9];
        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // PMID: 4 bytes (big-endian, matching the format from ReplyBeltpackInformation)
        payload[offset++] = (byte)((Pmid >> 24) & 0xFF);
        payload[offset++] = (byte)((Pmid >> 16) & 0xFF);
        payload[offset++] = (byte)((Pmid >> 8) & 0xFF);
        payload[offset++] = (byte)(Pmid & 0xFF);

        Console.WriteLine($"[RequestBeltpackDelete] Payload bytes: {BitConverter.ToString(payload)}");

        return payload;
    }
}
