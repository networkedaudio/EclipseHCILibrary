using HCILibrary.Helpers;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a single VOX threshold entry for a port.
/// </summary>
public class VoxThresholdEntry
{
    /// <summary>
    /// Gets or sets the zero-based port number.
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    /// Gets or sets the VOX threshold level (0-40).
    /// Use <see cref="VoxGain"/> helper class for dB conversion.
    /// </summary>
    public ushort Level { get; set; }

    /// <summary>
    /// Gets the VOX threshold level in decibels (-40 to 0 dB).
    /// </summary>
    public int LevelDb => VoxGain.ToDecibels(Level);

    /// <summary>
    /// Gets whether this port has a non-default VOX threshold.
    /// Default threshold is 20 (-20 dB).
    /// </summary>
    public bool IsNonDefault => !VoxGain.IsDefaultLevel(Level);

    /// <summary>
    /// Returns a string representation of this VOX threshold entry.
    /// </summary>
    public override string ToString()
    {
        return $"Port {Port}: Level={Level} ({VoxGain.GetDescription(Level)})";
    }
}

/// <summary>
/// Reply VOX Threshold Status (Message ID 0x004A).
/// Used to acknowledge a VOX level set or reply to a Request VOX Threshold Status message.
/// Only ports with non-default threshold levels (default is 20) are reported.
/// </summary>
public class ReplyVoxThresholdStatus
{
    /// <summary>
    /// Gets or sets the protocol schema version.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// Gets or sets the list of VOX threshold entries.
    /// Only contains ports with non-default threshold levels.
    /// </summary>
    public List<VoxThresholdEntry> Entries { get; set; } = new();

    /// <summary>
    /// Decodes a Reply VOX Threshold Status message from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes.</param>
    /// <returns>The decoded ReplyVoxThresholdStatus.</returns>
    public static ReplyVoxThresholdStatus Decode(byte[] payload)
    {
        var result = new ReplyVoxThresholdStatus();

        if (payload == null || payload.Length < 1)
        {
            return result;
        }

        int offset = 0;

        // Check for HCIv2 marker (0xAB 0xBA 0xCE 0xDE)
        if (payload.Length >= 4 &&
            payload[0] == 0xAB && payload[1] == 0xBA &&
            payload[2] == 0xCE && payload[3] == 0xDE)
        {
            offset = 4;

            // Read schema byte
            if (offset < payload.Length)
            {
                result.Schema = payload[offset++];
            }
        }

        // Read entry count (2 bytes, big-endian)
        if (offset + 2 > payload.Length)
        {
            return result;
        }

        ushort entryCount = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Parse each entry
        // Each entry: Port (2 bytes) + Level (2 bytes) = 4 bytes
        const int entrySize = 4;

        for (int i = 0; i < entryCount && offset + entrySize <= payload.Length; i++)
        {
            var entry = new VoxThresholdEntry
            {
                Port = (ushort)((payload[offset] << 8) | payload[offset + 1]),
                Level = (ushort)((payload[offset + 2] << 8) | payload[offset + 3])
            };

            result.Entries.Add(entry);
            offset += entrySize;
        }

        return result;
    }

    /// <summary>
    /// Gets the VOX threshold entry for a specific port.
    /// </summary>
    /// <param name="port">The port number.</param>
    /// <returns>The VOX threshold entry, or null if not found.</returns>
    public VoxThresholdEntry? GetEntry(ushort port)
    {
        return Entries.Find(e => e.Port == port);
    }

    /// <summary>
    /// Gets the VOX threshold level for a specific port.
    /// Returns the default level (20) if the port is not in the list.
    /// </summary>
    /// <param name="port">The port number.</param>
    /// <returns>The VOX threshold level (0-40).</returns>
    public ushort GetLevel(ushort port)
    {
        var entry = GetEntry(port);
        return entry?.Level ?? VoxGain.DefaultLevel;
    }

    /// <summary>
    /// Gets the VOX threshold level in decibels for a specific port.
    /// Returns -20 dB (default) if the port is not in the list.
    /// </summary>
    /// <param name="port">The port number.</param>
    /// <returns>The VOX threshold level in decibels (-40 to 0).</returns>
    public int GetLevelDb(ushort port)
    {
        return VoxGain.ToDecibels(GetLevel(port));
    }

    /// <summary>
    /// Returns a string representation of this VOX threshold status.
    /// </summary>
    public override string ToString()
    {
        return $"VOX Threshold Status: {Entries.Count} non-default entries";
    }
}
