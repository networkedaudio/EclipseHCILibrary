using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Assigned Keys (With Labels) (HCIv2) - Message ID 0x017C (380).
/// Requests all assigned key configuration for selected panel. These
/// assignments are the net result of the map configuration downloaded
/// baseline plus any HCI, online, panel based assignments on top of this.
/// This message is the same as the Request Assigned Keys (ID 231), however
/// each entry in the reply includes the label shown on the key.
/// </summary>
public class RequestAssignedKeysWithLabelsRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Card slot number.
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    /// Port offset from first port of the card.
    /// </summary>
    public byte Port { get; set; }

    /// <summary>
    /// Creates a new Request Assigned Keys (With Labels).
    /// </summary>
    public RequestAssignedKeysWithLabelsRequest()
        : base(HCIMessageID.RequestAssignedKeysWithLabels)
    {
    }

    /// <summary>
    /// Creates a new Request Assigned Keys (With Labels) for a specific slot and port.
    /// </summary>
    /// <param name="slot">Card slot number.</param>
    /// <param name="port">Port offset from first port of the card.</param>
    public RequestAssignedKeysWithLabelsRequest(byte slot, byte port)
        : base(HCIMessageID.RequestAssignedKeysWithLabels)
    {
        Slot = slot;
        Port = port;
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
        // Slot: 1 byte (card slot number)
        // Port: 1 byte (port offset from first port of the card)

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // Slot: 1 byte
        ms.WriteByte(Slot);

        // Port: 1 byte
        ms.WriteByte(Port);

        return ms.ToArray();
    }
}
