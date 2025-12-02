using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Entity Info (0x00AF).
/// Requests entity instance information from the target matrix frame.
/// The information returned indicates the existence of an entity and its label in UTF-16 Unicode format.
/// Supported entity types: Conferences, Groups, IFBs.
/// HCIv2 only.
/// </summary>
public class RequestEntityInfoRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Gets or sets the protocol schema version. Currently set to 1.
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// Gets or sets the entity type to request.
    /// Use <see cref="EntityInfoType.All"/> to request all supported entity types.
    /// </summary>
    public EntityInfoType EntityType { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestEntityInfoRequest"/> class.
    /// </summary>
    public RequestEntityInfoRequest()
        : base(HCIMessageID.RequestEntityInfo)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestEntityInfoRequest"/> class
    /// with the specified entity type.
    /// </summary>
    /// <param name="entityType">The entity type to request.</param>
    public RequestEntityInfoRequest(EntityInfoType entityType)
        : base(HCIMessageID.RequestEntityInfo)
    {
        EntityType = entityType;
    }

    /// <summary>
    /// Creates a request for all entity types (Conferences, Groups, and IFBs).
    /// </summary>
    /// <returns>A new RequestEntityInfoRequest configured for all entities.</returns>
    public static RequestEntityInfoRequest CreateForAll()
    {
        return new RequestEntityInfoRequest(EntityInfoType.All);
    }

    /// <summary>
    /// Creates a request for conferences only.
    /// </summary>
    /// <returns>A new RequestEntityInfoRequest configured for conferences.</returns>
    public static RequestEntityInfoRequest CreateForConferences()
    {
        return new RequestEntityInfoRequest(EntityInfoType.Conferences);
    }

    /// <summary>
    /// Creates a request for groups only.
    /// </summary>
    /// <returns>A new RequestEntityInfoRequest configured for groups.</returns>
    public static RequestEntityInfoRequest CreateForGroups()
    {
        return new RequestEntityInfoRequest(EntityInfoType.Groups);
    }

    /// <summary>
    /// Creates a request for IFBs only.
    /// </summary>
    /// <returns>A new RequestEntityInfoRequest configured for IFBs.</returns>
    public static RequestEntityInfoRequest CreateForIfbs()
    {
        return new RequestEntityInfoRequest(EntityInfoType.Ifbs);
    }

    /// <summary>
    /// Generates the payload for the Request Entity Info message.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // Protocol Tag: 4 bytes (0xABBACEDE)
        // Protocol Schema: 1 byte
        // Entity Type: 1 byte

        var payload = new byte[6];
        int offset = 0;

        // Protocol Tag (4 bytes)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema (1 byte)
        payload[offset++] = ProtocolSchema;

        // Entity Type (1 byte)
        payload[offset++] = (byte)EntityType;

        return payload;
    }
}
