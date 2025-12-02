namespace HCILibrary.Enums;

/// <summary>
/// Telephony key state values for Reply Telephony Key Status messages.
/// </summary>
public enum TelephonyKeyState : byte
{
    /// <summary>
    /// Key is released and not latched.
    /// </summary>
    KeyOff = 0,

    /// <summary>
    /// Key was off, has just been pressed.
    /// </summary>
    KeyOffNowPressed = 1,

    /// <summary>
    /// Key was off, has now been held.
    /// </summary>
    KeyOffNowHeld = 2,

    /// <summary>
    /// Key was on, has just been pressed.
    /// </summary>
    KeyOnNowPressed = 3,

    /// <summary>
    /// Key was on, has now been held.
    /// </summary>
    KeyOnNowHeld = 4,

    /// <summary>
    /// Key was on (event 2), has just been pressed.
    /// </summary>
    KeyOn2NowPressed = 5,

    /// <summary>
    /// Key is latched (event 2), and not currently pressed.
    /// </summary>
    KeyOn2 = 6,

    /// <summary>
    /// Key is latched, and not currently pressed.
    /// </summary>
    KeyOn = 7,

    /// <summary>
    /// This is a mirrored key which is mirroring a key that is currently held.
    /// </summary>
    KeyOnMirroredHeld = 8,

    /// <summary>
    /// Usually the same as KeyOnMirroredHeld except for Dual Talk Listen.
    /// </summary>
    KeyOffMirroredHeld = 9,

    /// <summary>
    /// Key state is unknown.
    /// </summary>
    KeyOffUnknown = 10
}
