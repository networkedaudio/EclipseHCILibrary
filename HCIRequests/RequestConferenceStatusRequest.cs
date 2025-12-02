using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Conference Status (HCIv2) - Message ID 0x0013.
/// Asks the matrix for a list of the users connected to the specified conference.
/// </summary>
public class RequestConferenceStatusRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version.
    /// </summary>
    private const byte ProtocolSchema = 0x01;

    /// <summary>
    /// The conference number to query.
    /// </summary>
    public ushort ConferenceNumber { get; set; }

    /// <summary>
    /// Creates a new Request Conference Status request.
    /// </summary>
    /// <param name="conferenceNumber">The conference number to query.</param>
    public RequestConferenceStatusRequest(ushort conferenceNumber = 0)
        : base(HCIMessageID.RequestConferenceStatus)
    {
        ConferenceNumber = conferenceNumber;
    }

    /// <summary>
    /// Generates the payload for this request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload: ProtocolTag(4) + ProtocolSchema(1) + ConferenceNumber(2) = 7 bytes
        var payload = new byte[7];

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, 0, 4);

        // Protocol Schema: 1 byte
        payload[4] = ProtocolSchema;

        // Conference Number: 16 bit word (big-endian)
        payload[5] = (byte)(ConferenceNumber >> 8);
        payload[6] = (byte)(ConferenceNumber & 0xFF);

        return payload;
    }
}
