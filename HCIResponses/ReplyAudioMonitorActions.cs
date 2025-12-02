using System.Net;
using HCILibrary.Enums;
using HCILibrary.HCIRequests;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Audio Monitor Actions (HCIv2) - Message ID 0x0010.
/// This message is sent in reply to Request Audio Monitor Actions.
/// Contains the current state of audio monitors after the action.
/// </summary>
public class ReplyAudioMonitorActions
{
    /// <summary>
    /// The schema version from the message.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// The IP address of the client that issued the HCI request.
    /// </summary>
    public IPAddress? ClientIPAddress { get; set; }

    /// <summary>
    /// The list of audio monitor action entries in the reply.
    /// </summary>
    public List<AudioMonitorAction> Actions { get; } = new();

    /// <summary>
    /// Decodes a Reply Audio Monitor Actions message from payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes to decode.</param>
    /// <returns>A new ReplyAudioMonitorActions instance.</returns>
    public static ReplyAudioMonitorActions Decode(byte[] payload)
    {
        var reply = new ReplyAudioMonitorActions();

        if (payload == null || payload.Length < 12)
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

        // Count (2 bytes, big-endian)
        if (offset + 2 > payload.Length)
        {
            return reply;
        }

        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Client IP Address (4 bytes)
        if (offset + 4 > payload.Length)
        {
            return reply;
        }

        byte[] ipBytes = new byte[4];
        Array.Copy(payload, offset, ipBytes, 0, 4);
        reply.ClientIPAddress = new IPAddress(ipBytes);
        offset += 4;

        // Parse each action entry
        // Each entry: ActionType(2) + ActionData(8) = 10 bytes
        for (int i = 0; i < count && offset + 10 <= payload.Length; i++)
        {
            // Action Type (2 bytes) - should be 0x0100 for audio monitor
            ushort actionType = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Action Data (8 bytes = 4 x 16-bit words)
            if (actionType == 0x0100)
            {
                var action = AudioMonitorAction.FromBytes(payload, offset);
                reply.Actions.Add(action);
            }
            offset += 8;
        }

        return reply;
    }
}
