using HCILibrary.HCIRequests;
using HCILibrary.Helpers;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents the level status of a single input port.
/// </summary>
public class InputLevelStatus
{
    /// <summary>
    /// Port number (0-1023).
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Level value (0-255).
    /// </summary>
    public byte LevelValue { get; set; }

    /// <summary>
    /// Gets the level as a dB value using the GainLevel helper.
    /// </summary>
    public double LevelDb => GainLevel.LevelToDb(LevelValue);

    /// <summary>
    /// Gets a formatted string representation of the level.
    /// </summary>
    public string LevelFormatted => GainLevel.FormatLevel(LevelValue);
}

/// <summary>
/// Reply Input Level Status (0x0022).
/// Generated when a Request Input Level Status or Actions message has been received.
/// Only non-zero values are included in the reply.
/// </summary>
public class ReplyInputLevelStatus
{
    /// <summary>
    /// Protocol schema version from the response.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The list of input level statuses (only non-zero values).
    /// </summary>
    public List<InputLevelStatus> Levels { get; } = new();

    /// <summary>
    /// Parses a Reply Input Level Status response from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags, starting at protocol tag).</param>
    /// <returns>The parsed response.</returns>
    public static ReplyInputLevelStatus Parse(byte[] payload)
    {
        var result = new ReplyInputLevelStatus();
        int offset = 0;

        // Skip protocol tag (4 bytes) - already validated by caller
        offset += 4;

        // Protocol schema (1 byte)
        result.ProtocolSchema = payload[offset++];

        // Count (16-bit, big-endian)
        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Level data (4 bytes each: 2 for port + 2 for level)
        for (int i = 0; i < count; i++)
        {
            if (offset + 4 <= payload.Length)
            {
                var status = new InputLevelStatus
                {
                    PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]),
                    LevelValue = payload[offset + 3] // Level is in low byte of 16-bit word
                };
                result.Levels.Add(status);
                offset += 4;
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the level for a specific port, or null if not found.
    /// </summary>
    /// <param name="portNumber">The port number to look up.</param>
    /// <returns>The level status, or null if not in the response.</returns>
    public InputLevelStatus? GetLevel(ushort portNumber)
    {
        return Levels.Find(l => l.PortNumber == portNumber);
    }
}
