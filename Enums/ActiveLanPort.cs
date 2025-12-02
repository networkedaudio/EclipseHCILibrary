namespace HCILibrary.Enums;

/// <summary>
/// Active LAN port values for network redundancy status.
/// </summary>
public enum ActiveLanPort : byte
{
    /// <summary>
    /// Active LAN port is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Main LAN port is active.
    /// </summary>
    Main = 1,

    /// <summary>
    /// Backup LAN port is active.
    /// </summary>
    Backup = 2
}
