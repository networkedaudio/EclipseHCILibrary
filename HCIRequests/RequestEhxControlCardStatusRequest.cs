using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request EHX Control Card Status (0x0015).
/// Causes the CSU to generate a Reply EHX Control Card Status message.
/// HCIv2 only.
/// </summary>
public class RequestEhxControlCardStatusRequest : HCIRequest
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
    /// Initializes a new instance of the <see cref="RequestEhxControlCardStatusRequest"/> class.
    /// </summary>
    public RequestEhxControlCardStatusRequest()
        : base(HCIMessageID.RequestEhxControlCardStatus)
    {
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request EHX Control Card Status.
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
