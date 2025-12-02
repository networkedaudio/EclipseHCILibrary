namespace HCILibrary.Enums;

/// <summary>
/// Edit type flags for conference member assignment.
/// </summary>
[Flags]
public enum ConferenceEditFlags : byte
{
    /// <summary>
    /// No flags set.
    /// </summary>
    None = 0x00,

    /// <summary>
    /// Member can talk in the conference.
    /// </summary>
    Talk = 0x01,

    /// <summary>
    /// Member can listen in the conference.
    /// </summary>
    Listen = 0x02,

    /// <summary>
    /// Member has been locally deleted.
    /// </summary>
    LocalDeleted = 0x04,

    /// <summary>
    /// Member has been locally assigned.
    /// </summary>
    LocalAssigned = 0x08,

    /// <summary>
    /// Member has been map assigned.
    /// </summary>
    MapAssigned = 0x10,

    /// <summary>
    /// Member has a local override.
    /// </summary>
    LocalOverride = 0x20
}
