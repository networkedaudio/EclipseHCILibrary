using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request IFB Status (0x003D).
/// Requests any of the attributes of the specified IFB.
/// If the message is sent to a matrix that doesn't own the IFB (meaning the IFB was
/// created on another matrix), IFB Attribute Type must be set to PotentialCallers,
/// otherwise no answer will be returned.
/// The IFB attribute information is returned in the Reply IFB Status message.
/// HCIv2 only.
/// </summary>
public class RequestIfbStatusRequest : HCIRequest
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
    /// Gets or sets the IFB attribute type to request.
    /// Use <see cref="IfbAttributeType.PotentialCallers"/> when requesting from a matrix
    /// that doesn't own the IFB.
    /// Use <see cref="IfbAttributeType.All"/> to request all attribute types.
    /// </summary>
    public IfbAttributeType AttributeType { get; set; }

    /// <summary>
    /// Gets or sets the matrix identifier.
    /// </summary>
    public byte MatrixIdentifier { get; set; }

    /// <summary>
    /// Gets or sets the unique IFB identifier within the context of this frame.
    /// IFB identifiers are uniquely addressable across linked systems by prefixing
    /// the matrix identifier.
    /// </summary>
    public ushort IfbIdentifier { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestIfbStatusRequest"/> class.
    /// </summary>
    public RequestIfbStatusRequest()
        : base(HCIMessageID.RequestIfbStatus)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestIfbStatusRequest"/> class
    /// with the specified parameters.
    /// </summary>
    /// <param name="matrixIdentifier">The matrix identifier.</param>
    /// <param name="ifbIdentifier">The unique IFB identifier.</param>
    /// <param name="attributeType">The IFB attribute type to request.</param>
    public RequestIfbStatusRequest(byte matrixIdentifier, ushort ifbIdentifier, 
        IfbAttributeType attributeType = IfbAttributeType.All)
        : base(HCIMessageID.RequestIfbStatus)
    {
        MatrixIdentifier = matrixIdentifier;
        IfbIdentifier = ifbIdentifier;
        AttributeType = attributeType;
    }

    /// <summary>
    /// Creates a request for all IFB attributes.
    /// </summary>
    /// <param name="matrixIdentifier">The matrix identifier.</param>
    /// <param name="ifbIdentifier">The unique IFB identifier.</param>
    /// <returns>A new RequestIfbStatusRequest configured to request all attributes.</returns>
    public static RequestIfbStatusRequest CreateForAllAttributes(byte matrixIdentifier, ushort ifbIdentifier)
    {
        return new RequestIfbStatusRequest(matrixIdentifier, ifbIdentifier, IfbAttributeType.All);
    }

    /// <summary>
    /// Creates a request for potential callers (use when requesting from non-owning matrix).
    /// </summary>
    /// <param name="matrixIdentifier">The matrix identifier.</param>
    /// <param name="ifbIdentifier">The unique IFB identifier.</param>
    /// <returns>A new RequestIfbStatusRequest configured to request potential callers.</returns>
    public static RequestIfbStatusRequest CreateForPotentialCallers(byte matrixIdentifier, ushort ifbIdentifier)
    {
        return new RequestIfbStatusRequest(matrixIdentifier, ifbIdentifier, IfbAttributeType.PotentialCallers);
    }

    /// <summary>
    /// Generates the payload for the Request IFB Status message.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // Protocol Tag: 4 bytes (0xABBACEDE)
        // Protocol Schema: 1 byte
        // IFB Attribute Type: 1 byte
        // Matrix Identifier: 1 byte
        // IFB Identifier: 2 bytes (big-endian)

        var payload = new byte[9];
        int offset = 0;

        // Protocol Tag (4 bytes)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema (1 byte)
        payload[offset++] = ProtocolSchema;

        // IFB Attribute Type (1 byte)
        payload[offset++] = (byte)AttributeType;

        // Matrix Identifier (1 byte)
        payload[offset++] = MatrixIdentifier;

        // IFB Identifier (2 bytes, big-endian)
        payload[offset++] = (byte)((IfbIdentifier >> 8) & 0xFF);
        payload[offset++] = (byte)(IfbIdentifier & 0xFF);

        return payload;
    }
}
