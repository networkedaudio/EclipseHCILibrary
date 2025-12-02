namespace HCILibrary.Enums;

/// <summary>
/// Partial NV Clear Options for SRecord transmission initiation.
/// These options specify which local changes should be cleared during a reset.
/// Use bitwise OR to combine multiple options.
/// </summary>
[Flags]
public enum PartialNvClearOptions : ushort
{
    /// <summary>
    /// No options selected.
    /// </summary>
    None = 0,

    /// <summary>
    /// Clear locally held over the air registration (FreeSpeak II).
    /// Bit 10.
    /// </summary>
    ClearOtaRegistration = 1 << 10,

    /// <summary>
    /// Clear locally made conference changes.
    /// Bit 11.
    /// </summary>
    ClearConferenceChanges = 1 << 11,

    /// <summary>
    /// Clear locally made fixed group changes.
    /// Bit 12.
    /// </summary>
    ClearFixedGroupChanges = 1 << 12,

    /// <summary>
    /// Clear locally made level changes.
    /// Bit 13.
    /// </summary>
    ClearLevelChanges = 1 << 13,

    /// <summary>
    /// Clear locally assigned (online) forced listens.
    /// Bit 14.
    /// </summary>
    ClearForcedListens = 1 << 14,

    /// <summary>
    /// Clear locally assigned keys.
    /// Bit 15.
    /// </summary>
    ClearLocallyAssignedKeys = 1 << 15,

    /// <summary>
    /// Clear all local changes (all options combined).
    /// </summary>
    ClearAll = ClearLocallyAssignedKeys | ClearForcedListens | ClearLevelChanges |
               ClearFixedGroupChanges | ClearConferenceChanges | ClearOtaRegistration
}
