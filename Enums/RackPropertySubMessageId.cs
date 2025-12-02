namespace HCILibrary.Enums;

/// <summary>
/// Sub Message ID types for Rack Properties messages.
/// </summary>
public enum RackPropertySubMessageId : ushort
{
    /// <summary>
    /// Request the currently active configuration bank.
    /// </summary>
    ConfigBankRequest = 4,

    /// <summary>
    /// Reply with the currently active configuration bank.
    /// </summary>
    ConfigBankReply = 5,

    /// <summary>
    /// Request the current rack running state.
    /// </summary>
    RackStateGetRequest = 8,

    /// <summary>
    /// Reply with the current rack running state.
    /// </summary>
    RackStateGetReply = 9,

    /// <summary>
    /// Request the configuration status from the matrix.
    /// </summary>
    RackConfigurationStatusRequest = 12,

    /// <summary>
    /// Reply with the configuration status from the matrix.
    /// </summary>
    RackConfigurationStatusReply = 13
}
