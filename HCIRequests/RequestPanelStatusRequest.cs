using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Panel Status (0x0005).
/// Asks the matrix to send information about the current state (online/offline)
/// of the panels/endpoints available to the system.
/// HCIv2 only.
/// </summary>
public class RequestPanelStatusRequest : HCIRequest
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
    /// Initializes a new instance of the <see cref="RequestPanelStatusRequest"/> class.
    /// </summary>
    public RequestPanelStatusRequest()
        : base(HCIMessageID.RequestPanelStatus)
    {
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request Panel Status.
    /// Payload: Protocol Tag (4 bytes) + Protocol Schema (1 byte).
    /// </summary>
    /// <returns>The payload byte array.</returns>
    protected override byte[] GeneratePayload()
    {
        using var ms = new MemoryStream();

        // HCIv2 protocol tag
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol schema
        ms.WriteByte(ProtocolSchema);

        return ms.ToArray();
    }
}
