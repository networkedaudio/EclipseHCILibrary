using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Peripheral Info (0x00F7, Sub Message ID 0x14).
/// Requests software and hardware version information for FreeSpeak 2 Antenna,
/// Splitters, Beltpacks, and other peripherals.
/// HCIv2 only.
/// </summary>
public class RequestPeripheralInfoRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Sub message ID for Peripheral Info request.
    /// </summary>
    public const byte SubMessageId = 0x14;

    /// <summary>
    /// Slot ID value to use for wireless devices.
    /// </summary>
    public const byte WirelessDeviceSlot = 0xFF;

    /// <summary>
    /// Gets or sets the protocol schema version. Currently set to 1.
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// Gets or sets the slot ID of interest.
    /// For wireless devices, set to 0xFF (use <see cref="WirelessDeviceSlot"/>).
    /// </summary>
    public byte SlotId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPeripheralInfoRequest"/> class.
    /// </summary>
    public RequestPeripheralInfoRequest()
        : base(HCIMessageID.RequestPeripheralInfo)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPeripheralInfoRequest"/> class
    /// with the specified slot ID.
    /// </summary>
    /// <param name="slotId">The slot ID of interest. Use 0xFF for wireless devices.</param>
    public RequestPeripheralInfoRequest(byte slotId)
        : base(HCIMessageID.RequestPeripheralInfo)
    {
        SlotId = slotId;
    }

    /// <summary>
    /// Creates a request for wireless device peripheral info.
    /// </summary>
    /// <returns>A new RequestPeripheralInfoRequest configured for wireless devices.</returns>
    public static RequestPeripheralInfoRequest CreateForWirelessDevices()
    {
        return new RequestPeripheralInfoRequest(WirelessDeviceSlot);
    }

    /// <summary>
    /// Generates the payload for the Request Peripheral Info message.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // Protocol Tag: 4 bytes (0xABBACEDE)
        // Protocol Schema: 1 byte
        // Sub Message ID: 1 byte (0x14)
        // Slot ID: 1 byte

        var payload = new byte[7];
        int offset = 0;

        // Protocol Tag (4 bytes)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema (1 byte)
        payload[offset++] = ProtocolSchema;

        // Sub Message ID (1 byte)
        payload[offset++] = SubMessageId;

        // Slot ID (1 byte)
        payload[offset++] = SlotId;

        return payload;
    }
}
