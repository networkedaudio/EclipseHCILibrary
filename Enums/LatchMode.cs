namespace HCILibrary.Enums;

/// <summary>
/// Latch mode for key operation.
/// </summary>
public enum LatchMode : byte
{
    /// <summary>
    /// Non-latching mode.
    /// </summary>
    NonLatch = 0,

    /// <summary>
    /// Latching mode.
    /// </summary>
    Latch = 1,

    /// <summary>
    /// Non-latching mode (explicit).
    /// </summary>
    NonLatchExplicit = 2,

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
