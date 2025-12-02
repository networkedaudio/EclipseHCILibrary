namespace HCILibrary.Enums;

/// <summary>
/// Represents the type of a GPIO/SFO pin.
/// </summary>
public enum GpioPinType : byte
{
    /// <summary>
    /// Output pin.
    /// </summary>
    Output = 0,

    /// <summary>
    /// Input pin.
    /// </summary>
    Input = 1
}
