namespace HCILibrary.Enums;

/// <summary>
/// Online/Offline status for beltpacks.
/// </summary>
public enum BeltpackStatus : byte
{
    /// <summary>
    /// Beltpack is offline (not connected).
    /// </summary>
    Offline = 0,

    /// <summary>
    /// Beltpack is online (connected).
    /// </summary>
    Online = 1
}
