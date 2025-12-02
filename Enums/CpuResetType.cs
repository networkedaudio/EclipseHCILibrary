namespace HCILibrary.Enums;

/// <summary>
/// CPU Reset type flags.
/// </summary>
[Flags]
public enum CpuResetType : byte
{
    /// <summary>
    /// No reset.
    /// </summary>
    None = 0,

    /// <summary>
    /// Red reset (bit 0).
    /// </summary>
    Red = 1 << 0,

    /// <summary>
    /// Black reset (bit 1).
    /// </summary>
    Black = 1 << 1
}
