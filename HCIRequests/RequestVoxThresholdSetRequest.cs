using HCILibrary.Enums;
using HCILibrary.Helpers;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Represents a single VOX threshold update entry for a port.
/// </summary>
public class VoxThresholdSetEntry
{
    /// <summary>
    /// Gets or sets the zero-based port number to update.
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    /// Gets or sets the VOX threshold level (0-40).
    /// Use <see cref="VoxGain"/> helper class for dB conversion.
    /// </summary>
    public ushort Level { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VoxThresholdSetEntry"/> class.
    /// </summary>
    public VoxThresholdSetEntry()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VoxThresholdSetEntry"/> class
    /// with the specified port and level.
    /// </summary>
    /// <param name="port">The zero-based port number.</param>
    /// <param name="level">The VOX threshold level (0-40).</param>
    public VoxThresholdSetEntry(ushort port, ushort level)
    {
        Port = port;
        Level = VoxGain.ClampLevel(level);
    }

    /// <summary>
    /// Creates a VOX threshold entry from a dB value.
    /// </summary>
    /// <param name="port">The zero-based port number.</param>
    /// <param name="decibels">The gain in decibels (-40 to 0).</param>
    /// <returns>A new VoxThresholdSetEntry.</returns>
    public static VoxThresholdSetEntry FromDecibels(ushort port, int decibels)
    {
        return new VoxThresholdSetEntry(port, VoxGain.FromDecibels(decibels));
    }

    /// <summary>
    /// Gets the VOX threshold level in decibels (-40 to 0 dB).
    /// </summary>
    public int LevelDb => VoxGain.ToDecibels(Level);

    /// <summary>
    /// Returns a string representation of this VOX threshold entry.
    /// </summary>
    public override string ToString()
    {
        return $"Port {Port}: Level={Level} ({VoxGain.GetDescription(Level)})";
    }
}

/// <summary>
/// Request VOX Threshold Set (0x0048).
/// Sets the VOX threshold levels for one or more ports.
/// HCIv2 only.
/// </summary>
public class RequestVoxThresholdSetRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Gets or sets the protocol schema version. Currently set to 1.
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// Gets or sets the list of VOX threshold entries to set.
    /// </summary>
    public List<VoxThresholdSetEntry> Entries { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestVoxThresholdSetRequest"/> class.
    /// </summary>
    public RequestVoxThresholdSetRequest()
        : base(HCIMessageID.RequestVoxThresholdSet)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestVoxThresholdSetRequest"/> class
    /// with a single port and level.
    /// </summary>
    /// <param name="port">The zero-based port number.</param>
    /// <param name="level">The VOX threshold level (0-40).</param>
    public RequestVoxThresholdSetRequest(ushort port, ushort level)
        : base(HCIMessageID.RequestVoxThresholdSet)
    {
        Entries.Add(new VoxThresholdSetEntry(port, level));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestVoxThresholdSetRequest"/> class
    /// with the specified entries.
    /// </summary>
    /// <param name="entries">The VOX threshold entries to set.</param>
    public RequestVoxThresholdSetRequest(IEnumerable<VoxThresholdSetEntry> entries)
        : base(HCIMessageID.RequestVoxThresholdSet)
    {
        Entries.AddRange(entries);
    }

    /// <summary>
    /// Adds a VOX threshold entry to the request.
    /// </summary>
    /// <param name="port">The zero-based port number.</param>
    /// <param name="level">The VOX threshold level (0-40).</param>
    /// <returns>This request instance for method chaining.</returns>
    public RequestVoxThresholdSetRequest AddEntry(ushort port, ushort level)
    {
        Entries.Add(new VoxThresholdSetEntry(port, level));
        return this;
    }

    /// <summary>
    /// Adds a VOX threshold entry to the request using a dB value.
    /// </summary>
    /// <param name="port">The zero-based port number.</param>
    /// <param name="decibels">The gain in decibels (-40 to 0).</param>
    /// <returns>This request instance for method chaining.</returns>
    public RequestVoxThresholdSetRequest AddEntryFromDecibels(ushort port, int decibels)
    {
        Entries.Add(VoxThresholdSetEntry.FromDecibels(port, decibels));
        return this;
    }

    /// <summary>
    /// Sets the VOX threshold to the default level (20, -20 dB) for the specified port.
    /// </summary>
    /// <param name="port">The zero-based port number.</param>
    /// <returns>This request instance for method chaining.</returns>
    public RequestVoxThresholdSetRequest SetToDefault(ushort port)
    {
        Entries.Add(new VoxThresholdSetEntry(port, VoxGain.DefaultLevel));
        return this;
    }

    /// <summary>
    /// Generates the payload for the Request VOX Threshold Set message.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // [0-3]:   HCIv2 Protocol Tag (0xAB 0xBA 0xCE 0xDE)
        // [4]:     Protocol Schema (1 byte)
        // [5-6]:   Count (2 bytes, big-endian) - number of entries
        // For each entry:
        //   [0-1]: Port (2 bytes, big-endian) - zero-based port number
        //   [2-3]: Level (2 bytes, big-endian) - VOX threshold level (0-40)

        int entryCount = Entries.Count;
        int payloadSize = 4 + 1 + 2 + (entryCount * 4); // Tag + Schema + Count + Entries
        var payload = new byte[payloadSize];

        int offset = 0;

        // HCIv2 protocol tag
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol schema
        payload[offset++] = ProtocolSchema;

        // Count (2 bytes, big-endian)
        payload[offset++] = (byte)((entryCount >> 8) & 0xFF);
        payload[offset++] = (byte)(entryCount & 0xFF);

        // Entries
        foreach (var entry in Entries)
        {
            // Port (2 bytes, big-endian)
            payload[offset++] = (byte)((entry.Port >> 8) & 0xFF);
            payload[offset++] = (byte)(entry.Port & 0xFF);

            // Level (2 bytes, big-endian)
            ushort clampedLevel = VoxGain.ClampLevel(entry.Level);
            payload[offset++] = (byte)((clampedLevel >> 8) & 0xFF);
            payload[offset++] = (byte)(clampedLevel & 0xFF);
        }

        return payload;
    }
}
