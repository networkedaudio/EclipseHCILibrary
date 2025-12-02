namespace HCILibrary.Enums;

/// <summary>
/// Entity type for Request Entity Info.
/// </summary>
public enum EntityInfoType : byte
{
    /// <summary>
    /// Request all entity types (Conferences, Groups, and IFBs).
    /// </summary>
    All = 0,

    /// <summary>
    /// Request conferences only.
    /// </summary>
    Conferences = 2,

    /// <summary>
    /// Request groups only.
    /// </summary>
    Groups = 3,

    /// <summary>
    /// Request IFBs only.
    /// </summary>
    Ifbs = 4
}
