using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Beltpack Information (HCIv2) - Message ID 0x0101 (257).
/// This message is used to request all beltpack data records from the matrix.
/// This information record (one per beltpack) includes information on the
/// beltpack hardware, registration and configuration.
/// </summary>
public class RequestBeltpackInformationRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version.
    /// Schema 1: No additional parameters.
    /// Schema 2: Includes request type attribute.
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// Request type (only used when ProtocolSchema is 2).
    /// Specifies which beltpack entries to return.
    /// </summary>
    public BeltpackRequestType? RequestType { get; set; }

    /// <summary>
    /// Creates a new Request Beltpack Information request (schema 1 - all entries).
    /// </summary>
    public RequestBeltpackInformationRequest() 
        : base(HCIMessageID.RequestBeltpackInformation)
    {
        ExpectedReplyMessageID = HCIMessageID.ReplyBeltpackInformation;
    }

    /// <summary>
    /// Creates a new Request Beltpack Information request with a specific request type (schema 2).
    /// </summary>
    /// <param name="requestType">The type of beltpack entries to request.</param>
    public RequestBeltpackInformationRequest(BeltpackRequestType requestType) 
        : base(HCIMessageID.RequestBeltpackInformation)
    {
        ProtocolSchema = 2;
        RequestType = requestType;
        ExpectedReplyMessageID = HCIMessageID.ReplyBeltpackInformation;
    }

    /// <summary>
    /// Creates a request to get all beltpack entries (map or OTA added).
    /// </summary>
    /// <returns>A configured request to get all map/OTA beltpack entries.</returns>
    public static RequestBeltpackInformationRequest AllEntries()
    {
        return new RequestBeltpackInformationRequest();
    }

    /// <summary>
    /// Creates a request to get only map or OTA added beltpack entries.
    /// </summary>
    /// <returns>A configured request to get map/OTA beltpack entries.</returns>
    public static RequestBeltpackInformationRequest MapOrOtaEntries()
    {
        return new RequestBeltpackInformationRequest(BeltpackRequestType.MapOrOtaEntries);
    }

    /// <summary>
    /// Creates a request to get only HCI API added beltpack entries.
    /// </summary>
    /// <returns>A configured request to get HCI added beltpack entries.</returns>
    public static RequestBeltpackInformationRequest HciAddedEntries()
    {
        return new RequestBeltpackInformationRequest(BeltpackRequestType.HciAddedEntries);
    }

    /// <inheritdoc/>
    protected override byte[] GeneratePayload()
    {
        // Schema 1: Protocol Tag (4) + Protocol Schema (1) = 5 bytes
        // Schema 2: Protocol Tag (4) + Protocol Schema (1) + Request Type (1) = 6 bytes
        int payloadSize = ProtocolSchema == 1 ? 5 : 6;
        var payload = new byte[payloadSize];
        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Request Type: 1 byte (only if schema 2)
        if (ProtocolSchema >= 2 && RequestType.HasValue)
        {
            payload[offset++] = (byte)RequestType.Value;
        }

        return payload;
    }
}
