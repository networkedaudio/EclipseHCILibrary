namespace HCILibrary.Enums;

/// <summary>
/// Entry action for IFB status elements.
/// </summary>
public enum IfbEntryAction : byte
{
    /// <summary>
    /// Entry was deleted.
    /// </summary>
    Deleted = 0,

    /// <summary>
    /// Entry was added.
    /// </summary>
    Added = 1,

    /// <summary>
    /// Entry was added and is pending.
    /// </summary>
    AddedPending = 2,

    /// <summary>
    /// Entry is present with no change.
    /// </summary>
    Present = 3,

    /// <summary>
    /// Entry was edited.
    /// </summary>
    Edited = 4
}
