using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request VOX Threshold Status (0x004A).
/// Requests the VOX threshold levels for inputs on the matrix.
/// HCIv2 only.
/// </summary>
public class RequestVoxThresholdStatusRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Gets or sets the protocol schema version. Currently set to 1.
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// Gets or sets the slot number to query.
    /// Use 0xFF to request VOX threshold status for all slots.
    /// </summary>
    public byte Slot { get; set; } = 0xFF;

    /// <summary>
    /// Gets or sets the port number to query.
    /// Use 0xFF to request VOX threshold status for all ports on the specified slot(s).
    /// </summary>
    public byte Port { get; set; } = 0xFF;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestVoxThresholdStatusRequest"/> class
    /// requesting all VOX thresholds (all slots and ports).
    /// </summary>
    public RequestVoxThresholdStatusRequest()
        : base(HCIMessageID.RequestVoxThresholdStatus)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestVoxThresholdStatusRequest"/> class
    /// requesting VOX thresholds for a specific slot.
    /// </summary>
    /// <param name="slot">The slot number to query (0-based), or 0xFF for all slots.</param>
    public RequestVoxThresholdStatusRequest(byte slot)
        : base(HCIMessageID.RequestVoxThresholdStatus)
    {
        Slot = slot;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestVoxThresholdStatusRequest"/> class
    /// requesting VOX thresholds for a specific slot and port.
    /// </summary>
    /// <param name="slot">The slot number to query (0-based), or 0xFF for all slots.</param>
    /// <param name="port">The port number to query (0-based), or 0xFF for all ports.</param>
    public RequestVoxThresholdStatusRequest(byte slot, byte port)
        : base(HCIMessageID.RequestVoxThresholdStatus)
    {
        Slot = slot;
        Port = port;
    }

    /// <summary>
    /// Generates the payload for the Request VOX Threshold Status message.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // [0-3]: HCIv2 Protocol Tag (0xAB 0xBA 0xCE 0xDE)
        // [4]:   Protocol Schema (1 byte)
        // [5]:   Slot (1 byte) - 0xFF for all slots
        // [6]:   Port (1 byte) - 0xFF for all ports

        var payload = new byte[7];

        // HCIv2 protocol tag
        Array.Copy(ProtocolTag, 0, payload, 0, 4);

        // Protocol schema
        payload[4] = ProtocolSchema;

        // Slot
        payload[5] = Slot;

        // Port
        payload[6] = Port;

        return payload;
    }
}
