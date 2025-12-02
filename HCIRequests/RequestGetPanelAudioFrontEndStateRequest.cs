using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Get Panel Audio Front End State Request (HCIv2) - Message ID 0x0163 (355).
/// This message is used to get the HCI requested state of the panel MIC,
/// speaker and sidetone state.
/// 
/// When enabled, the state of the panel is exactly as though a talk key has
/// been pressed. This HCI request simply acts as though a virtual talk key is pressed.
/// 
/// For example, if a panel talk key is already pressed and the HCI request is
/// received then the panel audio hardware state does not change. However, if
/// the panel talk key were now released the panel mic, sidetone etc would
/// stay in the same state i.e. as though a talk key is still pressed, until the
/// HCI disable equivalent of this message is received by the matrix.
/// 
/// This message is useful when e.g. a 3rd party application is using the HX
/// panel for routing to other types of device than the HX matrix.
/// </summary>
public class RequestGetPanelAudioFrontEndStateRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Value representing all panels.
    /// </summary>
    public const ushort AllPanels = 65535;

    /// <summary>
    /// The panel port number. Use AllPanels (65535) to query all panels.
    /// </summary>
    public ushort PanelNumber { get; set; }

    /// <summary>
    /// Creates a new Get Panel Audio Front End State Request.
    /// </summary>
    public RequestGetPanelAudioFrontEndStateRequest()
        : base(HCIMessageID.RequestGetPanelAudioFrontEndState)
    {
    }

    /// <summary>
    /// Creates a new Get Panel Audio Front End State Request for a specific panel.
    /// </summary>
    /// <param name="panelNumber">The panel port number, or AllPanels (65535) for all panels.</param>
    public RequestGetPanelAudioFrontEndStateRequest(ushort panelNumber)
        : base(HCIMessageID.RequestGetPanelAudioFrontEndState)
    {
        PanelNumber = panelNumber;
    }

    /// <summary>
    /// Creates a new Get Panel Audio Front End State Request for all panels.
    /// </summary>
    /// <returns>A request configured for all panels.</returns>
    public static RequestGetPanelAudioFrontEndStateRequest ForAllPanels()
    {
        return new RequestGetPanelAudioFrontEndStateRequest(AllPanels);
    }

    /// <summary>
    /// Generates the payload for the request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // Protocol Tag (Magic Number): 4 bytes (0xABBACEDE)
        // Schema Number: 1 byte (set to 1)
        // Panel Number: 2 bytes (big-endian)

        using var ms = new MemoryStream();

        // Protocol Tag (Magic Number): 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Schema Number: 1 byte
        ms.WriteByte(0x01);

        // Panel Number: 2 bytes (big-endian)
        ms.WriteByte((byte)(PanelNumber >> 8));
        ms.WriteByte((byte)(PanelNumber & 0xFF));

        return ms.ToArray();
    }

    /// <summary>
    /// Builds the complete message bytes including start/end markers and length.
    /// Overridden to set the correct flags byte (0x08).
    /// </summary>
    /// <returns>The complete message as a byte array.</returns>
    public override byte[] BuildMessage()
    {
        var payload = GeneratePayload();

        // Calculate total length (excluding start marker but including length bytes and end marker)
        // Length = 2 (length) + 2 (message ID) + 1 (flags) + payload + 2 (end marker)
        ushort length = (ushort)(2 + 2 + 1 + payload.Length + 2);

        using var ms = new MemoryStream();

        // Start marker
        ms.Write(StartMarker, 0, StartMarker.Length);

        // Length (big-endian)
        ms.WriteByte((byte)(length >> 8));
        ms.WriteByte((byte)(length & 0xFF));

        // Message ID (big-endian)
        ms.WriteByte((byte)((ushort)MessageID >> 8));
        ms.WriteByte((byte)((ushort)MessageID & 0xFF));

        // Flags: 0x08 as per protocol spec
        ms.WriteByte(0x08);

        // Payload
        if (payload.Length > 0)
        {
            ms.Write(payload, 0, payload.Length);
        }

        // End marker
        ms.Write(EndMarker, 0, EndMarker.Length);

        return ms.ToArray();
    }
}
