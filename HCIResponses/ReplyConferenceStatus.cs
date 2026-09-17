namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Conference Status (HCIv2) - Message ID 0x0014.
/// Contains a list of all the currently connected ports to a conference.
/// Generated in response to Request Conference Status message or whenever
/// a port is connected to or removed from any conference.
/// </summary>
public class ReplyConferenceStatus
{
    /// <summary>
    /// The conference number (0-8191, 13 bits).
    /// </summary>
    public ushort ConferenceNumber { get; set; }

    /// <summary>
    /// The list of ports connected to this conference.
    /// </summary>
    public List<ConferencePortStatus> Ports { get; } = new();

    /// <summary>
    /// Decodes a Reply Conference Status from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes (after protocol tag and schema).</param>
    /// <returns>The decoded reply, or null if invalid.</returns>
    /// <summary>
    /// Decodes a Reply Conference Status from the payload bytes.
    /// 
    /// Per the HCI protocol documentation (section 4.12.2), the structure is:
    ///   Port Count (2B) | Conference Data (2B) | Port Data (2B per port)
    /// where Port Count is the number of items (Conference Data + all Port Data words).
    /// 
    /// Conference Data structure:
    ///   bits 0-12: conference number
    ///   bits 13-14: set to 0
    ///   bit 15: set to 1
    /// 
    /// Port Data structure (2 bytes per port):
    ///   bits 0-12: port number (0-1023)
    ///   bit 13: listener flag (0=no, 1=yes)
    ///   bit 14: talker flag (0=no, 1=yes)
    ///   bit 15: set to 0
    /// </summary>
    /// <param name="payload">The payload bytes after protocol tag and schema.</param>
    /// <returns>The decoded reply, or null if invalid.</returns>
    public static ReplyConferenceStatus? Decode(byte[] payload)
    {
        // Minimum: 2 bytes (Conference Data)
        if (payload == null || payload.Length < 2)
        {
            System.Diagnostics.Debug.WriteLine($"[ReplyConferenceStatus.Decode] Invalid payload: null or too short ({payload?.Length ?? 0} bytes)");
            return null;
        }

        System.Diagnostics.Debug.WriteLine($"[ReplyConferenceStatus.Decode] Payload ({payload.Length} bytes): {BitConverter.ToString(payload)}");

        var reply = new ReplyConferenceStatus();
        int offset = 0;

        // Port Count: bytes 0-1 (16-bit, big-endian).
        // This is the number of items that follow (Conference Data + Port Data words).
        ushort portCount = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        System.Diagnostics.Debug.WriteLine($"[ReplyConferenceStatus.Decode] Port Count: {portCount}");

        // Conference Data: next 16-bit word (big-endian). Bit 15 is set to 1.
        if (offset + 2 > payload.Length)
        {
            System.Diagnostics.Debug.WriteLine($"[ReplyConferenceStatus.Decode] Missing Conference Data word");
            return reply;
        }

        ushort conferenceData = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        reply.ConferenceNumber = (ushort)(conferenceData & 0x1FFF);  // Extract bits 0-12
        offset += 2;

        System.Diagnostics.Debug.WriteLine($"[ReplyConferenceStatus.Decode] Conference: 0x{conferenceData:X4} => Conference #{reply.ConferenceNumber}");

        // Port Data: 2 bytes per port. Bit 15 is 0 for a valid port entry.
        int numPorts = (payload.Length - offset) / 2;

        System.Diagnostics.Debug.WriteLine($"[ReplyConferenceStatus.Decode] Port data: {numPorts} potential port(s)");

        for (int i = 0; i < numPorts; i++)
        {
            ushort portData = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // A port entry has bit 15 clear. A word with bit 15 set is a new
            // Conference Data marker (multiple conferences in one message) or padding.
            if ((portData & 0x8000) != 0)
            {
                System.Diagnostics.Debug.WriteLine($"[ReplyConferenceStatus.Decode]   Word {i}: 0x{portData:X4} has bit 15 set - not a port entry, stopping");
                break;
            }

            var portStatus = new ConferencePortStatus
            {
                PortNumber = (ushort)(portData & 0x1FFF),              // bits 0-12
                IsListener = (portData & 0x2000) != 0,                 // bit 13
                IsTalker = (portData & 0x4000) != 0                    // bit 14
            };

            System.Diagnostics.Debug.WriteLine($"[ReplyConferenceStatus.Decode]   Port {i}: 0x{portData:X4} => #{portStatus.PortNumber} (L={portStatus.IsListener}, T={portStatus.IsTalker})");
            reply.Ports.Add(portStatus);
        }

        System.Diagnostics.Debug.WriteLine($"[ReplyConferenceStatus.Decode] Result: Conference {reply.ConferenceNumber} with {reply.Ports.Count} port(s)");
        return reply;
    }

    public override string ToString()
    {
        return $"Conference {ConferenceNumber}: {Ports.Count} port(s)";
    }
}
