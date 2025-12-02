namespace HCILibrary.Enums;

/// <summary>
/// PSU (Power Supply Unit) and frame status flags for Reply Frame Status.
/// Only functional on the HX Delta.
/// </summary>
[Flags]
public enum FramePsuStatus : ushort
{
    /// <summary>
    /// No failures or alarms.
    /// </summary>
    None = 0x0000,

    /// <summary>
    /// External PSU 1 failure.
    /// </summary>
    ExtPsu1Fail = 0x0001,

    /// <summary>
    /// External PSU 2 failure.
    /// </summary>
    ExtPsu2Fail = 0x0002,

    /// <summary>
    /// Internal PSU 1 failure.
    /// </summary>
    IntPsu1Fail = 0x0004,

    /// <summary>
    /// Internal PSU 2 failure.
    /// </summary>
    IntPsu2Fail = 0x0008,

    /// <summary>
    /// Fan 1 failure.
    /// </summary>
    Fan1Fail = 0x0010,

    /// <summary>
    /// Fan 2 failure.
    /// </summary>
    Fan2Fail = 0x0020,

    /// <summary>
    /// Configuration failure. Fired by CPU App, e.g., when master/slave switchover occurs.
    /// </summary>
    ConfigFail = 0x0040,

    /// <summary>
    /// External alarm GPO state. Activated when core alarm bits are high.
    /// </summary>
    ExtAlarm = 0x0080,

    /// <summary>
    /// Over-temperature condition.
    /// </summary>
    Overtemp = 0x0100
}
