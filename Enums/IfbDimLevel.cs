namespace HCILibrary.Enums;

/// <summary>
/// IFB dim level values in dB.
/// </summary>
public enum IfbDimLevel : byte
{
    /// <summary>
    /// 0 dB (no attenuation).
    /// </summary>
    Db0 = 0,

    /// <summary>
    /// -3 dB.
    /// </summary>
    Db3 = 1,

    /// <summary>
    /// -6 dB.
    /// </summary>
    Db6 = 2,

    /// <summary>
    /// -9 dB.
    /// </summary>
    Db9 = 3,

    /// <summary>
    /// -12 dB.
    /// </summary>
    Db12 = 4,

    /// <summary>
    /// -15 dB.
    /// </summary>
    Db15 = 5,

    /// <summary>
    /// -18 dB.
    /// </summary>
    Db18 = 6,

    /// <summary>
    /// -21 dB.
    /// </summary>
    Db21 = 7,

    /// <summary>
    /// -24 dB.
    /// </summary>
    Db24 = 8,

    /// <summary>
    /// -27 dB.
    /// </summary>
    Db27 = 9,

    /// <summary>
    /// -30 dB.
    /// </summary>
    Db30 = 10,

    /// <summary>
    /// -72 dB (effectively off).
    /// </summary>
    Off = 15
}
