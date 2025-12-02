using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Get Proxy Indication State (HCIv2) - Message ID 0x0136 (310).
/// This message is used to request the state of the proxy key LED indications for the
/// specified panel, or all panels. These keys are 3rd party system proxy keys i.e.
/// keys that are driven by a third party system via HCI.
/// </summary>
public class RequestGetProxyIndicationStateRequest : HCIRequest
{
    /// <summary>
    /// Value representing all panels.
    /// </summary>
    public const ushort AllPanels = 65535;

    /// <summary>
    /// The panel number to request proxy indication state for.
    /// Use AllPanels (65535) to request for all panels, otherwise a valid panel number.
    /// </summary>
    public ushort PanelNumber { get; set; } = AllPanels;

    /// <summary>
    /// Creates a new Request Get Proxy Indication State for all panels.
    /// </summary>
    public RequestGetProxyIndicationStateRequest()
        : base(HCIMessageID.RequestGetProxyIndicationState)
    {
    }

    /// <summary>
    /// Creates a new Request Get Proxy Indication State for a specific panel.
    /// </summary>
    /// <param name="panelNumber">The panel number, or AllPanels (65535) for all panels.</param>
    public RequestGetProxyIndicationStateRequest(ushort panelNumber)
        : base(HCIMessageID.RequestGetProxyIndicationState)
    {
        PanelNumber = panelNumber;
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
        // Panel Number: 2 bytes (big-endian)

        var payload = new byte[7];
        int offset = 0;

        // Protocol Tag (HCIv2 marker)
        payload[offset++] = 0xAB;
        payload[offset++] = 0xBA;
        payload[offset++] = 0xCE;
        payload[offset++] = 0xDE;

        // Protocol Schema
        payload[offset++] = 0x01;

        // Panel Number (big-endian)
        payload[offset++] = (byte)(PanelNumber >> 8);
        payload[offset++] = (byte)(PanelNumber & 0xFF);

        return payload;
    }
}
