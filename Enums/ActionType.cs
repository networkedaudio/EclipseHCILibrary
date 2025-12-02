namespace HCILibrary.Enums;

/// <summary>
/// Represents the Action Type flags for Request Actions Status.
/// </summary>
[Flags]
public enum ActionType : ushort
{
    /// <summary>
    /// No action types selected.
    /// </summary>
    None = 0,

    /// <summary>
    /// Crosspoint action (bit 0).
    /// </summary>
    Crosspoint = 1 << 0,

    /// <summary>
    /// Reserved (bit 1).
    /// </summary>
    Reserved1 = 1 << 1,

    /// <summary>
    /// GPIO/SFO action (bit 2).
    /// </summary>
    GpioSfo = 1 << 2,

    /// <summary>
    /// Reserved (bit 3).
    /// </summary>
    Reserved3 = 1 << 3,

    /// <summary>
    /// Reserved (bit 4).
    /// </summary>
    Reserved4 = 1 << 4,

    /// <summary>
    /// Conference action (bit 5).
    /// </summary>
    Conference = 1 << 5,

    /// <summary>
    /// EHX Control action (value 0x0004).
    /// Used for GPO and EHX control state changes.
    /// </summary>
    EhxControl = 0x0004

    // Bits 6-15 are reserved for flags, but EhxControl uses a different value scheme.
}
