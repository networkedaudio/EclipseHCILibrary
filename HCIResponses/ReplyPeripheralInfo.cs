using System.Text;
using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Panel operation flags for peripheral info.
/// </summary>
[Flags]
public enum PeripheralPanelOperation : byte
{
    /// <summary>
    /// No flags set.
    /// </summary>
    None = 0x00,

    /// <summary>
    /// Panel is online.
    /// </summary>
    Online = 0x01,

    /// <summary>
    /// Sub type flag.
    /// </summary>
    SubType = 0x02
}

/// <summary>
/// Represents version information for a peripheral.
/// </summary>
public class PeripheralVersionInfo
{
    /// <summary>
    /// Major version number.
    /// </summary>
    public ushort Major { get; set; }

    /// <summary>
    /// Minor version number.
    /// </summary>
    public ushort Minor { get; set; }

    /// <summary>
    /// Firmware ID.
    /// </summary>
    public ushort FirmwareId { get; set; }

    /// <summary>
    /// Revision number.
    /// </summary>
    public ushort Revision { get; set; }

    /// <summary>
    /// Version string (up to 20 bytes / 10 Unicode characters).
    /// </summary>
    public string VersionString { get; set; } = string.Empty;

    /// <summary>
    /// Returns a formatted version string.
    /// </summary>
    public override string ToString()
    {
        return $"{Major}.{Minor}.{Revision} (FW:{FirmwareId}) {VersionString}".Trim();
    }
}

/// <summary>
/// Represents a single peripheral info entry.
/// </summary>
public class PeripheralInfoEntry
{
    /// <summary>
    /// Slot number of port. 0xFFFF for entries that do not have a slot (e.g., beltpack roles).
    /// </summary>
    public ushort SlotNumber { get; set; }

    /// <summary>
    /// Port or role number.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Panel type.
    /// </summary>
    public ushort PanelType { get; set; }

    /// <summary>
    /// Raw panel operation byte.
    /// </summary>
    public byte RawPanelOperation { get; set; }

    /// <summary>
    /// Whether the panel is online (bit 0).
    /// </summary>
    public bool IsOnline => (RawPanelOperation & 0x01) != 0;

    /// <summary>
    /// Sub type flag (bit 1).
    /// </summary>
    public bool SubType => (RawPanelOperation & 0x02) != 0;

    /// <summary>
    /// Type value (bits 2-7).
    /// </summary>
    public byte Type => (byte)((RawPanelOperation >> 2) & 0x3F);

    /// <summary>
    /// Talk and listen label (Unicode, up to 10 characters).
    /// </summary>
    public string TalkListenLabel { get; set; } = string.Empty;

    /// <summary>
    /// Talk and listen alias (Unicode, up to 10 characters).
    /// </summary>
    public string TalkListenAlias { get; set; } = string.Empty;

    /// <summary>
    /// Number of keys on the panel.
    /// </summary>
    public byte NumberOfKeys { get; set; }

    /// <summary>
    /// Answerback timeout value.
    /// </summary>
    public byte AnswerbackTimeout { get; set; }

    /// <summary>
    /// Number of expansion panels.
    /// </summary>
    public byte NumberOfExpansionPanels { get; set; }

    /// <summary>
    /// Region number of the first expansion panel.
    /// </summary>
    public byte FirstExpansionPanelRegion { get; set; }

    /// <summary>
    /// Beltpack unique identifier (PMID).
    /// </summary>
    public uint Pmid { get; set; }

    /// <summary>
    /// List of version information for this peripheral.
    /// </summary>
    public List<PeripheralVersionInfo> Versions { get; set; } = new();

    /// <summary>
    /// Indicates if this entry has no slot (e.g., beltpack role).
    /// </summary>
    public bool HasNoSlot => SlotNumber == 0xFFFF;
}

/// <summary>
/// Reply Peripheral Info (0x007B, Sub Type 0x15).
/// Response to Request Peripheral Info. Contains software and hardware version information
/// for FreeSpeak 2 Antenna, Splitters, Beltpacks, and other peripherals.
/// Never sent unsolicited.
/// </summary>
public class ReplyPeripheralInfo
{
    /// <summary>
    /// Sub type for Peripheral Info reply.
    /// </summary>
    public const byte SubType = 0x15;

    /// <summary>
    /// Protocol schema version from the response.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The slot number that was in the request.
    /// </summary>
    public byte RequestedSlotNumber { get; set; }

    /// <summary>
    /// Number of peripheral info entries.
    /// </summary>
    public ushort Count { get; set; }

    /// <summary>
    /// List of peripheral info entries.
    /// </summary>
    public List<PeripheralInfoEntry> Entries { get; set; } = new();

    /// <summary>
    /// Parses a Reply Peripheral Info response from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags, starting at protocol schema).</param>
    /// <returns>The parsed response, or null if parsing fails.</returns>
    public static ReplyPeripheralInfo? Parse(byte[] payload)
    {
        // Minimum payload: ProtocolSchema(1) + SubType(1) + SlotNumber(1) + Count(2) = 5 bytes
        if (payload == null || payload.Length < 5)
        {
            return null;
        }

        int offset = 0;
        var result = new ReplyPeripheralInfo();

        // Protocol Schema (1 byte)
        result.ProtocolSchema = payload[offset++];

        // Sub Type (1 byte) - should be 0x15
        byte subType = payload[offset++];
        if (subType != SubType)
        {
            return null;
        }

        // Slot Number (1 byte)
        result.RequestedSlotNumber = payload[offset++];

        // Count (2 bytes, big-endian)
        result.Count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Parse each entry
        for (int i = 0; i < result.Count; i++)
        {
            var entry = ParseEntry(payload, ref offset);
            if (entry == null)
            {
                break;
            }
            result.Entries.Add(entry);
        }

        return result;
    }

    private static PeripheralInfoEntry? ParseEntry(byte[] payload, ref int offset)
    {
        // Minimum entry size before version strings:
        // SlotNumber(2) + PortNumber(2) + PanelType(2) + PanelOperation(1) +
        // TalkListenLabel(20) + TalkListenAlias(20) + NumberOfKeys(1) +
        // AnswerbackTimeout(1) + NumberOfExpansionPanels(1) + FirstExpansionPanelRegion(1) +
        // PMID(4) + NumberOfVersionStrings(1) = 56 bytes minimum
        const int minEntrySize = 56;

        if (payload.Length < offset + minEntrySize)
        {
            return null;
        }

        var entry = new PeripheralInfoEntry();

        // Slot Number (2 bytes, big-endian)
        entry.SlotNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Port Number (2 bytes, big-endian)
        entry.PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Panel Type (2 bytes, big-endian)
        entry.PanelType = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Panel Operation (1 byte)
        entry.RawPanelOperation = payload[offset++];

        // Talk and Listen Label (20 bytes, Unicode/UTF-16BE)
        entry.TalkListenLabel = ParseUnicodeString(payload, offset, 20);
        offset += 20;

        // Talk and Listen Alias (20 bytes, Unicode/UTF-16BE)
        entry.TalkListenAlias = ParseUnicodeString(payload, offset, 20);
        offset += 20;

        // Number of Keys (1 byte)
        entry.NumberOfKeys = payload[offset++];

        // Answerback Timeout (1 byte)
        entry.AnswerbackTimeout = payload[offset++];

        // Number of Expansion Panels (1 byte)
        entry.NumberOfExpansionPanels = payload[offset++];

        // Region Number of First Expansion Panel (1 byte)
        entry.FirstExpansionPanelRegion = payload[offset++];

        // PMID (4 bytes, big-endian)
        entry.Pmid = (uint)((payload[offset] << 24) | (payload[offset + 1] << 16) |
                           (payload[offset + 2] << 8) | payload[offset + 3]);
        offset += 4;

        // Number of Version Strings (1 byte)
        byte numVersions = payload[offset++];

        // Parse version strings
        // Each version: Major(2) + Minor(2) + FirmwareId(2) + Revision(2) + VersionString(20) = 28 bytes
        const int versionSize = 28;

        for (int v = 0; v < numVersions; v++)
        {
            if (payload.Length < offset + versionSize)
            {
                break;
            }

            var version = new PeripheralVersionInfo();

            // Major (2 bytes, big-endian)
            version.Major = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Minor (2 bytes, big-endian)
            version.Minor = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Firmware ID (2 bytes, big-endian)
            version.FirmwareId = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Revision (2 bytes, big-endian)
            version.Revision = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Version String (20 bytes, assume ASCII or UTF-8)
            version.VersionString = ParseAsciiString(payload, offset, 20);
            offset += 20;

            entry.Versions.Add(version);
        }

        return entry;
    }

    private static string ParseUnicodeString(byte[] payload, int offset, int byteLength)
    {
        if (payload.Length < offset + byteLength)
        {
            return string.Empty;
        }

        // Parse as UTF-16 Big Endian (wchar_t)
        var bytes = new byte[byteLength];
        Array.Copy(payload, offset, bytes, 0, byteLength);

        try
        {
            string result = Encoding.BigEndianUnicode.GetString(bytes);
            // Trim null characters
            int nullIndex = result.IndexOf('\0');
            if (nullIndex >= 0)
            {
                result = result.Substring(0, nullIndex);
            }
            return result;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string ParseAsciiString(byte[] payload, int offset, int byteLength)
    {
        if (payload.Length < offset + byteLength)
        {
            return string.Empty;
        }

        var bytes = new byte[byteLength];
        Array.Copy(payload, offset, bytes, 0, byteLength);

        try
        {
            string result = Encoding.ASCII.GetString(bytes);
            // Trim null characters
            int nullIndex = result.IndexOf('\0');
            if (nullIndex >= 0)
            {
                result = result.Substring(0, nullIndex);
            }
            return result;
        }
        catch
        {
            return string.Empty;
        }
    }
}
