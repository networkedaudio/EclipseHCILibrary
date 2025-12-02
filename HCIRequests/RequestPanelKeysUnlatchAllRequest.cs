using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Panel Keys Unlatch All (HCIv2) - Message ID 0x014E (334).
/// This message is used to request the unlatching of all keys associated with
/// the specified panel.
/// </summary>
public class RequestPanelKeysUnlatchAllRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Card slot number.
    /// </summary>
    public byte CardSlot { get; set; }

    /// <summary>
    /// Zero-based offset into panel card.
    /// </summary>
    public byte PortOffsetOnCard { get; set; }

    /// <summary>
    /// Creates a new Request Panel Keys Unlatch All.
    /// </summary>
    public RequestPanelKeysUnlatchAllRequest()
        : base(HCIMessageID.RequestPanelKeysUnlatchAll)
    {
    }

    /// <summary>
    /// Creates a new Request Panel Keys Unlatch All with the specified parameters.
    /// </summary>
    /// <param name="cardSlot">The card slot number.</param>
    /// <param name="portOffsetOnCard">Zero-based offset into panel card.</param>
    public RequestPanelKeysUnlatchAllRequest(byte cardSlot, byte portOffsetOnCard)
        : base(HCIMessageID.RequestPanelKeysUnlatchAll)
    {
        CardSlot = cardSlot;
        PortOffsetOnCard = portOffsetOnCard;
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
        // Card Slot: 1 byte
        // Port Offset On Card: 1 byte

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // Card Slot: 1 byte
        ms.WriteByte(CardSlot);

        // Port Offset On Card: 1 byte
        ms.WriteByte(PortOffsetOnCard);

        return ms.ToArray();
    }
}
