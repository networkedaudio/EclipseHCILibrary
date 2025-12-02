namespace HCILibrary.Enums;
/// <summary>
/// Slot type enumeration.
/// </summary>
public enum SlotType : byte
{
    /// <summary>
    /// No slot.
    /// </summary>
    NoSlot = 0,

    /// <summary>
    /// CPU slot.
    /// </summary>
    CpuSlot = 1,

    /// <summary>
    /// DCC slot.
    /// </summary>
    DccSlot = 2,

    /// <summary>
    /// Audio slot.
    /// </summary>
    AudioSlot = 3
}

/// <summary>
/// Card type enumeration for card info.
/// </summary>
public enum CardInfoType : byte
{
    /// <summary>
    /// Unknown card type.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// CPU Master card.
    /// </summary>
    CPUMaster = 1,

    /// <summary>
    /// CPU Slave card.
    /// </summary>
    CPUSecondary = 2,

    /// <summary>
    /// MVX card.
    /// </summary>
    MVX = 3,

    /// <summary>
    /// MADI card.
    /// </summary>
    MADI = 4,

    /// <summary>
    /// Fibre card.
    /// </summary>
    Fiber = 26,

    /// <summary>
    /// EQUE/IVC32/LMC64 card.
    /// </summary>
    EqueIvc32Lmc64 = 27
}

/// <summary>
/// Card health status enumeration.
/// </summary>
public enum CardHealth : byte
{
    /// <summary>
    /// Correct type fitted, initialized and believed working.
    /// </summary>
    Good = 0,

    /// <summary>
    /// Cannot detect any card.
    /// </summary>
    Absent = 1,

    /// <summary>
    /// Correct card believed permanently broken.
    /// </summary>
    Faulty = 2,

    /// <summary>
    /// Card detected but slot ID not yet stable.
    /// </summary>
    Detected = 3,

    /// <summary>
    /// Correct card first/later found in need of initialization.
    /// </summary>
    Waiting = 4,

    /// <summary>
    /// Detected wrong type of card.
    /// </summary>
    Misfit = 5,

    /// <summary>
    /// Correct card only just inserted, considered not yet stable.
    /// </summary>
    PoweringUp = 6,

    /// <summary>
    /// Correct card stable, ready state being checked.
    /// </summary>
    Initialising = 7,

    /// <summary>
    /// Correct card failed some tests but maybe still usable.
    /// </summary>
    Suspect = 8,

    /// <summary>
    /// Not yet tested (only used for FRM channels).
    /// </summary>
    Untested = 9
}
