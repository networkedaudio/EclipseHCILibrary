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
    public static ReplyConferenceStatus? Decode(byte[] payload)
    {
        // Minimum: PortCount(2) + ConferenceData(2) = 4 bytes
        if (payload == null || payload.Length < 4)
        {
            return null;
        }

        var reply = new ReplyConferenceStatus();
        int offset = 0;

        // Port Count: 16 bit word (big-endian)
        ushort portCount = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Conference Data: 16 bit word
        // bits 0-12: conference number
        // bits 13,14: set to 0
        // bit 15: set to 1
        ushort conferenceData = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        reply.ConferenceNumber = (ushort)(conferenceData & 0x1FFF); // bits 0-12
        offset += 2;

        // Port Data: 16 bit word for each port
        for (int i = 0; i < portCount && offset + 2 <= payload.Length; i++)
        {
            ushort portData = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            var portStatus = new ConferencePortStatus
            {
                // bits 0-12: port number
                PortNumber = (ushort)(portData & 0x1FFF),
                // bit 13: listener flag
                IsListener = (portData & 0x2000) != 0,
                // bit 14: talker flag
                IsTalker = (portData & 0x4000) != 0
            };

            reply.Ports.Add(portStatus);
        }

        return reply;
    }

    public override string ToString()
    {
        return $"Conference {ConferenceNumber}: {Ports.Count} port(s)";
    }
}
