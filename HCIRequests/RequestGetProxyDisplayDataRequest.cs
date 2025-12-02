using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Get Proxy Display Data (HCIv2) - Message ID 0x013A (314).
/// This message requests the display data currently set for the specified
/// third-party proxy displays. These keys are third-party system proxy keys,
/// that is keys that are driven by a third-party system via HCI.
/// Display data is the information that is displayed on the physical display
/// physically associated with the key (e.g., OLED display on V-Series panels).
/// </summary>
public class RequestGetProxyDisplayDataRequest : HCIRequest
{
    /// <summary>
    /// Value representing all panels.
    /// </summary>
    public const ushort AllPanels = 65535;

    /// <summary>
    /// The panel number to request proxy display data for.
    /// Use AllPanels (65535) to request for all panels, otherwise a valid panel number.
    /// </summary>
    public ushort PanelNumber { get; set; } = AllPanels;

    /// <summary>
    /// Creates a new Request Get Proxy Display Data for all panels.
    /// </summary>
    public RequestGetProxyDisplayDataRequest()
        : base(HCIMessageID.RequestGetProxyDisplayData)
    {
    }

    /// <summary>
    /// Creates a new Request Get Proxy Display Data for a specific panel.
    /// </summary>
    /// <param name="panelNumber">The panel number, or AllPanels (65535) for all panels.</param>
    public RequestGetProxyDisplayDataRequest(ushort panelNumber)
        : base(HCIMessageID.RequestGetProxyDisplayData)
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
