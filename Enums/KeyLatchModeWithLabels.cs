namespace HCILibrary.Enums;

/// <summary>
/// Key latch mode values for key operation.
/// </summary>
public enum KeyLatchModeWithLabels : byte
{
    /// <summary>
    /// Latch/Non-Latch mode.
    /// </summary>
    LatchNonLatch = 0,

    /// <summary>
    /// Latch mode.
    /// </summary>
    Latch = 1,

    /// <summary>
    /// Non-Latch mode.
    /// </summary>
    NonLatch = 2,

    /// <summary>
    /// Return after hold mode.
    /// </summary>
    ReturnAfterHold = 3,

    /// <summary>
    /// Tri-state rotary mode.
    /// </summary>
    TriRot = 4,

    /// <summary>
    /// Tri-state toggle mode.
    /// </summary>
    TriTog = 5,

    /// <summary>
    /// Return after latch mode.
    /// </summary>
    ReturnAfterLatch = 6,

    /// <summary>
    /// Delayed latch mode.
    /// </summary>
    DelLatch = 7,

    /// <summary>
    /// HKS latch mode.
    /// </summary>
    HksLatch = 8
}
