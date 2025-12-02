using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request System Crosspoint (HCIv2) - Message ID 0x00C9 (201).
/// This message is used to request the state of linked matrices point to point
/// routes (cross-points).
/// 
/// The sources and destinations involved in these messages are user endpoint
/// ports, that is the endpoint to trunk cross-points (intelligent linking hops)
/// of the end to end routing are not included.
/// </summary>
public class RequestSystemCrosspointRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Creates a new Request System Crosspoint.
    /// </summary>
    public RequestSystemCrosspointRequest()
        : base(HCIMessageID.RequestSystemCrosspoint)
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
