namespace HCILibrary.Enums;

/// <summary>
/// Key event types for panel key actions.
/// </summary>
public enum KeyEvent : byte
{
    /// <summary>
    /// Key press event (key down).
    /// </summary>
    KeyPress = 0,

    /// <summary>
    /// Key release event (key up).
    /// </summary>
    KeyRelease = 1,

    /// <summary>
    /// Key press and release event (latch if possible).
    /// </summary>
    KeyPressRelease = 255
}
