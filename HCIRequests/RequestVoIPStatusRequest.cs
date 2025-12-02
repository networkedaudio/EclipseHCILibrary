using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request VoIP Status (0x00F8).
/// Requests VoIP Status for all IVC32 channels on all IVC32 and E-IPA cards.
/// On the IVC32/E-IPA card a channel equates to a port.
/// Supported VoIP ports are IVP Panels, Agent-IC, LQ, Trunks, directs and hosted panel directs.
/// </summary>
public class RequestVoIPStatusRequest : HCIRequest
{
    /// <summary>
    /// Sub message ID for VoIP status request.
    /// </summary>
    public const byte SubMessageId = 0x16;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestVoIPStatusRequest"/> class.
    /// </summary>
    public RequestVoIPStatusRequest()
        : base(HCIMessageID.RequestVoIPStatus)
    {
    }

    /// <summary>
    /// Generates the payload for the Request VoIP Status message.
    /// </summary>
    /// <returns>The message payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload: SubMessageId(1) = 1 byte
        return new byte[] { SubMessageId };
    }
}
