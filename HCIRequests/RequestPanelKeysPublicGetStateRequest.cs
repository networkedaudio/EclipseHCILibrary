using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Panel Keys Public Get State (HCIv2) - Message ID 0x0140 (320).
/// This message is used to request the current state (latch state) of keys on
/// the specified panel. The response can also be sent unsolicited if auto updates
/// have been enabled for a panel.
/// </summary>
public class RequestPanelKeysPublicGetStateRequest : HCIRequest
{
    /// <summary>
    /// Value representing all panels.
    /// </summary>
    public const ushort AllPanels = 65535;

    /// <summary>
    /// The port number (panel) to request key state for.
    /// Use AllPanels (65535) to request for all panels, otherwise a valid panel number.
    /// </summary>
    public ushort PortNumber { get; set; } = AllPanels;

    /// <summary>
    /// Creates a new Request Panel Keys Public Get State for all panels.
    /// </summary>
    public RequestPanelKeysPublicGetStateRequest()
        : base(HCIMessageID.RequestPanelKeysPublicGetState)
    {
    }

    /// <summary>
    /// Creates a new Request Panel Keys Public Get State for a specific panel.
    /// </summary>
    /// <param name="portNumber">The port number (panel), or AllPanels (65535) for all panels.</param>
    public RequestPanelKeysPublicGetStateRequest(ushort portNumber)
        : base(HCIMessageID.RequestPanelKeysPublicGetState)
    {
        PortNumber = portNumber;
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
        // Port Number: 2 bytes (big-endian)

        var payload = new byte[7];
        int offset = 0;

        // Protocol Tag (HCIv2 marker)
        payload[offset++] = 0xAB;
        payload[offset++] = 0xBA;
        payload[offset++] = 0xCE;
        payload[offset++] = 0xDE;

        // Protocol Schema
        payload[offset++] = 0x01;

        // Port Number (big-endian)
        payload[offset++] = (byte)(PortNumber >> 8);
        payload[offset++] = (byte)(PortNumber & 0xFF);

        return payload;
    }
}
