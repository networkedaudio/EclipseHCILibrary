using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Panel Keys Status (0x00B1).
/// Requests the latch status of keys on a specific panel or role.
/// HCIv2 only.
/// </summary>
public class RequestPanelKeysStatusRequest : HCIRequest
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
    public byte Slot { get; set; }

    /// <summary>
    /// Gets or sets the port offset.
    /// </summary>
    public byte PortOffset { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPanelKeysStatusRequest"/> class.
    /// </summary>
    public RequestPanelKeysStatusRequest()
        : base(HCIMessageID.RequestPanelKeysStatus)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPanelKeysStatusRequest"/> class
    /// with specified slot and port offset.
    /// </summary>
    /// <param name="slot">The card slot number.</param>
    /// <param name="portOffset">The port offset.</param>
    public RequestPanelKeysStatusRequest(byte slot, byte portOffset)
        : base(HCIMessageID.RequestPanelKeysStatus)
    {
        Slot = slot;
        PortOffset = portOffset;
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request Panel Keys Status.
    /// Payload: Protocol Tag (4 bytes) + Protocol Schema (1 byte) + Slot (1 byte) + Port Offset (1 byte).
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

        // Port offset: 1 byte
        ms.WriteByte(PortOffset);

        return ms.ToArray();
    }
}
