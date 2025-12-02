namespace HCILibrary.Enums;

/// <summary>
/// Proxy key LED colour and brightness flags.
/// These flags can be combined (e.g., Bright | Red | Green for bright yellow).
/// </summary>
[Flags]
public enum ProxyKeyColour : byte
{
    /// <summary>
    /// No colour set.
    /// </summary>
    None = 0x00,

    /// <summary>
    /// Bright mode (when not set, LED is dim).
    /// </summary>
    Bright = 0x01,

    /// <summary>
    /// Red LED enabled.
    /// </summary>
    Red = 0x02,

    /// <summary>
    /// Green LED enabled.
    /// </summary>
    Green = 0x04,

    /// <summary>
    /// Blue LED enabled.
    /// </summary>
    Blue = 0x08
}
