using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Input Level Status (0x0021).
/// Asks the matrix to send information about the current settings of all audio input levels.
/// HCIv2 only.
/// </summary>
public class RequestInputLevelStatusRequest : HCIRequest
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
    /// Initializes a new instance of the <see cref="RequestInputLevelStatusRequest"/> class.
    /// </summary>
    public RequestInputLevelStatusRequest()
        : base(HCIMessageID.RequestInputLevelStatus)
    {
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request Input Level Status.
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
