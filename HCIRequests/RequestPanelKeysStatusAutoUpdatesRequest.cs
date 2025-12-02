using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Panel Keys Status Auto Updates (HCIv2) - Message ID 0x013E (318).
/// This message is used to request the unsolicited transmission of Panel Key
/// Status Reply messages when a key status updates.
/// </summary>
public class RequestPanelKeysStatusAutoUpdatesRequest : HCIRequest
{
    /// <summary>
    /// The first port number to apply the setting to.
    /// </summary>
    public ushort PortNumberStart { get; set; }

    /// <summary>
    /// The last port number to apply the setting to.
    /// </summary>
    public ushort PortNumberEnd { get; set; }

    /// <summary>
    /// Whether auto updates are enabled for the specified port range.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Creates a new Request Panel Keys Status Auto Updates with default values.
    /// </summary>
    public RequestPanelKeysStatusAutoUpdatesRequest()
        : base(HCIMessageID.RequestPanelKeysStatusAutoUpdates)
    {
    }

    /// <summary>
    /// Creates a new Request Panel Keys Status Auto Updates for a single port.
    /// </summary>
    /// <param name="portNumber">The port number to apply the setting to.</param>
    /// <param name="enabled">Whether auto updates are enabled.</param>
    public RequestPanelKeysStatusAutoUpdatesRequest(ushort portNumber, bool enabled)
        : base(HCIMessageID.RequestPanelKeysStatusAutoUpdates)
    {
        PortNumberStart = portNumber;
        PortNumberEnd = portNumber;
        Enabled = enabled;
    }

    /// <summary>
    /// Creates a new Request Panel Keys Status Auto Updates for a port range.
    /// </summary>
    /// <param name="portNumberStart">The first port number to apply the setting to.</param>
    /// <param name="portNumberEnd">The last port number to apply the setting to.</param>
    /// <param name="enabled">Whether auto updates are enabled.</param>
    public RequestPanelKeysStatusAutoUpdatesRequest(ushort portNumberStart, ushort portNumberEnd, bool enabled)
        : base(HCIMessageID.RequestPanelKeysStatusAutoUpdates)
    {
        PortNumberStart = portNumberStart;
        PortNumberEnd = portNumberEnd;
        Enabled = enabled;
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
        // Port Number Start: 2 bytes (big-endian)
        // Port Number End: 2 bytes (big-endian)
        // Enabled/Disabled: 1 byte (0 = Disabled, 1 = Enabled)

        var payload = new byte[10];
        int offset = 0;

        // Protocol Tag (HCIv2 marker)
        payload[offset++] = 0xAB;
        payload[offset++] = 0xBA;
        payload[offset++] = 0xCE;
        payload[offset++] = 0xDE;

        // Protocol Schema
        payload[offset++] = 0x01;

        // Port Number Start (big-endian)
        payload[offset++] = (byte)(PortNumberStart >> 8);
        payload[offset++] = (byte)(PortNumberStart & 0xFF);

        // Port Number End (big-endian)
        payload[offset++] = (byte)(PortNumberEnd >> 8);
        payload[offset++] = (byte)(PortNumberEnd & 0xFF);

        // Enabled/Disabled
        payload[offset++] = (byte)(Enabled ? 1 : 0);

        return payload;
    }
}
