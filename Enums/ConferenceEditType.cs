namespace HCILibrary.Enums;

/// <summary>
/// Edit type for Conference / Fixed Group Members Edits request.
/// </summary>
public enum ConferenceEditType : ushort
{
    /// <summary>
    /// Conference (partyline) edit type.
    /// </summary>
    Conference = 2,

    /// <summary>
    /// Fixed group edit type.
    /// </summary>
    Group = 3
}
