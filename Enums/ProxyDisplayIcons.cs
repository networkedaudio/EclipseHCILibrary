namespace HCILibrary.Enums;

/// <summary>
/// Icons that can be displayed on proxy key displays.
/// These flags can be combined.
/// </summary>
[Flags]
public enum ProxyDisplayIcons : ushort
{
    /// <summary>
    /// No icons displayed.
    /// </summary>
    None = 0x00,

    /// <summary>
    /// VOX (voice-operated switch) icon.
    /// </summary>
    Vox = 0x01,

    /// <summary>
    /// Monitored icon.
    /// </summary>
    Monitored = 0x02,

    /// <summary>
    /// Telos/SIP telephony icon.
    /// </summary>
    TelosSip = 0x04,

    /// <summary>
    /// Relay icon.
    /// </summary>
    Relay = 0x08,

    /// <summary>
    /// Up arrow icon.
    /// </summary>
    UpArrow = 0x10,

    /// <summary>
    /// Down arrow icon.
    /// </summary>
    DownArrow = 0x11
}
