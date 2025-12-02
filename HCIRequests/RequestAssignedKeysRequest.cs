using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Assigned Keys (0x00E7).
/// Requests all assigned key configuration for a selected panel.
/// These assignments are the net result of the map configuration downloaded
/// baseline plus any HCI, online, panel based assignments on top of this.
/// HCIv2 only.
/// </summary>
public class RequestAssignedKeysRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Gets or sets the protocol schema version (1 or 2).
    /// Schema 2 returns more information in the reply.
    /// </summary>
    public AssignedKeysSchema Schema { get; set; } = AssignedKeysSchema.Schema1;

    /// <summary>
    /// Gets or sets the card slot number.
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    /// Gets or sets the port offset from first port of the card.
    /// </summary>
    public byte Port { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestAssignedKeysRequest"/> class.
    /// </summary>
    public RequestAssignedKeysRequest()
        : base(HCIMessageID.RequestAssignedKeys)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestAssignedKeysRequest"/> class
    /// with specified slot and port.
    /// </summary>
    /// <param name="slot">The card slot number.</param>
    /// <param name="port">The port offset from first port of the card.</param>
    /// <param name="schema">The protocol schema version (default: Schema1).</param>
    public RequestAssignedKeysRequest(byte slot, byte port, AssignedKeysSchema schema = AssignedKeysSchema.Schema1)
        : base(HCIMessageID.RequestAssignedKeys)
    {
        Slot = slot;
        Port = port;
        Schema = schema;
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request Assigned Keys.
    /// Payload: Protocol Tag (4 bytes) + Protocol Schema (1 byte) + Slot (1 byte) + Port (1 byte).
    /// </summary>
    /// <returns>The payload byte array.</returns>
    protected override byte[] GeneratePayload()
    {
        using var ms = new MemoryStream();

        // HCIv2 protocol tag
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol schema (1 or 2)
        ms.WriteByte((byte)Schema);

        // Slot: 1 byte
        ms.WriteByte(Slot);

        // Port: 1 byte
        ms.WriteByte(Port);

        return ms.ToArray();
    }
}
