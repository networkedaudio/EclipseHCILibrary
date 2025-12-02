using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Telephony Client Disconnect Incoming (HCIv2) - Message ID 0x0123 (291).
/// This message is used to acknowledge the incoming disconnect request from the
/// associated Telephony Server.
/// </summary>
public class ReplyTelephonyClientDisconnect
{
    /// <summary>
    /// The schema version from the message.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// The port number (0-495) on the matrix that has an associated Telephony client
    /// on the Telephony server.
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    /// Indicates whether the disconnect was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Decodes a Reply Telephony Client Disconnect message from payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes to decode.</param>
    /// <returns>A new ReplyTelephonyClientDisconnect instance.</returns>
    public static ReplyTelephonyClientDisconnect Decode(byte[] payload)
    {
        var reply = new ReplyTelephonyClientDisconnect();

        if (payload == null || payload.Length < 8)
        {
            return reply;
        }

        int offset = 0;

        // Check for HCIv2 marker (0xAB 0xBA 0xCE 0xDE)
        if (payload[0] == 0xAB && payload[1] == 0xBA &&
            payload[2] == 0xCE && payload[3] == 0xDE)
        {
            offset = 4;
        }

        // Schema byte
        if (offset < payload.Length)
        {
            reply.Schema = payload[offset++];
        }

        // Port (2 bytes, big-endian)
        if (offset + 2 <= payload.Length)
        {
            reply.Port = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Success (1 byte)
        if (offset < payload.Length)
        {
            reply.Success = payload[offset] != 0;
        }

        return reply;
    }
}
