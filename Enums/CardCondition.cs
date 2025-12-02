namespace HCILibrary.Enums;

/// <summary>
/// Condition code for a card in the matrix.
/// </summary>
public enum CardCondition : byte
{
    /// <summary>
    /// Unknown condition.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Card is good/operational.
    /// </summary>
    Good = 1,

    /// <summary>
    /// Card is faulty or absent.
    /// </summary>
    FaultyOrAbsent = 2
}
