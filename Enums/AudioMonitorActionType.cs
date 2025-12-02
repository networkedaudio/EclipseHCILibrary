namespace HCILibrary.Enums;

/// <summary>
/// Represents the action type for Audio Monitor Actions.
/// </summary>
public enum AudioMonitorActionType : byte
{
    /// <summary>
    /// Stop monitoring audio.
    /// </summary>
    Stop = 0x00,

    /// <summary>
    /// Start monitoring audio.
    /// </summary>
    Start = 0x01,

    /// <summary>
    /// Toggle monitoring audio.
    /// </summary>
    Toggle = 0x02
}
