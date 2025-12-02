namespace HCILibrary.Enums;

/// <summary>
/// Card type enumeration for cards in the matrix.
/// </summary>
public enum CardType : byte
{
    /// <summary>
    /// Unknown card type.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// CPU Master card.
    /// </summary>
    CpuMaster = 1,

    /// <summary>
    /// CPU Slave card.
    /// </summary>
    CpuSlave = 2,

    /// <summary>
    /// MVX card.
    /// </summary>
    Mvx = 3,

    /// <summary>
    /// E-MADI card.
    /// </summary>
    EMadi = 4,

    /// <summary>
    /// E-DANTE card.
    /// </summary>
    EDante = 5,

    /// <summary>
    /// E-IPA card.
    /// </summary>
    EIpa = 6,

    /// <summary>
    /// PCM30 card.
    /// </summary>
    Pcm30 = 23,

    /// <summary>
    /// E-FIB card.
    /// </summary>
    EFib = 26,

    /// <summary>
    /// EQUE/IVC32/LMC64 card.
    /// </summary>
    EquIvc32Lmc64 = 27
}
