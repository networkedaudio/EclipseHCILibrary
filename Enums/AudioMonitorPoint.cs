namespace HCILibrary.Enums;

/// <summary>
/// Represents the audio monitoring point in the signal chain.
/// </summary>
public enum AudioMonitorPoint : byte
{
    /// <summary>
    /// Monitor at the input stage (pre-crosspoint).
    /// </summary>
    Input = 0x00,

    /// <summary>
    /// Monitor at the output stage (post-crosspoint).
    /// </summary>
    Output = 0x01,

    /// <summary>
    /// Monitor at the crosspoint (sum of active crosspoints).
    /// </summary>
    Crosspoint = 0x02
}
