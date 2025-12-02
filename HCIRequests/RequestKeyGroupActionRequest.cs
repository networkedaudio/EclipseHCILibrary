using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Key Group Action (HCIv2) - Message ID 0x00FB (251).
/// This message is used to request the association of a key entity (port, IFB
/// instance, etc.) to a key group present in the matrix configuration.
/// The matrix replies with a Reply Key Group Status message.
/// </summary>
public class RequestKeyGroupActionRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Sub Message ID for Key Group Action.
    /// </summary>
    public const byte SubMessageId = 0x01;

    /// <summary>
    /// System number of the key group.
    /// </summary>
    public byte KeyGroupSystemNumber { get; set; }

    /// <summary>
    /// Instance number of the key group.
    /// </summary>
    public ushort KeyGroupId { get; set; }

    /// <summary>
    /// System number of the target entity (port, IFB, etc.).
    /// </summary>
    public byte TargetEntitySystemId { get; set; }

    /// <summary>
    /// Type ID of the target entity.
    /// </summary>
    public KeyEntityType EntityType { get; set; }

    /// <summary>
    /// Instance number of the target entity.
    /// </summary>
    public ushort EntityInstance { get; set; }

    /// <summary>
    /// Creates a new Request Key Group Action.
    /// </summary>
    public RequestKeyGroupActionRequest()
        : base(HCIMessageID.RequestKeyGroupAction)
    {
    }

    /// <summary>
    /// Creates a new Request Key Group Action with the specified parameters.
    /// </summary>
    /// <param name="keyGroupSystemNumber">System number of the key group.</param>
    /// <param name="keyGroupId">Instance number of the key group.</param>
    /// <param name="targetEntitySystemId">System number of the target entity.</param>
    /// <param name="entityType">Type ID of the target entity.</param>
    /// <param name="entityInstance">Instance number of the target entity.</param>
    public RequestKeyGroupActionRequest(
        byte keyGroupSystemNumber,
        ushort keyGroupId,
        byte targetEntitySystemId,
        KeyEntityType entityType,
        ushort entityInstance)
        : base(HCIMessageID.RequestKeyGroupAction)
    {
        KeyGroupSystemNumber = keyGroupSystemNumber;
        KeyGroupId = keyGroupId;
        TargetEntitySystemId = targetEntitySystemId;
        EntityType = entityType;
        EntityInstance = entityInstance;
    }

    /// <summary>
    /// Creates a new Request Key Group Action for a port entity.
    /// </summary>
    /// <param name="keyGroupSystemNumber">System number of the key group.</param>
    /// <param name="keyGroupId">Instance number of the key group.</param>
    /// <param name="portSystemId">System number of the port.</param>
    /// <param name="portInstance">Instance number of the port.</param>
    /// <returns>A new request configured for a port entity.</returns>
    public static RequestKeyGroupActionRequest ForPort(
        byte keyGroupSystemNumber,
        ushort keyGroupId,
        byte portSystemId,
        ushort portInstance)
    {
        return new RequestKeyGroupActionRequest(
            keyGroupSystemNumber,
            keyGroupId,
            portSystemId,
            KeyEntityType.Port,
            portInstance);
    }

    /// <summary>
    /// Creates a new Request Key Group Action for an IFB entity.
    /// </summary>
    /// <param name="keyGroupSystemNumber">System number of the key group.</param>
    /// <param name="keyGroupId">Instance number of the key group.</param>
    /// <param name="ifbSystemId">System number of the IFB.</param>
    /// <param name="ifbInstance">Instance number of the IFB.</param>
    /// <returns>A new request configured for an IFB entity.</returns>
    public static RequestKeyGroupActionRequest ForIfb(
        byte keyGroupSystemNumber,
        ushort keyGroupId,
        byte ifbSystemId,
        ushort ifbInstance)
    {
        return new RequestKeyGroupActionRequest(
            keyGroupSystemNumber,
            keyGroupId,
            ifbSystemId,
            KeyEntityType.Ifb,
            ifbInstance);
    }

    /// <summary>
    /// Creates a new Request Key Group Action for a conference entity.
    /// </summary>
    /// <param name="keyGroupSystemNumber">System number of the key group.</param>
    /// <param name="keyGroupId">Instance number of the key group.</param>
    /// <param name="conferenceSystemId">System number of the conference.</param>
    /// <param name="conferenceInstance">Instance number of the conference.</param>
    /// <returns>A new request configured for a conference entity.</returns>
    public static RequestKeyGroupActionRequest ForConference(
        byte keyGroupSystemNumber,
        ushort keyGroupId,
        byte conferenceSystemId,
        ushort conferenceInstance)
    {
        return new RequestKeyGroupActionRequest(
            keyGroupSystemNumber,
            keyGroupId,
            conferenceSystemId,
            KeyEntityType.Conference,
            conferenceInstance);
    }

    /// <summary>
    /// Creates a new Request Key Group Action for a group entity.
    /// </summary>
    /// <param name="keyGroupSystemNumber">System number of the key group.</param>
    /// <param name="keyGroupId">Instance number of the key group.</param>
    /// <param name="groupSystemId">System number of the group.</param>
    /// <param name="groupInstance">Instance number of the group.</param>
    /// <returns>A new request configured for a group entity.</returns>
    public static RequestKeyGroupActionRequest ForGroup(
        byte keyGroupSystemNumber,
        ushort keyGroupId,
        byte groupSystemId,
        ushort groupInstance)
    {
        return new RequestKeyGroupActionRequest(
            keyGroupSystemNumber,
            keyGroupId,
            groupSystemId,
            KeyEntityType.Group,
            groupInstance);
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
        // Sub Message ID: 1 byte (0x01)
        // Key Group System Number: 1 byte
        // Key Group ID: 2 bytes (big-endian)
        // Target Entity System ID: 1 byte
        // Entity Type: 1 byte
        // Entity Instance: 2 bytes (big-endian)

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

        // Target Entity System ID: 1 byte
        ms.WriteByte(TargetEntitySystemId);

        // Entity Type: 1 byte
        ms.WriteByte((byte)EntityType);

        // Entity Instance: 2 bytes (big-endian)
        ms.WriteByte((byte)(EntityInstance >> 8));
        ms.WriteByte((byte)(EntityInstance & 0xFF));

        return ms.ToArray();
    }
}
