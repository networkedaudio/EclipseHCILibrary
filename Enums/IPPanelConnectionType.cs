namespace HCILibrary.Enums;

/// <summary>
/// Connection type for IP panel settings.
/// </summary>
public enum IPPanelConnectionType : byte
{
    /// <summary>
    /// LAN connection type.
    /// </summary>
    LAN = 0,

    /// <summary>
    /// WAN connection type.
    /// </summary>
    WAN = 1,

    /// <summary>
    /// Internet connection type.
    /// </summary>
    Internet = 2,

    /// <summary>
    /// Default connection type (use type in MAP).
    /// Note: Default should be used when possible, otherwise panel may need
    /// to connect twice to apply non-map default type.
    /// </summary>
    Default = 3
}
