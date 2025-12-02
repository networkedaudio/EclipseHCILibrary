using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Set System Time (0x0044).
/// Sets the current system time on the config card using a 32-bit UTC time_t value.
/// Only one of these messages will be accepted per config card boot to prevent
/// multiple HCI clients from repeatedly setting the system time to different values.
/// There is no reply sent to this message.
/// HCIv2 only.
/// </summary>
public class RequestSetSystemTimeRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Unix epoch (January 1, 1970 00:00:00 UTC).
    /// </summary>
    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets or sets the protocol schema version. Currently set to 1.
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// Gets or sets the system time as a 32-bit Unix timestamp (seconds since January 1, 1970 UTC).
    /// </summary>
    public uint UnixTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the system time as a DateTime (UTC).
    /// Setting this property will update the UnixTimestamp property.
    /// </summary>
    public DateTime SystemTimeUtc
    {
        get => UnixEpoch.AddSeconds(UnixTimestamp);
        set => UnixTimestamp = (uint)(value.ToUniversalTime() - UnixEpoch).TotalSeconds;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestSetSystemTimeRequest"/> class.
    /// </summary>
    public RequestSetSystemTimeRequest()
        : base(HCIMessageID.RequestSetSystemTime)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestSetSystemTimeRequest"/> class
    /// with the specified Unix timestamp.
    /// </summary>
    /// <param name="unixTimestamp">The system time as a 32-bit Unix timestamp.</param>
    public RequestSetSystemTimeRequest(uint unixTimestamp)
        : base(HCIMessageID.RequestSetSystemTime)
    {
        UnixTimestamp = unixTimestamp;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestSetSystemTimeRequest"/> class
    /// with the specified DateTime (UTC).
    /// </summary>
    /// <param name="systemTimeUtc">The system time as a DateTime (will be converted to UTC).</param>
    public RequestSetSystemTimeRequest(DateTime systemTimeUtc)
        : base(HCIMessageID.RequestSetSystemTime)
    {
        SystemTimeUtc = systemTimeUtc;
    }

    /// <summary>
    /// Creates a request with the current system time.
    /// </summary>
    /// <returns>A new RequestSetSystemTimeRequest with the current UTC time.</returns>
    public static RequestSetSystemTimeRequest CreateWithCurrentTime()
    {
        return new RequestSetSystemTimeRequest(DateTime.UtcNow);
    }

    /// <summary>
    /// Generates the payload for the Request Set System Time message.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // Protocol Tag: 4 bytes (0xABBACEDE)
        // Protocol Schema: 1 byte
        // UTC time_t: 4 bytes (big-endian)

        var payload = new byte[9];
        int offset = 0;

        // Protocol Tag (4 bytes)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema (1 byte)
        payload[offset++] = ProtocolSchema;

        // UTC time_t (4 bytes, big-endian)
        payload[offset++] = (byte)((UnixTimestamp >> 24) & 0xFF);
        payload[offset++] = (byte)((UnixTimestamp >> 16) & 0xFF);
        payload[offset++] = (byte)((UnixTimestamp >> 8) & 0xFF);
        payload[offset++] = (byte)(UnixTimestamp & 0xFF);

        return payload;
    }
}
