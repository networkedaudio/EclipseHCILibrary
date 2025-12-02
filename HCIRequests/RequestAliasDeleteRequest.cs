using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Alias Delete (0x0084).
/// Used by the host to delete an alias previously requested.
/// Only aliases made by an HCI link to the local system can be deleted.
/// HCIv2 only.
/// </summary>
public class RequestAliasDeleteRequest : HCIRequest
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
    /// The list of alias dialcodes to delete.
    /// </summary>
    public List<AliasDialcode> Dialcodes { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestAliasDeleteRequest"/> class.
    /// </summary>
    public RequestAliasDeleteRequest()
        : base(HCIMessageID.RequestAliasDelete)
    {
    }

    /// <summary>
    /// Adds a dialcode to the delete list.
    /// </summary>
    /// <param name="dialcode">The dialcode to delete.</param>
    public void AddDialcode(AliasDialcode dialcode)
    {
        Dialcodes.Add(dialcode);
    }

    /// <summary>
    /// Adds a dialcode to the delete list.
    /// </summary>
    /// <param name="entityInstance">Entity instance (0-65535).</param>
    /// <param name="entityType">Entity type.</param>
    /// <param name="targetSystemNumber">Target system number (0 for local).</param>
    public void AddDialcode(ushort entityInstance, byte entityType, byte targetSystemNumber = 0)
    {
        Dialcodes.Add(new AliasDialcode(entityInstance, entityType, targetSystemNumber));
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request Alias Delete.
    /// Payload: Protocol Tag (4 bytes) + Protocol Schema (1 byte) + Count (2 bytes) + Alias Data (4 bytes each).
    /// </summary>
    /// <returns>The payload byte array.</returns>
    protected override byte[] GeneratePayload()
    {
        using var ms = new MemoryStream();

        // HCIv2 protocol tag
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol schema
        ms.WriteByte(ProtocolSchema);

        // Count (16-bit, big-endian)
        ushort count = (ushort)Dialcodes.Count;
        ms.WriteByte((byte)((count >> 8) & 0xFF));
        ms.WriteByte((byte)(count & 0xFF));

        // Alias data (4 bytes each)
        foreach (var dialcode in Dialcodes)
        {
            ms.Write(dialcode.ToBytes(), 0, 4);
        }

        return ms.ToArray();
    }
}
