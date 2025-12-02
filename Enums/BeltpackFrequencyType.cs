namespace HCILibrary.Enums;

/// <summary>
/// Frequency type for FreeSpeak beltpacks.
/// </summary>
public enum BeltpackFrequencyType : byte
{
    /// <summary>
    /// Frequency not set.
    /// </summary>
    NotSet = 0,

    /// <summary>
    /// 1.9 GHz frequency band (DECT).
    /// </summary>
    Freq1_9Ghz = 1,

    /// <summary>
    /// 2.4 GHz frequency band.
    /// </summary>
    Freq2_4Ghz = 2
}
