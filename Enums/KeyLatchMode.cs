namespace HCILibrary.Enums;

/// <summary>
/// Latch mode for key actions.
/// </summary>
public enum KeyLatchMode : byte
{
    /// <summary>
    /// Key will unlatch after release.
    /// </summary>
    LatchNonLatch = 0,

    /// <summary>
    /// Key will always latch after being pressed and released.
    /// </summary>
    Latch = 1,

    /// <summary>
    /// Key will always deselect when released.
    /// </summary>
    NonLatch = 2,

    /// <summary>
    /// Key will toggle select/deselect state when pressed and released,
    /// but if held will return to previous state. Use with Dual Talk and Listen.
    /// </summary>
    ReturnAfterHold = 3
}
