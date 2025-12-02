using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Panel Discovery (HCIv2) - Message ID 0x00F7 (247).
/// This message is used to request the discovery of any IP panels connected
/// to the current LAN and populate a panel information cache in the matrix
/// ready for retrieval.
/// </summary>
public class RequestPanelDiscoveryRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Sub ID for Discovery Request.
    /// </summary>
    public const byte SubIdDiscoveryRequest = 0;

    /// <summary>
    /// The Sub ID for the request. Default is 0 (Discovery Request).
    /// </summary>
    public byte SubId { get; set; } = SubIdDiscoveryRequest;

    /// <summary>
    /// Creates a new Request Panel Discovery.
    /// </summary>
    public RequestPanelDiscoveryRequest()
        : base(HCIMessageID.RequestPanelDiscovery)
    {
    }

    /// <summary>
    /// Creates a new Request Panel Discovery with the specified Sub ID.
    /// </summary>
    /// <param name="subId">The Sub ID (default is 0 for Discovery Request).</param>
    public RequestPanelDiscoveryRequest(byte subId)
        : base(HCIMessageID.RequestPanelDiscovery)
    {
        SubId = subId;
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
        // Sub ID: 1 byte (0 = Discovery Request)

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // Sub ID: 1 byte
        ms.WriteByte(SubId);

        return ms.ToArray();
    }
}
