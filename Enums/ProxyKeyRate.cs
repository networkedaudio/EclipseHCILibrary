namespace HCILibrary.Enums;

/// <summary>
/// Proxy key LED flash rate.
/// </summary>
public enum ProxyKeyRate : byte
{
    /// <summary>
    /// LED is off.
    /// </summary>
    Off = 0,

    /// <summary>
    /// Flash rate of 1 Hz.
    /// </summary>
    Flash1Hz = 1,

    /// <summary>
    /// Flash rate of 2 Hz.
    /// </summary>
    Flash2Hz = 2,

    /// <summary>
    /// Flash rate of 4 Hz.
    /// </summary>
    Flash4Hz = 3,

    /// <summary>
    /// LED is always on (no flashing).
    /// </summary>
    AlwaysOn = 15
}
