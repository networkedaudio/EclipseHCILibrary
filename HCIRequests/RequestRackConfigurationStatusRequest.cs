using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Rack Configuration Status (HCIv2) - Message ID 0x002C (44), Sub ID 12.
/// This message is used to request the configuration status from the matrix.
/// </summary>
public class RequestRackConfigurationStatusRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Sub Message ID for Rack Configuration Status.
    /// </summary>
    public const byte SubMessageId = 0x0C;

    /// <summary>
    /// Creates a new Request Rack Configuration Status.
    /// </summary>
    public RequestRackConfigurationStatusRequest()
        : base(HCIMessageID.RequestRackProperties)
    {
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
        // Sub ID: 1 byte (set to 12)

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // Sub ID: 1 byte
        ms.WriteByte(SubMessageId);

        return ms.ToArray();
    }
}
