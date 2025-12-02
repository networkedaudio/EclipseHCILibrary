using System.Text;
using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Telephony Client Dial Info Outgoing (HCIv2) - Message ID 0x0127 (295).
/// This message is used to send dial information from the Matrix to the specified
/// SIP server application. There is no reply to this message.
/// </summary>
public class RequestTelephonyClientDialInfoOutgoingRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version.
    /// </summary>
    private const byte ProtocolSchema = 0x01;

    /// <summary>
    /// Size of the dial buffer in bytes.
    /// </summary>
    public const int DialBufferSize = 32;

    /// <summary>
    /// The port number (0-495) on the matrix that has an associated Telephony client
    /// on the Telephony server.
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    /// The dial buffer containing ASCII dial digits.
    /// Maximum 32 characters.
    /// </summary>
    public string DialBuffer { get; set; } = string.Empty;

    /// <summary>
    /// Creates a new Request Telephony Client Dial Info Outgoing request.
    /// </summary>
    public RequestTelephonyClientDialInfoOutgoingRequest()
        : base(HCIMessageID.RequestTelephonyClientDialInfoOutgoing)
    {
    }

    /// <summary>
    /// Creates a new Request Telephony Client Dial Info Outgoing request with specified parameters.
    /// </summary>
    /// <param name="port">The port number (0-495).</param>
    /// <param name="dialBuffer">The dial digits to send (max 32 characters).</param>
    public RequestTelephonyClientDialInfoOutgoingRequest(ushort port, string dialBuffer)
        : base(HCIMessageID.RequestTelephonyClientDialInfoOutgoing)
    {
        Port = port;
        DialBuffer = dialBuffer;
    }

    /// <summary>
    /// Sets the dial buffer.
    /// </summary>
    /// <param name="dialDigits">The dial digits to send.</param>
    /// <returns>This request instance for method chaining.</returns>
    public RequestTelephonyClientDialInfoOutgoingRequest WithDialDigits(string dialDigits)
    {
        DialBuffer = dialDigits;
        return this;
    }

    /// <summary>
    /// Appends digits to the dial buffer.
    /// </summary>
    /// <param name="digits">The digits to append.</param>
    /// <returns>This request instance for method chaining.</returns>
    public RequestTelephonyClientDialInfoOutgoingRequest AppendDigits(string digits)
    {
        DialBuffer += digits;
        return this;
    }

    /// <summary>
    /// Generates the payload for this request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // ProtocolTag(4) + ProtocolSchema(1) + Port(2) + DialBuffer(32)
        var payload = new byte[4 + 1 + 2 + DialBufferSize];

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Port: 2 bytes (big-endian)
        payload[offset++] = (byte)(Port >> 8);
        payload[offset++] = (byte)(Port & 0xFF);

        // Dial Buffer: 32 bytes (ASCII, null-padded)
        string dialData = DialBuffer ?? string.Empty;
        if (dialData.Length > DialBufferSize)
        {
            dialData = dialData.Substring(0, DialBufferSize);
        }

        byte[] dialBytes = Encoding.ASCII.GetBytes(dialData);
        Array.Copy(dialBytes, 0, payload, offset, dialBytes.Length);
        // Remaining bytes are already zero (null-padded)

        return payload;
    }
}
