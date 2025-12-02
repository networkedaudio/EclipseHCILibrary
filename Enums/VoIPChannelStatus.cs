namespace HCILibrary.Enums;

/// <summary>
/// VoIP channel status.
/// </summary>
public enum VoIPChannelStatus : byte
{
    /// <summary>
    /// Status is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Channel is up.
    /// </summary>
    Up = 1,

    /// <summary>
    /// Channel is down.
    /// </summary>
    Down = 2
}
