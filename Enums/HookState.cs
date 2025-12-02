namespace HCILibrary.Enums;

/// <summary>
/// Hook state for telephone hybrid devices (TEL-14, LQ-SIP).
/// </summary>
public enum HookState : byte
{
    /// <summary>
    /// On hook - remote telephone line release.
    /// </summary>
    OnHook = 0,

    /// <summary>
    /// Off hook - line connected.
    /// </summary>
    OffHook = 1
}
