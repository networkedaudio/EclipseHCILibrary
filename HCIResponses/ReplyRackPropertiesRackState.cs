using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Rack Properties: Rack State Get (Message ID 0x002C, Sub Message ID 9).
/// Contains the current and previous rack running state.
/// </summary>
public class ReplyRackPropertiesRackState
{
    /// <summary>
    /// Gets or sets the protocol schema version.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// Gets or sets the Sub Message ID.
    /// Should be <see cref="RackPropertySubMessageId.RackStateGetReply"/> (9).
    /// </summary>
    public RackPropertySubMessageId SubMessageId { get; set; }

    /// <summary>
    /// Gets or sets the current rack state.
    /// </summary>
    public RackState CurrentState { get; set; }

    /// <summary>
    /// Gets or sets the additional info for the current state.
    /// Provides details on failure reasons.
    /// </summary>
    public RackStateAdditionalInfo CurrentStateAdditionalInfo { get; set; }

    /// <summary>
    /// Gets or sets the previous rack state.
    /// </summary>
    public RackState PreviousState { get; set; }

    /// <summary>
    /// Gets or sets the additional info for the previous state.
    /// Provides details on failure reasons.
    /// </summary>
    public RackStateAdditionalInfo PreviousStateAdditionalInfo { get; set; }

    /// <summary>
    /// Gets whether the rack is ready and operational.
    /// </summary>
    public bool IsReady => CurrentState == RackState.Ready;

    /// <summary>
    /// Gets whether a download is in progress.
    /// </summary>
    public bool IsDownloading => CurrentState == RackState.Download;

    /// <summary>
    /// Gets whether the last download completed successfully.
    /// </summary>
    public bool IsDownloadComplete => CurrentState == RackState.DownloadComplete;

    /// <summary>
    /// Gets whether the last download failed.
    /// </summary>
    public bool IsDownloadFailed => CurrentState == RackState.DownloadFailed;

    /// <summary>
    /// Gets whether the rack is currently resetting.
    /// </summary>
    public bool IsResetting => CurrentState == RackState.Resetting;

    /// <summary>
    /// Decodes a Reply Rack Properties: Rack State Get message from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes.</param>
    /// <returns>The decoded ReplyRackPropertiesRackState.</returns>
    public static ReplyRackPropertiesRackState Decode(byte[] payload)
    {
        var result = new ReplyRackPropertiesRackState();

        if (payload == null || payload.Length < 1)
        {
            return result;
        }

        int offset = 0;

        // Check for HCIv2 marker (0xAB 0xBA 0xCE 0xDE)
        if (payload.Length >= 4 &&
            payload[0] == 0xAB && payload[1] == 0xBA &&
            payload[2] == 0xCE && payload[3] == 0xDE)
        {
            offset = 4;

            // Read schema byte
            if (offset < payload.Length)
            {
                result.Schema = payload[offset++];
            }
        }

        // Sub Message ID (2 bytes, big-endian)
        if (offset + 2 > payload.Length)
        {
            return result;
        }

        result.SubMessageId = (RackPropertySubMessageId)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Current State (2 bytes, big-endian)
        if (offset + 2 <= payload.Length)
        {
            result.CurrentState = (RackState)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Current State Additional Info (2 bytes, big-endian)
        if (offset + 2 <= payload.Length)
        {
            result.CurrentStateAdditionalInfo = (RackStateAdditionalInfo)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Previous State (2 bytes, big-endian)
        if (offset + 2 <= payload.Length)
        {
            result.PreviousState = (RackState)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Previous State Additional Info (2 bytes, big-endian)
        if (offset + 2 <= payload.Length)
        {
            result.PreviousStateAdditionalInfo = (RackStateAdditionalInfo)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        return result;
    }

    /// <summary>
    /// Returns a string representation of this rack state reply.
    /// </summary>
    public override string ToString()
    {
        string current = CurrentStateAdditionalInfo != RackStateAdditionalInfo.NotSet
            ? $"{CurrentState} ({CurrentStateAdditionalInfo})"
            : CurrentState.ToString();

        string previous = PreviousStateAdditionalInfo != RackStateAdditionalInfo.NotSet
            ? $"{PreviousState} ({PreviousStateAdditionalInfo})"
            : PreviousState.ToString();

        return $"Rack State: Current={current}, Previous={previous}";
    }
}
