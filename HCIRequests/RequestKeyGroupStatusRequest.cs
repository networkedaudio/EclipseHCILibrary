using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Key Group Status (HCIv2) - Message ID 0x00FB (251), Sub Message ID 0x03.
/// This message is used to request the current assignment state of the
/// specified key group.
/// </summary>
public class RequestKeyGroupStatusRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Sub Message ID for Key Group Status.
    /// </summary>
    public const byte SubMessageId = 0x03;

    /// <summary>
    /// System number of the key group.
    /// </summary>
    public byte KeyGroupSystemNumber { get; set; }

    /// <summary>
    /// Instance number of the key group.
    /// </summary>
    public ushort KeyGroupId { get; set; }

    /// <summary>
    /// Creates a new Request Key Group Status.
    /// </summary>
    public RequestKeyGroupStatusRequest()
        : base(HCIMessageID.RequestKeyGroupAction)
    {
    }

    /// <summary>
    /// Creates a new Request Key Group Status with the specified parameters.
    /// </summary>
    /// <param name="keyGroupSystemNumber">System number of the key group.</param>
    /// <param name="keyGroupId">Instance number of the key group.</param>
    public RequestKeyGroupStatusRequest(byte keyGroupSystemNumber, ushort keyGroupId)
        : base(HCIMessageID.RequestKeyGroupAction)
    {
        KeyGroupSystemNumber = keyGroupSystemNumber;
        KeyGroupId = keyGroupId;
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
        // Sub Message ID: 1 byte (0x03)
        // Key Group System Number: 1 byte
        // Key Group ID: 2 bytes (big-endian)

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // Sub Message ID: 1 byte
        ms.WriteByte(SubMessageId);

        // Key Group System Number: 1 byte
        ms.WriteByte(KeyGroupSystemNumber);

        // Key Group ID: 2 bytes (big-endian)
        ms.WriteByte((byte)(KeyGroupId >> 8));
        ms.WriteByte((byte)(KeyGroupId & 0xFF));

        return ms.ToArray();
    }
}
