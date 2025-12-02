namespace HCILibrary.Enums;

/// <summary>
/// Port type enumeration (bits 2-7 of operational status).
/// </summary>
public enum PortType : byte
{
    /// <summary>
    /// Null/unknown port type.
    /// </summary>
    Null = 0,

    /// <summary>
    /// Beltpack.
    /// </summary>
    Beltpack = 1,

    /// <summary>
    /// Wireless Port (E-IPA).
    /// </summary>
    WirelessPort = 2,

    /// <summary>
    /// Transceiver (EQue).
    /// </summary>
    Transceiver = 3,

    /// <summary>
    /// EQue Direct.
    /// </summary>
    EQueDirect = 4,

    /// <summary>
    /// Speed Dial.
    /// </summary>
    SpeedDial = 5,

    /// <summary>
    /// FOR-22.
    /// </summary>
    For22 = 6,

    /// <summary>
    /// PABX.
    /// </summary>
    Pabx = 8,

    /// <summary>
    /// Direct.
    /// </summary>
    Direct = 10,

    /// <summary>
    /// EQue Trunk.
    /// </summary>
    EQueTrunk = 11,

    /// <summary>
    /// Panel.
    /// </summary>
    Panel = 14,

    /// <summary>
    /// Trunk (non EQue).
    /// </summary>
    Trunk = 18,

    /// <summary>
    /// Tel-14/SIP.
    /// </summary>
    Tel14Sip = 19,

    /// <summary>
    /// Fibre Trunk.
    /// </summary>
    FibreTrunk = 25,

    /// <summary>
    /// EQue T1 Trunk.
    /// </summary>
    EQueT1Trunk = 27,

    /// <summary>
    /// LMC.
    /// </summary>
    Lmc = 28,

    /// <summary>
    /// CCI-22.
    /// </summary>
    Cci22 = 29,

    /// <summary>
    /// HelixNet.
    /// </summary>
    HelixNet = 30,

    /// <summary>
    /// EQue Direct (T1 E&amp;M, E1 E&amp;M, or T1).
    /// </summary>
    EQueDirectT1E1 = 31
}
