namespace HCILibrary.Enums;

/// <summary>
/// Crosspoint priority values for Request Crosspoint Actions.
/// </summary>
public enum CrosspointPriority : byte
{
    /// <summary>
    /// Normal priority (use for non-local CSU).
    /// </summary>
    Normal = 1,

    /// <summary>
    /// Local CSU priority level 2.
    /// </summary>
    LocalCSU2 = 2,

    /// <summary>
    /// Local CSU priority level 3.
    /// </summary>
    LocalCSU3 = 3
}
