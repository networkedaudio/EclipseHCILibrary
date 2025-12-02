namespace HCILibrary.Enums;

/// <summary>
/// Expansion panel type enumeration.
/// </summary>
public enum ExpansionPanelType : ushort
{
    /// <summary>
    /// Not set.
    /// </summary>
    NotSet = 0,

    /// <summary>
    /// Lever type.
    /// </summary>
    Lever = 1,

    /// <summary>
    /// Push type.
    /// </summary>
    Push = 2,

    /// <summary>
    /// Rotary type.
    /// </summary>
    Rotary = 3,

    /// <summary>
    /// 16 key lever (V32).
    /// </summary>
    Lever16Key = 4
}
