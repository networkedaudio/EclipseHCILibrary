using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Telephony Client Disconnect Outgoing (HCIv2) - Message ID 0x0125 (293).
/// This message is used by the matrix to request the disconnection of a Telephony call
/// that is currently connected via a third party SIP server switch.
/// </summary>
public class RequestTelephonyClientDisconnectOutgoingRequest : HCIRequest
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
    /// The port number (0-495) on the matrix that has an associated Telephony client
    /// on the Telephony server.
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    /// The reason for disconnecting the call.
    /// </summary>
    public TelephonyDisconnectReason Reason { get; set; } = TelephonyDisconnectReason.ReasonNotSet;

    /// <summary>
    /// Creates a new Request Telephony Client Disconnect Outgoing request.
    /// </summary>
    public RequestTelephonyClientDisconnectOutgoingRequest()
        : base(HCIMessageID.RequestTelephonyClientDisconnectOutgoing)
    {
    }

    /// <summary>
    /// Creates a new Request Telephony Client Disconnect Outgoing request for a specific port.
    /// </summary>
    /// <param name="port">The port number (0-495).</param>
    /// <param name="reason">The reason for disconnecting the call.</param>
    public RequestTelephonyClientDisconnectOutgoingRequest(ushort port,
        TelephonyDisconnectReason reason = TelephonyDisconnectReason.TerminatedByMatrix)
        : base(HCIMessageID.RequestTelephonyClientDisconnectOutgoing)
    {
        Port = port;
        Reason = reason;
    }

    /// <summary>
    /// Creates a disconnect request terminated by user.
    /// </summary>
    /// <param name="port">The port number (0-495).</param>
    /// <returns>A new request instance.</returns>
    public static RequestTelephonyClientDisconnectOutgoingRequest ByUser(ushort port)
    {
        return new RequestTelephonyClientDisconnectOutgoingRequest(port, TelephonyDisconnectReason.TerminatedByUser);
    }

    /// <summary>
    /// Creates a disconnect request terminated by matrix.
    /// </summary>
    /// <param name="port">The port number (0-495).</param>
    /// <returns>A new request instance.</returns>
    public static RequestTelephonyClientDisconnectOutgoingRequest ByMatrix(ushort port)
    {
        return new RequestTelephonyClientDisconnectOutgoingRequest(port, TelephonyDisconnectReason.TerminatedByMatrix);
    }

    /// <summary>
    /// Generates the payload for this request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // ProtocolTag(4) + ProtocolSchema(1) + Port(2) + Reason(1)
        var payload = new byte[4 + 1 + 2 + 1];

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Port: 2 bytes (big-endian)
        payload[offset++] = (byte)(Port >> 8);
        payload[offset++] = (byte)(Port & 0xFF);

        // Reason: 1 byte
        payload[offset++] = (byte)Reason;

        return payload;
    }
}
