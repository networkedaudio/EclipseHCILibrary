using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Rack Properties: Active Configuration Bank (0x002C).
/// Requests information about the currently active configuration bank.
/// HCIv2 only.
/// </summary>
public class RequestRackPropertiesConfigBankRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Gets or sets the protocol schema version. Currently set to 1.
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// Gets the Sub Message ID for this request.
    /// </summary>
    public RackPropertySubMessageId SubMessageId => RackPropertySubMessageId.ConfigBankRequest;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestRackPropertiesConfigBankRequest"/> class.
    /// </summary>
    public RequestRackPropertiesConfigBankRequest()
        : base(HCIMessageID.RequestRackProperties)
    {
    }

    /// <summary>
    /// Generates the payload for the Request Rack Properties: Active Configuration Bank message.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // [0-3]: HCIv2 Protocol Tag (0xAB 0xBA 0xCE 0xDE)
        // [4]:   Protocol Schema (1 byte)
        // [5-6]: Sub Message ID (2 bytes, big-endian) - 4 for Config Bank Request

        const int totalSize = 4 + 1 + 2; // Tag + Schema + SubMsgID = 7

        var payload = new byte[totalSize];
        int offset = 0;

        // HCIv2 protocol tag
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol schema
        payload[offset++] = ProtocolSchema;

        // Sub Message ID (2 bytes, big-endian)
        ushort subMsgId = (ushort)SubMessageId;
        payload[offset++] = (byte)((subMsgId >> 8) & 0xFF);
        payload[offset++] = (byte)(subMsgId & 0xFF);

        return payload;
    }
}
