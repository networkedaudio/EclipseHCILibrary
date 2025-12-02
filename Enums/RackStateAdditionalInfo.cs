namespace HCILibrary.Enums;

/// <summary>
/// Additional info for rack state (provides details on failure reasons).
/// </summary>
public enum RackStateAdditionalInfo : ushort
{
    /// <summary>
    /// No additional info set.
    /// </summary>
    NotSet = 0,

    /// <summary>
    /// NID not possible.
    /// </summary>
    NidNotPossible = 1,

    /// <summary>
    /// Hardware is different from expected.
    /// </summary>
    HardwareDifferent = 2,

    /// <summary>
    /// Trunk configuration is different from expected.
    /// </summary>
    TrunksDifferent = 3,

    /// <summary>
    /// Checksum validation failed.
    /// </summary>
    ChecksumFailed = 4,

    /// <summary>
    /// Operation timed out.
    /// </summary>
    TimeoutFail = 5,

    /// <summary>
    /// Format validation failed.
    /// </summary>
    FormatFail = 6
}
