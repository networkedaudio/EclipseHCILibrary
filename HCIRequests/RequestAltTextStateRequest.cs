using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Alt Text State (HCIv2) - Message ID 0x0178 (376).
/// This message is used to request the 'Alt Text' display state of a specified
/// panel or all panels. The Alt Text feature can be activated/deactivated via
/// a panel key or via HCI message.
/// </summary>
public class RequestAltTextStateRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Value to target all panels.
    /// </summary>
    public const ushort AllPanels = 65535;

    /// <summary>
    /// Port number of the panel. Set to 65535 for all panels.
    /// </summary>
    public ushort PortNumber { get; set; } = AllPanels;

    /// <summary>
    /// Creates a new Request Alt Text State for all panels.
    /// </summary>
    public RequestAltTextStateRequest()
        : base(HCIMessageID.RequestAltTextState)
    {
    }

    /// <summary>
    /// Creates a new Request Alt Text State for a specific panel.
    /// </summary>
    /// <param name="portNumber">The port number of the panel, or 65535 for all panels.</param>
    public RequestAltTextStateRequest(ushort portNumber)
        : base(HCIMessageID.RequestAltTextState)
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
        // Port Number: 2 bytes (big-endian), 65535 for all panels

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // Port Number: 2 bytes (big-endian)
        ms.WriteByte((byte)(PortNumber >> 8));
        ms.WriteByte((byte)(PortNumber & 0xFF));

        return ms.ToArray();
    }
}
