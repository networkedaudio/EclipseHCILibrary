using HCILibrary.Enums;

namespace HCILibrary.Models;

/// <summary>
/// Base class for HCI request messages.
/// Inherit from this class to create specific request types.
/// </summary>
public class HCIRequest
{
    /// <summary>
    /// Start marker for HCI messages.
    /// </summary>
    protected static readonly byte[] StartMarker = { 0x5A, 0x0F };

    /// <summary>
    /// End marker for HCI messages.
    /// </summary>
    protected static readonly byte[] EndMarker = { 0x2E, 0x8D };

    /// <summary>
    /// The message ID for this request.
    /// </summary>
    public HCIMessageID MessageID { get; protected set; }

    /// <summary>
    /// Optional expected response message ID.
    /// If set, the system will wait for a reply with this message ID.
    /// </summary>
    public HCIMessageID? ExpectedReplyMessageID { get; set; }

    /// <summary>
    /// Timestamp when the request was created.
    /// Used for ordering urgent messages.
    /// </summary>
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    /// <summary>
    /// Indicates if this request is urgent and should be prioritized in the queue.
    /// </summary>
    public bool IsUrgent { get; set; }

    /// <summary>
    /// Task completion source for async request/response pattern.
    /// </summary>
    internal TaskCompletionSource<HCIReply?>? ResponseCompletionSource { get; set; }

    /// <summary>
    /// Creates a new HCI request with the specified message ID.
    /// </summary>
    /// <param name="messageId">The message ID for this request.</param>
    public HCIRequest(HCIMessageID messageId)
    {
        MessageID = messageId;
    }

    /// <summary>
    /// Override this method to generate the payload for this request.
    /// </summary>
    /// <returns>The payload bytes for this request.</returns>
    protected virtual byte[] GeneratePayload()
    {
        return Array.Empty<byte>();
    }

    /// <summary>
    /// Builds the complete message bytes including start/end markers and length.
    /// </summary>
    /// <returns>The complete message as a byte array.</returns>
    public virtual byte[] BuildMessage()
    {
        var payload = GeneratePayload();
        
        // Calculate total length (excluding start marker but including length bytes and end marker)
        // Length = 2 (length) + 2 (message ID) + 1 (flags) + payload + 2 (end marker)
        ushort length = (ushort)(2 + 2 + 1 + payload.Length + 2);
        
        using var ms = new MemoryStream();
        
        // Start marker
        ms.Write(StartMarker, 0, StartMarker.Length);
        
        // Length (big-endian)
        ms.WriteByte((byte)(length >> 8));
        ms.WriteByte((byte)(length & 0xFF));
        
        // Message ID (big-endian)
        ms.WriteByte((byte)((ushort)MessageID >> 8));
        ms.WriteByte((byte)((ushort)MessageID & 0xFF));
        
        // Flags (default to 0)
        ms.WriteByte(0x00);
        
        // Payload
        if (payload.Length > 0)
        {
            ms.Write(payload, 0, payload.Length);
        }
        
        // End marker
        ms.Write(EndMarker, 0, EndMarker.Length);
        
        return ms.ToArray();
    }
}
