namespace HCILibrary.Enums;

/// <summary>
/// Card type for EHX Control Card Status.
/// </summary>
public enum EhxCardType : byte
{
    /// <summary>
    /// GPIO card.
    /// </summary>
    Gpio = 0,

    /// <summary>
    /// SFO card.
    /// </summary>
    Sfo = 1
}
