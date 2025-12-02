namespace HCILibrary.Enums;

/// <summary>
/// Card redundancy state values (Main/Standby).
/// </summary>
public enum CardRedundancyState : byte
{
    /// <summary>
    /// Card is in Main/Active state.
    /// </summary>
    Main = 0,

    /// <summary>
    /// Card is in Standby state.
    /// </summary>
    Standby = 1
}
