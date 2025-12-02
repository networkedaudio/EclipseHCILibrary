using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Locally Assigned Keys (0x00B9).
/// Requests all locally assigned key configuration for a selected panel.
/// A locally assigned key is a key that has been assigned by means other than
/// map configuration download (e.g., EHX online key assign, HCI API assignment,
/// panel fast key assign, scroll group key assignment, etc.).
/// HCIv2 only.
/// </summary>
public class RequestLocallyAssignedKeysRequest : HCIRequest
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
    /// Gets or sets the card slot number.
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    /// Gets or sets the port offset from first port of the card.
    /// </summary>
    public byte Port { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestLocallyAssignedKeysRequest"/> class.
    /// </summary>
    public RequestLocallyAssignedKeysRequest()
        : base(HCIMessageID.RequestLocallyAssignedKeys)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestLocallyAssignedKeysRequest"/> class
    /// with specified slot and port.
    /// </summary>
    /// <param name="slot">The card slot number.</param>
    /// <param name="port">The port offset from first port of the card.</param>
    public RequestLocallyAssignedKeysRequest(byte slot, byte port)
        : base(HCIMessageID.RequestLocallyAssignedKeys)
    {
        Slot = slot;
        Port = port;
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request Locally Assigned Keys.
    /// Payload: Protocol Tag (4 bytes) + Protocol Schema (1 byte) + Slot (1 byte) + Port (1 byte).
    /// </summary>
    /// <returns>The payload byte array.</returns>
    protected override byte[] GeneratePayload()
    {
        using var ms = new MemoryStream();

        // HCIv2 protocol tag
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol schema
        ms.WriteByte(ProtocolSchema);

        // Slot: 1 byte
        ms.WriteByte(Slot);

        // Port: 1 byte
        ms.WriteByte(Port);

        return ms.ToArray();
    }
}
