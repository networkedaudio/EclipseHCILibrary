using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Alt Text Set (HCIv2) - Message ID 0x0180 (378).
/// This message is used to set the state of the Alt Text feature of a
/// panel/panels in the local matrix.
/// </summary>
public class RequestAltTextSetRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Port number of the panel (0-based).
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Alt Text state. True to enable, false to disable.
    /// </summary>
    public bool State { get; set; }

    /// <summary>
    /// Creates a new Request Alt Text Set with default values.
    /// </summary>
    public RequestAltTextSetRequest()
        : base(HCIMessageID.RequestAltTextSet)
    {
    }

    /// <summary>
    /// Creates a new Request Alt Text Set for a specific panel.
    /// </summary>
    /// <param name="portNumber">The 0-based port number of the panel.</param>
    /// <param name="state">True to enable Alt Text, false to disable.</param>
    public RequestAltTextSetRequest(ushort portNumber, bool state)
        : base(HCIMessageID.RequestAltTextSet)
    {
        PortNumber = portNumber;
        State = state;
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
        // Port Number: 2 bytes (big-endian), 0-based
        // State: 1 byte (0 = Off, 1 = On)

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // Port Number: 2 bytes (big-endian)
        ms.WriteByte((byte)(PortNumber >> 8));
        ms.WriteByte((byte)(PortNumber & 0xFF));

        // State: 1 byte (0 = Off, 1 = On)
        ms.WriteByte((byte)(State ? 1 : 0));

        return ms.ToArray();
    }
}
