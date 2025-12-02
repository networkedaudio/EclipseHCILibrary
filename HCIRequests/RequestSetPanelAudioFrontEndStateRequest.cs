using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Set Panel Audio Front End State Request (HCIv2) - Message ID 0x0161 (353).
/// This message is used to set the HCI requested state of the panel MIC,
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
public class RequestSetPanelAudioFrontEndStateRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// The panel port number.
    /// </summary>
    public ushort PanelNumber { get; set; }

    /// <summary>
    /// The enabled state of the panel audio front end.
    /// When true, the panel mic, speaker and sidetone are enabled as if a talk key is pressed.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Creates a new Set Panel Audio Front End State Request.
    /// </summary>
    public RequestSetPanelAudioFrontEndStateRequest()
        : base(HCIMessageID.RequestSetPanelAudioFrontEndState)
    {
    }

    /// <summary>
    /// Creates a new Set Panel Audio Front End State Request with the specified parameters.
    /// </summary>
    /// <param name="panelNumber">The panel port number.</param>
    /// <param name="enabled">True to enable panel audio front end, false to disable.</param>
    public RequestSetPanelAudioFrontEndStateRequest(ushort panelNumber, bool enabled)
        : base(HCIMessageID.RequestSetPanelAudioFrontEndState)
    {
        PanelNumber = panelNumber;
        Enabled = enabled;
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
        // Enabled State: 1 byte (0 = disabled, 1 = enabled)

        using var ms = new MemoryStream();

        // Protocol Tag (Magic Number): 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Schema Number: 1 byte
        ms.WriteByte(0x01);

        // Panel Number: 2 bytes (big-endian)
        ms.WriteByte((byte)(PanelNumber >> 8));
        ms.WriteByte((byte)(PanelNumber & 0xFF));

        // Enabled State: 1 byte
        ms.WriteByte((byte)(Enabled ? 1 : 0));

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
