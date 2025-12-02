using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Card Info (0x00C3).
/// Retrieves the card information at a specified slot together with its health status.
/// HCIv2 only.
/// </summary>
public class RequestCardInfoRequest : HCIRequest
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
    /// Gets or sets the slot number.
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestCardInfoRequest"/> class.
    /// </summary>
    public RequestCardInfoRequest()
        : base(HCIMessageID.RequestCardInfo)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestCardInfoRequest"/> class
    /// with specified slot.
    /// </summary>
    /// <param name="slot">The slot number.</param>
    public RequestCardInfoRequest(byte slot)
        : base(HCIMessageID.RequestCardInfo)
    {
        Slot = slot;
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request Card Info.
    /// Payload: Protocol Tag (4 bytes) + Protocol Schema (1 byte) + Slot (1 byte).
    /// </summary>
    /// <returns>The payload byte array.</returns>
    protected override byte[] GeneratePayload()
    {
        using var ms = new MemoryStream();

        // HCIv2 protocol tag
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol schema
        ms.WriteByte(ProtocolSchema);

        // Slot: 1 byte
        ms.WriteByte(Slot);

        return ms.ToArray();
    }
}
