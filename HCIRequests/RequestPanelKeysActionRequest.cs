using HCILibrary.Enums;
using HCILibrary.HCIResponses;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Represents a single key action to be applied to a panel.
/// </summary>
public class PanelKeyAction
{
    /// <summary>
    /// Region on this panel.
    /// </summary>
    public byte Region { get; set; }

    /// <summary>
    /// Key on this region.
    /// </summary>
    public byte Key { get; set; }

    /// <summary>
    /// Page on this region.
    /// </summary>
    public byte Page { get; set; }

    /// <summary>
    /// Key state (bits 0-1): 0 = OFF, 1 = ON, 2 = ON2 (internal use only).
    /// </summary>
    public KeyState KeyState { get; set; }

    /// <summary>
    /// Listen mode flag (bit 2): Comb Key on istation set to listen mode.
    /// </summary>
    public bool ListenMode { get; set; }

    /// <summary>
    /// Pot position (bits 4-7).
    /// </summary>
    public byte PotState { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PanelKeyAction"/> class.
    /// </summary>
    public PanelKeyAction()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PanelKeyAction"/> class with specified parameters.
    /// </summary>
    /// <param name="region">Region on the panel.</param>
    /// <param name="key">Key on the region.</param>
    /// <param name="page">Page on the region.</param>
    /// <param name="keyState">Key state (OFF, ON, ON2).</param>
    /// <param name="listenMode">Listen mode flag.</param>
    /// <param name="potState">Pot position (0-15).</param>
    public PanelKeyAction(byte region, byte key, byte page, KeyState keyState, bool listenMode = false, byte potState = 0)
    {
        Region = region;
        Key = key;
        Page = page;
        KeyState = keyState;
        ListenMode = listenMode;
        PotState = (byte)(potState & 0x0F);
    }

    /// <summary>
    /// Generates the DAK state byte from the component values.
    /// </summary>
    /// <returns>The DAK state byte.</returns>
    public byte GenerateDakState()
    {
        byte dakState = (byte)((byte)KeyState & 0x03);
        if (ListenMode)
        {
            dakState |= 0x04;
        }
        dakState |= (byte)((PotState & 0x0F) << 4);
        return dakState;
    }

    /// <summary>
    /// Writes this key action to a stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    public void WriteTo(Stream stream)
    {
        stream.WriteByte(Region);
        stream.WriteByte(Key);
        stream.WriteByte(Page);
        stream.WriteByte(GenerateDakState());
    }
}

/// <summary>
/// Request Panel Keys Action (0x00B3).
/// Sets the state of one or more keys on a specified panel.
/// HCIv2 only.
/// </summary>
public class RequestPanelKeysActionRequest : HCIRequest
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
    /// Gets the list of key actions to apply.
    /// </summary>
    public List<PanelKeyAction> KeyActions { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPanelKeysActionRequest"/> class.
    /// </summary>
    public RequestPanelKeysActionRequest()
        : base(HCIMessageID.RequestPanelKeysAction)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPanelKeysActionRequest"/> class
    /// with specified slot and port.
    /// </summary>
    /// <param name="slot">The card slot number.</param>
    /// <param name="port">The port offset from first port of the card.</param>
    public RequestPanelKeysActionRequest(byte slot, byte port)
        : base(HCIMessageID.RequestPanelKeysAction)
    {
        Slot = slot;
        Port = port;
    }

    /// <summary>
    /// Adds a key action to the request.
    /// </summary>
    /// <param name="action">The key action to add.</param>
    /// <returns>This request for method chaining.</returns>
    public RequestPanelKeysActionRequest AddKeyAction(PanelKeyAction action)
    {
        KeyActions.Add(action);
        return this;
    }

    /// <summary>
    /// Adds a key action to the request with specified parameters.
    /// </summary>
    /// <param name="region">Region on the panel.</param>
    /// <param name="key">Key on the region.</param>
    /// <param name="page">Page on the region.</param>
    /// <param name="keyState">Key state (OFF, ON, ON2).</param>
    /// <param name="listenMode">Listen mode flag.</param>
    /// <param name="potState">Pot position (0-15).</param>
    /// <returns>This request for method chaining.</returns>
    public RequestPanelKeysActionRequest AddKeyAction(
        byte region, 
        byte key, 
        byte page, 
        KeyState keyState, 
        bool listenMode = false, 
        byte potState = 0)
    {
        KeyActions.Add(new PanelKeyAction(region, key, page, keyState, listenMode, potState));
        return this;
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request Panel Keys Action.
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

        // Count: 2 bytes (big-endian)
        ushort count = (ushort)KeyActions.Count;
        ms.WriteByte((byte)(count >> 8));
        ms.WriteByte((byte)(count & 0xFF));

        // Key actions: 4 bytes each (Region + Key + Page + DakState)
        foreach (var action in KeyActions)
        {
            action.WriteTo(ms);
        }

        return ms.ToArray();
    }
}
