namespace HCILibrary.Enums;

/// <summary>
/// Apply type for SRecord transmission initiation.
/// Specifies how the matrix configuration should be applied.
/// </summary>
public enum SRecordApplyType : byte
{
    /// <summary>
    /// Apply with Reset (Red reset).
    /// Configuration is applied and the matrix is reset.
    /// </summary>
    ApplyWithReset = 1,

    /// <summary>
    /// Apply Changes (no reset).
    /// Configuration changes are applied without resetting the matrix.
    /// </summary>
    ApplyChanges = 2,

    /// <summary>
    /// Apply with Reset and NVRAM clear (Black reset).
    /// Configuration is applied, NVRAM is cleared, and the matrix is reset.
    /// </summary>
    ApplyWithResetAndNvramClear = 9
}
