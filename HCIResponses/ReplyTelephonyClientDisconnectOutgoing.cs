using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Telephony Client Disconnect Outgoing (HCIv2) - Message ID 0x0126 (294).
/// This message is used to acknowledge the disconnection of the call associated
/// with the specified port.
/// </summary>
public class ReplyTelephonyClientDisconnectOutgoing
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
    /// Decodes a Reply Telephony Client Disconnect Outgoing message from payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes to decode.</param>
    /// <returns>A new ReplyTelephonyClientDisconnectOutgoing instance.</returns>
    public static ReplyTelephonyClientDisconnectOutgoing Decode(byte[] payload)
    {
        var reply = new ReplyTelephonyClientDisconnectOutgoing();

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
