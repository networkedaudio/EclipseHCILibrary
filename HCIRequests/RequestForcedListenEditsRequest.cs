using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Forced Listen Edits (0x00C9).
/// Requests all forced listen edits.
/// </summary>
public class RequestForcedListenEditsRequest : HCIRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestForcedListenEditsRequest"/> class.
    /// </summary>
    public RequestForcedListenEditsRequest()
        : base(HCIMessageID.RequestForcedListenEdits)
    {
    }

    /// <summary>
    /// Generates the payload for the Request Forced Listen Edits message.
    /// </summary>
    /// <returns>An empty payload as this request has no additional data.</returns>
    protected override byte[] GeneratePayload()
    {
        // No payload beyond standard HCIv2 header
        return Array.Empty<byte>();
    }
}
