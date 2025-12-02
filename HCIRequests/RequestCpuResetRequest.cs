using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request CPU Reset (HCIv2) - Message ID 0x0029.
/// This message instructs the CPU to execute one of the four available reset instructions.
/// </summary>
public class RequestCpuResetRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version.
    /// </summary>
    private const byte ProtocolSchema = 0x01;

    /// <summary>
    /// The type of reset to perform.
    /// </summary>
    public CpuResetType ResetType { get; set; }

    /// <summary>
    /// Creates a new Request CPU Reset request.
    /// </summary>
    /// <param name="resetType">The type of reset to perform.</param>
    public RequestCpuResetRequest(CpuResetType resetType = CpuResetType.None)
        : base(HCIMessageID.RequestCpuReset)
    {
        ResetType = resetType;
    }

    /// <summary>
    /// Creates a request for a red reset.
    /// </summary>
    public static RequestCpuResetRequest RedReset() => new(CpuResetType.Red);

    /// <summary>
    /// Creates a request for a black reset.
    /// </summary>
    public static RequestCpuResetRequest BlackReset() => new(CpuResetType.Black);

    /// <summary>
    /// Generates the payload for this request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload: ProtocolTag(4) + ProtocolSchema(1) + ResetType(1) = 6 bytes
        var payload = new byte[6];

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Reset Type: 1 byte (bit 0 = red, bit 1 = black)
        payload[offset++] = (byte)ResetType;

        return payload;
    }
}
