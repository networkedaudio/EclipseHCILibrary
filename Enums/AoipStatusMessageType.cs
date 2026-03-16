namespace HCILibrary.Enums;

/// <summary>
/// Sub-message type within a Reply AoIP Device Status message (0x016B).
/// Each AoIP status message contains one or more sub-messages of these types.
/// </summary>
public enum AoipStatusMessageType : byte
{
    /// <summary>
    /// PTP clock status (master/local IDs, role, lock, priorities, path delay, offset).
    /// </summary>
    PtpStatus = 0,

    /// <summary>
    /// AES67 REST API connectivity status.
    /// </summary>
    Aes67RestApiStatus = 1,

    /// <summary>
    /// IVP (Agent-IC) connection status.
    /// </summary>
    IvpConnectionStatus = 2,

    /// <summary>
    /// DECT synchronisation status.
    /// </summary>
    DectSyncStatus = 3,

    /// <summary>
    /// Device IP address details.
    /// </summary>
    DeviceIpDetails = 4,

    /// <summary>
    /// PTP clock statistics (offset RMS/max, frequency, delay mean/stddev).
    /// </summary>
    PtpStatistics = 5,

    /// <summary>
    /// Wi-Fi channel information.
    /// </summary>
    WifiChannels = 6,

    /// <summary>
    /// PTP clock status for the secondary PTP domain.
    /// </summary>
    PtpStatusSecondary = 7,

    /// <summary>
    /// PTP clock statistics for the secondary PTP domain.
    /// </summary>
    PtpStatisticsSecondary = 8,
}
