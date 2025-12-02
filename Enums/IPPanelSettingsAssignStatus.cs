namespace HCILibrary.Enums;

/// <summary>
/// Status codes for IP Panel Settings Assign reply.
/// </summary>
public enum IPPanelSettingsAssignStatus : sbyte
{
    /// <summary>
    /// Operation completed successfully.
    /// </summary>
    Ok = 0,

    /// <summary>
    /// Length error occurred.
    /// </summary>
    LengthError = -1,

    /// <summary>
    /// Unpack error occurred.
    /// </summary>
    UnpackError = -2
}
