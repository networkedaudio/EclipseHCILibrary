namespace HCILibrary.Enums;

/// <summary>
/// Call end reason enumeration for VoIP channels.
/// </summary>
public enum CallEndReason : byte
{
    /// <summary>
    /// Call ended by local user.
    /// </summary>
    ByLocalUser = 0,

    /// <summary>
    /// No accept received.
    /// </summary>
    NoAccept = 1,

    /// <summary>
    /// Answer was denied.
    /// </summary>
    AnswerDenied = 2,

    /// <summary>
    /// Call ended by remote user.
    /// </summary>
    RemoteUser = 3,

    /// <summary>
    /// Call was refused.
    /// </summary>
    Refusal = 4,

    /// <summary>
    /// No answer from remote.
    /// </summary>
    NoAnswer = 5,

    /// <summary>
    /// Caller aborted the call.
    /// </summary>
    CallerAbort = 6,

    /// <summary>
    /// Transport failure.
    /// </summary>
    TransportFail = 7,

    /// <summary>
    /// Connection failure.
    /// </summary>
    ConnectFail = 8,

    /// <summary>
    /// Gatekeeper error.
    /// </summary>
    Gatekeeper = 9,

    /// <summary>
    /// No user found.
    /// </summary>
    NoUser = 10,

    /// <summary>
    /// No bandwidth available.
    /// </summary>
    NoBandwidth = 11,

    /// <summary>
    /// Capability exchange failure.
    /// </summary>
    CapabilityExchange = 12,

    /// <summary>
    /// Call was forwarded.
    /// </summary>
    CallForwarded = 13,

    /// <summary>
    /// Security denial.
    /// </summary>
    SecurityDenial = 14,

    /// <summary>
    /// Local is busy.
    /// </summary>
    LocalBusy = 15,

    /// <summary>
    /// Local congestion.
    /// </summary>
    LocalCongestion = 16,

    /// <summary>
    /// Remote is busy.
    /// </summary>
    RemoteBusy = 17,

    /// <summary>
    /// Remote congestion.
    /// </summary>
    RemoteCongestion = 18,

    /// <summary>
    /// Unreachable.
    /// </summary>
    Unreachable = 19,

    /// <summary>
    /// No endpoint found.
    /// </summary>
    NoEndPoint = 20,

    /// <summary>
    /// Remote host is offline.
    /// </summary>
    RemoteHostOffline = 21,

    /// <summary>
    /// Temporary failure.
    /// </summary>
    TempFailure = 22,

    /// <summary>
    /// Q.931 cause.
    /// </summary>
    Q931Cause = 23,

    /// <summary>
    /// Duration limit reached.
    /// </summary>
    DurationLimit = 24,

    /// <summary>
    /// Invalid conference ID.
    /// </summary>
    InvalidConferenceId = 25,

    /// <summary>
    /// Connection timeout.
    /// </summary>
    ConnectionTimeout = 26,

    /// <summary>
    /// Reason not set (channel is not down).
    /// </summary>
    NotSet = 27
}
