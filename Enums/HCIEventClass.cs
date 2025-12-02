namespace HCILibrary.Enums;

/// <summary>
/// Event class enumeration for HCI Event messages.
/// </summary>
public enum HCIEventClass : ushort
{
    /// <summary>
    /// Fatal error event.
    /// </summary>
    FatalError = 0,

    /// <summary>
    /// Non-fatal error event.
    /// </summary>
    NonFatalError = 1,

    /// <summary>
    /// Warning event.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Information event.
    /// </summary>
    Information = 3,

    /// <summary>
    /// Debug event.
    /// </summary>
    Debug = 4,

    /// <summary>
    /// Log to disk event.
    /// </summary>
    LogToDisk = 5

    // 6-65535 are reserved.
}
