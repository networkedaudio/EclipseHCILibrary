using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request All Key Group Statuses (HCIv2) - Message ID 0x00FB (251), Sub Message ID 0x02.
/// This message is used to request the key entity associated for all key
/// groups on the current matrix.
/// </summary>
public class RequestAllKeyGroupStatusesRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Sub Message ID for All Key Group Statuses.
    /// </summary>
    public const byte SubMessageId = 0x02;

    /// <summary>
    /// Creates a new Request All Key Group Statuses.
    /// </summary>
    public RequestAllKeyGroupStatusesRequest()
        : base(HCIMessageID.RequestKeyGroupAction)
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
        // Sub Message ID: 1 byte (0x02)

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // Sub Message ID: 1 byte
        ms.WriteByte(SubMessageId);

        return ms.ToArray();
    }
}
