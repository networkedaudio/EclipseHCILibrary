using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Trunk Usage Statistics (HCIv2) - Message ID 0x0170 (368).
/// Requests a report on the current trunk statistics held by the matrix.
/// The information contained in the reply is the equivalent of that displayed
/// every 10 minutes in the EHX event log.
/// </summary>
public class RequestTrunkUsageStatisticsRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Creates a new Request Trunk Usage Statistics.
    /// </summary>
    public RequestTrunkUsageStatisticsRequest()
        : base(HCIMessageID.RequestTrunkUsageStatistics)
    {
    }

    /// <summary>
    /// Generates the payload for the request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // Protocol Tag: 4 bytes (0xABBACEDE)
        // Protocol Schema: 1 byte (set to 1)

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        return ms.ToArray();
    }
}
