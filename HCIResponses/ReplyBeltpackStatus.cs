using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a single beltpack status entry.
/// </summary>
public class BeltpackStatusEntry
{
    /// <summary>
    /// Gets or sets the beltpack unique identifier (PMID - 20 bits).
    /// </summary>
    public uint Pmid { get; set; }

    /// <summary>
    /// Gets or sets the beltpack online/offline status.
    /// </summary>
    public BeltpackStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the frequency type (1.9 GHz, 2.4 GHz, or Not Set).
    /// </summary>
    public BeltpackFrequencyType FrequencyType { get; set; }

    /// <summary>
    /// Gets or sets the wireless mode (FS1 or FS2).
    /// </summary>
    public BeltpackWirelessMode WirelessMode { get; set; }

    /// <summary>
    /// Gets or sets the role number in use by this beltpack.
    /// </summary>
    public ushort RoleNumber { get; set; }

    /// <summary>
    /// Gets or sets the port number of the antenna the beltpack is connected to.
    /// </summary>
    public ushort AntennaPort { get; set; }

    /// <summary>
    /// Gets whether the beltpack is online.
    /// </summary>
    public bool IsOnline => Status == BeltpackStatus.Online;

    /// <summary>
    /// Gets whether the beltpack is using FreeSpeak 2 mode.
    /// </summary>
    public bool IsFreeSpeak2 => WirelessMode == BeltpackWirelessMode.FS2;

    /// <summary>
    /// Gets a description of the frequency type.
    /// </summary>
    public string FrequencyDescription => FrequencyType switch
    {
        BeltpackFrequencyType.Freq1_9Ghz => "1.9 GHz",
        BeltpackFrequencyType.Freq2_4Ghz => "2.4 GHz",
        _ => "Not Set"
    };

    /// <summary>
    /// Returns a string representation of this beltpack status entry.
    /// </summary>
    public override string ToString()
    {
        return $"PMID {Pmid:X5}: {Status}, {FrequencyDescription}, {WirelessMode}, Role={RoleNumber}, Antenna={AntennaPort}";
    }
}

/// <summary>
/// Reply Beltpack Status (Message ID 0x004C).
/// Sent when a beltpack status changes.
/// </summary>
public class ReplyBeltpackStatus
{
    /// <summary>
    /// Gets or sets the protocol schema version.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// Gets or sets the list of beltpack status entries.
    /// </summary>
    public List<BeltpackStatusEntry> Entries { get; set; } = new();

    /// <summary>
    /// Decodes a Reply Beltpack Status message from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes.</param>
    /// <returns>The decoded ReplyBeltpackStatus.</returns>
    public static ReplyBeltpackStatus Decode(byte[] payload)
    {
        var result = new ReplyBeltpackStatus();

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
        // Entry structure (8 bytes total):
        // Bytes 0-3: PMID (20 bits) | Status (1 bit) | FrequencyType (2 bits) | WirelessMode (1 bit) | Unused (8 bits)
        //   Bits 31-12: PMID (20 bits)
        //   Bit 11: Status (1 bit)
        //   Bits 10-9: Frequency Type (2 bits)
        //   Bit 8: Wireless Mode (1 bit)
        //   Bits 7-0: Unused (8 bits)
        // Bytes 4-5: Role Number (2 bytes, big-endian)
        // Bytes 6-7: Antenna Port (2 bytes, big-endian)
        const int entrySize = 8;

        for (int i = 0; i < entryCount && offset + entrySize <= payload.Length; i++)
        {
            // Read the 4-byte packed field
            uint packedField = (uint)((payload[offset] << 24) | 
                                      (payload[offset + 1] << 16) | 
                                      (payload[offset + 2] << 8) | 
                                      payload[offset + 3]);

            // Extract fields from packed data
            // PMID: bits 31-12 (20 bits)
            uint pmid = (packedField >> 12) & 0xFFFFF;

            // Status: bit 11 (1 bit)
            byte status = (byte)((packedField >> 11) & 0x01);

            // Frequency Type: bits 10-9 (2 bits)
            byte frequencyType = (byte)((packedField >> 9) & 0x03);

            // Wireless Mode: bit 8 (1 bit)
            byte wirelessMode = (byte)((packedField >> 8) & 0x01);

            // Unused: bits 7-0 (8 bits) - skip

            offset += 4;

            // Role Number (2 bytes, big-endian)
            ushort roleNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Antenna Port (2 bytes, big-endian)
            ushort antennaPort = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            var entry = new BeltpackStatusEntry
            {
                Pmid = pmid,
                Status = (BeltpackStatus)status,
                FrequencyType = (BeltpackFrequencyType)frequencyType,
                WirelessMode = (BeltpackWirelessMode)wirelessMode,
                RoleNumber = roleNumber,
                AntennaPort = antennaPort
            };

            result.Entries.Add(entry);
        }

        return result;
    }

    /// <summary>
    /// Gets the beltpack status entry for a specific PMID.
    /// </summary>
    /// <param name="pmid">The beltpack unique identifier.</param>
    /// <returns>The beltpack status entry, or null if not found.</returns>
    public BeltpackStatusEntry? GetEntry(uint pmid)
    {
        return Entries.Find(e => e.Pmid == pmid);
    }

    /// <summary>
    /// Gets all online beltpack entries.
    /// </summary>
    /// <returns>List of online beltpack entries.</returns>
    public List<BeltpackStatusEntry> GetOnlineEntries()
    {
        return Entries.FindAll(e => e.IsOnline);
    }

    /// <summary>
    /// Gets all offline beltpack entries.
    /// </summary>
    /// <returns>List of offline beltpack entries.</returns>
    public List<BeltpackStatusEntry> GetOfflineEntries()
    {
        return Entries.FindAll(e => !e.IsOnline);
    }

    /// <summary>
    /// Gets all beltpack entries connected to a specific antenna port.
    /// </summary>
    /// <param name="antennaPort">The antenna port number.</param>
    /// <returns>List of beltpack entries connected to the antenna.</returns>
    public List<BeltpackStatusEntry> GetEntriesByAntenna(ushort antennaPort)
    {
        return Entries.FindAll(e => e.AntennaPort == antennaPort);
    }

    /// <summary>
    /// Gets all beltpack entries using a specific role.
    /// </summary>
    /// <param name="roleNumber">The role number.</param>
    /// <returns>List of beltpack entries using the role.</returns>
    public List<BeltpackStatusEntry> GetEntriesByRole(ushort roleNumber)
    {
        return Entries.FindAll(e => e.RoleNumber == roleNumber);
    }

    /// <summary>
    /// Returns a string representation of this beltpack status.
    /// </summary>
    public override string ToString()
    {
        int online = Entries.Count(e => e.IsOnline);
        return $"Beltpack Status: {Entries.Count} entries ({online} online, {Entries.Count - online} offline)";
    }
}
