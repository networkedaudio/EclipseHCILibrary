using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Port Info (0x00B7).
/// Requests the connected port type (panel, interface, trunk, FreeSpeak BP)
/// and additional port information such as firmware and port settings per card.
/// HCIv2 only.
/// </summary>
public class RequestPortInfoRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version. Set to 1; future payload changes will increment this.
    /// </summary>
    private const byte ProtocolSchema = 0x01;

    /// <summary>
    /// Gets or sets the card slot number.
    /// </summary>
    public int SlotNumber { get; set; }

    /// <summary>
    /// Gets or sets the port offset within the card (0-based).
    /// An offset of 0 requests information for all ports on the card.
    /// </summary>
    public int PortOffset { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPortInfoRequest"/> class.
    /// </summary>
    public RequestPortInfoRequest()
        : base(HCIMessageID.RequestPortInfo)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPortInfoRequest"/> class
    /// with specified slot number. The port offset defaults to 0 (all ports on the card).
    /// </summary>
    /// <param name="slotNumber">The card slot number.</param>
    public RequestPortInfoRequest(int slotNumber)
        : base(HCIMessageID.RequestPortInfo)
    {
        SlotNumber = slotNumber;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPortInfoRequest"/> class
    /// with specified slot number and port offset.
    /// </summary>
    /// <param name="slotNumber">The card slot number.</param>
    /// <param name="portOffset">The port offset within the card (0-based). Use 0 for all ports.</param>
    public RequestPortInfoRequest(int slotNumber, int portOffset)
        : base(HCIMessageID.RequestPortInfo)
    {
        SlotNumber = slotNumber;
        PortOffset = portOffset;
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request Port Info.
    /// Payload: Protocol Tag (4 bytes) + Protocol Schema (1 byte) + Slot Number (2 bytes, big-endian).
    /// </summary>
    /// <returns>The payload byte array.</returns>
    protected override byte[] GeneratePayload()
    {
        using var ms = new MemoryStream();

        // HCIv2 protocol tag
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol schema
        ms.WriteByte(ProtocolSchema);

        // Slot Number: 16 bit word (big-endian) - original on-wire format
        ms.WriteByte((byte)(SlotNumber >> 8));
        ms.WriteByte((byte)(SlotNumber & 0xFF));

        return ms.ToArray();
    }
}
