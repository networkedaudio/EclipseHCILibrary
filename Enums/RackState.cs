namespace HCILibrary.Enums;

/// <summary>
/// Rack running state values for Reply Rack Properties: Rack State Get.
/// </summary>
public enum RackState : ushort
{
    /// <summary>
    /// Unknown state.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Rack is ready and operational.
    /// </summary>
    Ready = 1,

    /// <summary>
    /// Rack is currently resetting.
    /// </summary>
    Resetting = 2,

    /// <summary>
    /// Rack is in download mode (receiving configuration/firmware).
    /// </summary>
    Download = 3,

    /// <summary>
    /// Download has completed successfully.
    /// </summary>
    DownloadComplete = 4,

    /// <summary>
    /// Download has failed.
    /// </summary>
    DownloadFailed = 5
}
