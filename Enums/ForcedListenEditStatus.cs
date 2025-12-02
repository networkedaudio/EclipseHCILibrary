namespace HCILibrary.Enums;

/// <summary>
/// Edit status for forced listen edits.
/// </summary>
public enum ForcedListenEditStatus : byte
{
    /// <summary>
    /// Forced listen was deleted.
    /// </summary>
    Deleted = 0,

    /// <summary>
    /// Forced listen was added.
    /// </summary>
    Added = 1
}
