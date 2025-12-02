namespace HCILibrary.Enums;

/// <summary>
/// Represents the reason for disconnecting a telephony client call.
/// Used with Request Telephony Client Disconnect Incoming/Outgoing messages.
/// </summary>
public enum TelephonyDisconnectReason : byte
{
    /// <summary>
    /// Reason not specified.
    /// </summary>
    ReasonNotSet = 0x00,

    /// <summary>
    /// Call was terminated by the user.
    /// </summary>
    TerminatedByUser = 0x01,

    /// <summary>
    /// Call was terminated by the server (used for Disconnect Incoming).
    /// </summary>
    TerminatedByServer = 0x02,

    /// <summary>
    /// Call was terminated by the matrix (used for Disconnect Outgoing).
    /// </summary>
    TerminatedByMatrix = 0x02
}
