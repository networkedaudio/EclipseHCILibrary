namespace HCILibrary.Enums;

/// <summary>
/// Key state enumeration (bits 0-1 of DakState).
/// </summary>
public enum KeyState : byte
{
    /// <summary>
    /// Key is off.
    /// </summary>
    Off = 0,

    /// <summary>
    /// Key is on.
    /// </summary>
    On = 1,

    /// <summary>
    /// Key is on (internal use only).
    /// </summary>
    On2 = 2
}
