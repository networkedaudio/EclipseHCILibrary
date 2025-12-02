using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request System Messages (0x0002).
/// Used to request the broadcast of System event log messages.
/// HCIv2 only.
/// </summary>
public class RequestSystemMessagesRequest : HCIRequest
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
    /// Initializes a new instance of the <see cref="RequestSystemMessagesRequest"/> class.
    /// </summary>
    public RequestSystemMessagesRequest()
        : base(HCIMessageID.RequestSystemMessages)
    {
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request System Messages.
    /// Payload: Protocol Tag (4 bytes) + Protocol Schema (1 byte) + Unused (2 bytes).
    /// </summary>
    /// <returns>The payload byte array.</returns>
    protected override byte[] GeneratePayload()
    {
        using var ms = new MemoryStream();

        // HCIv2 protocol tag
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol schema
        ms.WriteByte(ProtocolSchema);

        // Unused (16-bit word, set to 0)
        ms.WriteByte(0x00);
        ms.WriteByte(0x00);

        return ms.ToArray();
    }
}
