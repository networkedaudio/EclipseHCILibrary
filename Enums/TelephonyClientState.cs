namespace HCILibrary.Enums;

/// <summary>
/// Represents the state of a telephony client port.
/// </summary>
public enum TelephonyClientState : byte
{
    /// <summary>
    /// On-hook (idle, no call).
    /// </summary>
    OnHook = 0x00,

    /// <summary>
    /// On-hook but allocated for a call.
    /// </summary>
    OnHookAllocated = 0x01,

    /// <summary>
    /// Connecting outgoing call (dialing).
    /// </summary>
    ConnectingOut = 0x02,

    /// <summary>
    /// Connected outgoing call (active outbound call).
    /// </summary>
    ConnectedOut = 0x03,

    /// <summary>
    /// Connecting incoming call (ringing).
    /// </summary>
    ConnectingIn = 0x04,

    /// <summary>
    /// Connected incoming call (active inbound call).
    /// </summary>
    ConnectedIn = 0x05
}
