namespace HCILibrary.Enums;

/// <summary>
/// Radio mode for key action.
/// </summary>
public enum RadioMode : byte
{
    /// <summary>
    /// Frequency mode.
    /// </summary>
    Freq = 0,

    /// <summary>
    /// TX select mode.
    /// </summary>
    TxSel = 1,

    /// <summary>
    /// RX select mode.
    /// </summary>
    RxSel = 2,

    /// <summary>
    /// TX master/slave mode.
    /// </summary>
    TxMasterSlave = 3,

    /// <summary>
    /// RX master/slave mode.
    /// </summary>
    RxMasterSlave = 4
}
