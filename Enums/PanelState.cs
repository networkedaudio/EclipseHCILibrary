namespace HCILibrary.Enums;

/// <summary>
/// Panel state/condition enumeration.
/// </summary>
public enum PanelState : byte
{
    /// <summary>
    /// Unknown state.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Panel is in good condition (online).
    /// </summary>
    Good = 1,

    /// <summary>
    /// Panel is absent.
    /// </summary>
    Absent = 2,

    /// <summary>
    /// Panel is faulty.
    /// </summary>
    Faulty = 3,

    /// <summary>
    /// Panel is disconnected.
    /// </summary>
    Disconnect = 4
}
