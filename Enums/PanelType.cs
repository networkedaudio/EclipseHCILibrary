namespace HCILibrary.Enums;

/// <summary>
/// Panel type enumeration for Reply Panel Status.
/// </summary>
public enum PanelType : byte
{
    /// <summary>
    /// Unknown panel type.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Generic panel.
    /// </summary>
    Generic = 1,

    /// <summary>
    /// FS1 Antenna 1.9 GHz.
    /// </summary>
    Fs1Antenna1_9Ghz = 2,

    /// <summary>
    /// FS2 Antenna 1.9 GHz.
    /// </summary>
    Fs2Antenna1_9Ghz = 3,

    /// <summary>
    /// FS2 Antenna 2.4 GHz.
    /// </summary>
    Fs2Antenna2_4Ghz = 4,

    /// <summary>
    /// Unknown antenna type.
    /// </summary>
    AntennaTypeUnknown = 5,

    /// <summary>
    /// Direct panel (includes hosted directs).
    /// </summary>
    Direct = 6,

    /// <summary>
    /// FS2 Antenna 5.0 GHz.
    /// </summary>
    Fs2Antenna5_0Ghz = 7,

    /// <summary>
    /// AES67 Direct panel.
    /// </summary>
    Aes67Direct = 8
}
