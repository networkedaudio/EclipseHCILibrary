using System.Net;
using System.Text;
using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Telephony Client Get State (HCIv2) - Message ID 0x011E (286).
/// Response to Request Telephony Client Get State containing the state of telephony client ports.
/// Telephony client ports supported include TEL-14 and SIP client types.
/// </summary>
public class ReplyTelephonyClientState
{
    /// <summary>
    /// The schema version from the message.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// The list of telephony client state entries.
    /// </summary>
    public List<TelephonyClientStateEntry> Entries { get; } = new();

    /// <summary>
    /// Decodes a Reply Telephony Client State message from payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes to decode.</param>
    /// <returns>A new ReplyTelephonyClientState instance.</returns>
    public static ReplyTelephonyClientState Decode(byte[] payload)
    {
        var reply = new ReplyTelephonyClientState();

        if (payload == null || payload.Length < 7)
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

        // Parse each telephony client state entry
        // Each entry: Port(2) + Status(1) + CallInfo(32) = 35 bytes
        for (int i = 0; i < count && offset + TelephonyClientStateEntry.EntrySize <= payload.Length; i++)
        {
            var entry = TelephonyClientStateEntry.FromBytes(payload, offset);
            reply.Entries.Add(entry);
            offset += TelephonyClientStateEntry.EntrySize;
        }

        return reply;
    }
}

/// <summary>
/// Represents a single telephony client state entry in the reply.
/// </summary>
public class TelephonyClientStateEntry
{
    /// <summary>
    /// Size of each entry in bytes: Port(2) + Status(1) + CallInfo(32) = 35 bytes.
    /// </summary>
    public const int EntrySize = 35;

    /// <summary>
    /// Size of the Call Info field in bytes.
    /// </summary>
    public const int CallInfoSize = 32;

    /// <summary>
    /// The port number (0-495), or 0xFFFF if requesting all ports.
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    /// The current status of the telephony client.
    /// </summary>
    public TelephonyClientState Status { get; set; }

    /// <summary>
    /// Call information as an ASCII string.
    /// Contains dial digits for outgoing calls, or CLI (Caller ID) information for incoming calls.
    /// Maximum 32 characters.
    /// </summary>
    public string CallInfo { get; set; } = string.Empty;

    /// <summary>
    /// Creates a new telephony client state entry.
    /// </summary>
    public TelephonyClientStateEntry()
    {
    }

    /// <summary>
    /// Creates a new telephony client state entry with specified parameters.
    /// </summary>
    /// <param name="port">The port number.</param>
    /// <param name="status">The client status.</param>
    /// <param name="callInfo">The call information string.</param>
    public TelephonyClientStateEntry(ushort port, TelephonyClientState status, string callInfo = "")
    {
        Port = port;
        Status = status;
        CallInfo = callInfo;
    }

    /// <summary>
    /// Parses a telephony client state entry from bytes.
    /// </summary>
    /// <param name="data">The source byte array.</param>
    /// <param name="offset">The offset to start reading from.</param>
    /// <returns>A new TelephonyClientStateEntry instance.</returns>
    public static TelephonyClientStateEntry FromBytes(byte[] data, int offset)
    {
        var entry = new TelephonyClientStateEntry();

        // Port (2 bytes, big-endian)
        entry.Port = (ushort)((data[offset] << 8) | data[offset + 1]);
        offset += 2;

        // Status (1 byte)
        entry.Status = (TelephonyClientState)data[offset++];

        // Call Info (32 bytes, ASCII string, null-terminated or padded)
        int callInfoEnd = offset + CallInfoSize;
        int nullIndex = Array.IndexOf(data, (byte)0, offset, CallInfoSize);
        int stringLength = (nullIndex >= 0) ? nullIndex - offset : CallInfoSize;
        entry.CallInfo = Encoding.ASCII.GetString(data, offset, stringLength).TrimEnd('\0', ' ');

        return entry;
    }

    /// <summary>
    /// Gets a value indicating whether the port is on-hook (idle).
    /// </summary>
    public bool IsOnHook => Status == TelephonyClientState.OnHook ||
                            Status == TelephonyClientState.OnHookAllocated;

    /// <summary>
    /// Gets a value indicating whether the port is in an active call.
    /// </summary>
    public bool IsInCall => Status == TelephonyClientState.ConnectedIn ||
                            Status == TelephonyClientState.ConnectedOut;

    /// <summary>
    /// Gets a value indicating whether the port is connecting (ringing or dialing).
    /// </summary>
    public bool IsConnecting => Status == TelephonyClientState.ConnectingIn ||
                                Status == TelephonyClientState.ConnectingOut;

    /// <summary>
    /// Gets a value indicating whether this is an outgoing call.
    /// </summary>
    public bool IsOutgoing => Status == TelephonyClientState.ConnectingOut ||
                              Status == TelephonyClientState.ConnectedOut;

    /// <summary>
    /// Gets a value indicating whether this is an incoming call.
    /// </summary>
    public bool IsIncoming => Status == TelephonyClientState.ConnectingIn ||
                              Status == TelephonyClientState.ConnectedIn;
}
