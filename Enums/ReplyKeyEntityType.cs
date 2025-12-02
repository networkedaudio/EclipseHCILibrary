namespace HCILibrary.Enums;

/// <summary>
/// Extended entity type for reply remote key actions (includes GPIO and Control).
/// </summary>
public enum ReplyKeyEntityType : ushort
{
    /// <summary>
    /// Remove assignment from key.
    /// </summary>
    Null = 0x0000,

    /// <summary>
    /// Port assignment.
    /// </summary>
    Port = 0x0001,

    /// <summary>
    /// Conference assignment.
    /// </summary>
    Conference = 0x0002,

    /// <summary>
    /// Fixed Group assignment.
    /// </summary>
    Group = 0x0003,

    /// <summary>
    /// IFB assignment.
    /// </summary>
    Ifb = 0x0004,

    /// <summary>
    /// GPIO assignment.
    /// </summary>
    Gpio = 0x0005,

    /// <summary>
    /// Control assignment.
    /// </summary>
    Control = 0x0006
}
