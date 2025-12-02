using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request IP Panel List (HCIv2) - Message ID 0x00F7 (247).
/// This message is used to request the content of the discovered IP panels cache
/// stored in the matrix. This information can be used to display panels
/// discovered on the network, but also to request the configuration /
/// connection of such panels to the matrix.
/// </summary>
public class RequestIPPanelListRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Sub ID for Panel Cache Request.
    /// </summary>
    public const byte SubIdPanelCacheRequest = 8;

    /// <summary>
    /// The Sub ID for the request. Default is 8 (Panel Cache Request).
    /// </summary>
    public byte SubId { get; set; } = SubIdPanelCacheRequest;

    /// <summary>
    /// Creates a new Request IP Panel List.
    /// </summary>
    public RequestIPPanelListRequest()
        : base(HCIMessageID.RequestIPPanelList)
    {
    }

    /// <summary>
    /// Creates a new Request IP Panel List with the specified Sub ID.
    /// </summary>
    /// <param name="subId">The Sub ID (default is 8 for Panel Cache Request).</param>
    public RequestIPPanelListRequest(byte subId)
        : base(HCIMessageID.RequestIPPanelList)
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
        // Sub ID: 1 byte (8 = Panel Cache Request)

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
