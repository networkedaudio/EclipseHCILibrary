using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a firmware/software version.
/// </summary>
public class VersionInfo
{
    /// <summary>
    /// Major version number.
    /// </summary>
    public byte Major { get; set; }

    /// <summary>
    /// Minor version number.
    /// </summary>
    public byte Minor { get; set; }

    /// <summary>
    /// Revision number.
    /// </summary>
    public ushort Revision { get; set; }

    /// <summary>
    /// Returns a string representation of the version.
    /// </summary>
    public override string ToString() => $"{Major}.{Minor}.{Revision}";

    /// <summary>
    /// Parses a version from 4 bytes.
    /// </summary>
    /// <param name="payload">The payload bytes.</param>
    /// <param name="offset">The offset to start reading from.</param>
    /// <returns>The parsed VersionInfo.</returns>
    public static VersionInfo Parse(byte[] payload, int offset)
    {
        return new VersionInfo
        {
            Major = payload[offset],
            Minor = payload[offset + 1],
            Revision = (ushort)((payload[offset + 2] << 8) | payload[offset + 3])
        };
    }
}

/// <summary>
/// Represents panel operational status.
/// </summary>
public class PanelOperationalStatus
{
    /// <summary>
    /// Raw status byte.
    /// </summary>
    public byte RawStatus { get; set; }

    /// <summary>
    /// Whether the panel is online.
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// SubType flag. Usage depends on panel type:
    /// - LQ Panel Type: LQ
    /// - V-Series Panel Type: V-Series Iris Panel Generation
    /// </summary>
    public bool SubType { get; set; }

    /// <summary>
    /// Port type (bits 2-7).
    /// </summary>
    public PortType PortType { get; set; }

    /// <summary>
    /// Returns a human-readable representation of the operational status.
    /// </summary>
    public override string ToString()
    {
        string state = IsOnline ? "Online" : "Offline";
        return $"{state} ({PortType})";
    }

    /// <summary>
    /// Parses an operational status byte.
    /// </summary>
    /// <param name="status">The raw status byte.</param>
    /// <returns>The parsed PanelOperationalStatus.</returns>
    public static PanelOperationalStatus Parse(byte status)
    {
        byte portTypeValue = (byte)((status >> 2) & 0x3F);
        return new PanelOperationalStatus
        {
            RawStatus = status,
            IsOnline = (status & 0x01) != 0,
            SubType = (status & 0x02) != 0,
            PortType = Enum.IsDefined(typeof(PortType), portTypeValue)
                ? (PortType)portTypeValue
                : PortType.Null
        };
    }
}

/// <summary>
/// Represents an expansion panel entry.
/// </summary>
public class ExpansionPanelInfo
{
    /// <summary>
    /// Region number.
    /// </summary>
    public byte Region { get; set; }

    /// <summary>
    /// Expansion panel type.
    /// </summary>
    public ExpansionPanelType Type { get; set; }

    /// <summary>
    /// Current page.
    /// </summary>
    public byte CurrentPage { get; set; }

    /// <summary>
    /// Application version.
    /// </summary>
    public VersionInfo? AppVersion { get; set; }

    /// <summary>
    /// Boot version.
    /// </summary>
    public VersionInfo? BootVersion { get; set; }

    /// <summary>
    /// Parses an expansion panel info entry from payload.
    /// </summary>
    /// <param name="payload">The payload bytes.</param>
    /// <param name="offset">The offset to start reading from.</param>
    /// <returns>The parsed ExpansionPanelInfo, or null if insufficient data.</returns>
    public static ExpansionPanelInfo? Parse(byte[] payload, int offset)
    {
        // Each expansion panel: Region(1) + Type(2) + CurrentPage(1) + AppVersion(4) + BootVersion(4) = 12 bytes
        if (payload.Length < offset + 12)
        {
            return null;
        }

        ushort typeValue = (ushort)((payload[offset + 1] << 8) | payload[offset + 2]);

        return new ExpansionPanelInfo
        {
            Region = payload[offset],
            Type = Enum.IsDefined(typeof(ExpansionPanelType), typeValue)
                ? (ExpansionPanelType)typeValue
                : ExpansionPanelType.NotSet,
            CurrentPage = payload[offset + 3],
            AppVersion = VersionInfo.Parse(payload, offset + 4),
            BootVersion = VersionInfo.Parse(payload, offset + 8)
        };
    }
}

/// <summary>
/// Represents information about a single port.
/// </summary>
public class PortInfo
{
    /// <summary>
    /// The port number.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Panel type ID (last online ID).
    /// </summary>
    public PortPanelType PanelType { get; set; }

    /// <summary>
    /// Raw panel type value.
    /// </summary>
    public ushort RawPanelType { get; set; }

    /// <summary>
    /// Panel operational status.
    /// </summary>
    public PanelOperationalStatus? OperationalStatus { get; set; }

    /// <summary>
    /// Panel firmware string (8 bytes).
    /// </summary>
    public string PanelFirmware { get; set; } = string.Empty;

    /// <summary>
    /// Kernel version.
    /// </summary>
    public VersionInfo? KernelVersion { get; set; }

    /// <summary>
    /// Boot version.
    /// </summary>
    public VersionInfo? BootVersion { get; set; }

    /// <summary>
    /// Talk and Listen label (10 words = 20 bytes).
    /// </summary>
    public string TalkListenLabel { get; set; } = string.Empty;

    /// <summary>
    /// FileSystem version.
    /// </summary>
    public VersionInfo? FileSystemVersion { get; set; }

    /// <summary>
    /// Talk label (10 words = 20 bytes).
    /// </summary>
    public string TalkLabel { get; set; } = string.Empty;

    /// <summary>
    /// Number of keys on panel.
    /// </summary>
    public byte NumberOfKeys { get; set; }

    /// <summary>
    /// Answer back timeout (1-60 secs). 0 means timeout disabled.
    /// </summary>
    public byte AnswerBackTimeout { get; set; }

    /// <summary>
    /// Number of expansion panels supported.
    /// </summary>
    public byte NumberOfExpansionPanels { get; set; }

    /// <summary>
    /// The region the first expansion panel starts at.
    /// </summary>
    public byte ExpansionPanelStartRegion { get; set; }

    /// <summary>
    /// List of expansion panel information.
    /// </summary>
    public List<ExpansionPanelInfo> ExpansionPanels { get; set; } = new();
}

/// <summary>
/// Reply Port Info (0x00B8).
/// Returns the connected port type and additional port information for a card.
/// </summary>
public class ReplyPortInfo
{
    /// <summary>
    /// Card slot number.
    /// </summary>
    public ushort SlotNumber { get; set; }

    /// <summary>
    /// Number of ports in this response.
    /// </summary>
    public byte NumberPorts { get; set; }

    /// <summary>
    /// List of port information entries.
    /// </summary>
    public List<PortInfo> Ports { get; set; } = new();

    /// <summary>
    /// Parses the payload of a Reply Port Info message.
    /// </summary>
    /// <param name="payload">The message payload (after protocol schema byte).</param>
    /// <returns>The parsed ReplyPortInfo, or null if parsing fails.</returns>
    public static ReplyPortInfo? Parse(byte[] payload)
    {
        // Minimum payload: SlotNumber(2) + NumberPorts(1) = 3 bytes
        if (payload == null || payload.Length < 3)
        {
            return null;
        }

        var result = new ReplyPortInfo
        {
            SlotNumber = (ushort)((payload[0] << 8) | payload[1]),
            NumberPorts = payload[2]
        };

        int offset = 3;

        for (int i = 0; i < result.NumberPorts; i++)
        {
            var portInfo = ParsePortInfo(payload, ref offset);
            if (portInfo == null)
            {
                break;
            }
            result.Ports.Add(portInfo);
        }

        return result;
    }

    /// <summary>
    /// Parses a single port info entry from the payload.
    /// </summary>
    private static PortInfo? ParsePortInfo(byte[] payload, ref int offset)
    {
        // Minimum per port: PortNumber(2) + PanelType(2) + OpStatus(1) + Firmware(8) + 
        // BootVer(4) + KernelVer(4) + FSVer(4) + Reserved(4) + TalkListenLabel(20) + TalkLabel(20) + NumKeys(1) + AnswerBack(1) + NumExpPanels(1) + ExpPanelStartRegion(1) = 74 bytes
        if (payload.Length < offset + 74)
        {
            System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Insufficient data: need {offset + 74}, have {payload.Length}");
            return null;
        }

        var portInfo = new PortInfo();
        int startOffset = offset;

        // Port number: 16 bit word (big-endian)
        portInfo.PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: PortNumber = 0x{portInfo.PortNumber:X4} (bytes: {payload[offset]:X2} {payload[offset + 1]:X2})");
        offset += 2;

        // Panel type: 16 bit word (big-endian)
        portInfo.RawPanelType = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        portInfo.PanelType = Enum.IsDefined(typeof(PortPanelType), portInfo.RawPanelType)
            ? (PortPanelType)portInfo.RawPanelType
            : PortPanelType.Unknown;
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: PanelType = 0x{portInfo.RawPanelType:X4} (bytes: {payload[offset]:X2} {payload[offset + 1]:X2})");
        offset += 2;

        // Operational status: 1 byte
        portInfo.OperationalStatus = PanelOperationalStatus.Parse(payload[offset]);
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: OpStatus = {payload[offset]:X2}");
        offset += 1;

        // Panel firmware: 8 bytes string
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: PanelFirmware raw bytes: {BitConverter.ToString(payload, offset, 8)}");
        portInfo.PanelFirmware = ParseNullTerminatedString(payload, offset, 8);
        offset += 8;

        // Boot version: 4 bytes
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: BootVersion raw bytes: {BitConverter.ToString(payload, offset, 4)}");
        portInfo.BootVersion = VersionInfo.Parse(payload, offset);
        offset += 4;

        // Kernel version: 4 bytes
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: KernelVersion raw bytes: {BitConverter.ToString(payload, offset, 4)}");
        portInfo.KernelVersion = VersionInfo.Parse(payload, offset);
        offset += 4;

        // FileSystem version: 4 bytes
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: FileSystemVersion raw bytes: {BitConverter.ToString(payload, offset, 4)}");
        portInfo.FileSystemVersion = VersionInfo.Parse(payload, offset);
        offset += 4;

        // Reserved: 4 bytes (unused, reserved for future expansion)
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: Reserved raw bytes: {BitConverter.ToString(payload, offset, 4)}");
        offset += 4;

        // Talk & Listen label (talkAndListenLabel): 10 words (20 bytes)
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: TalkListenLabel raw bytes: {BitConverter.ToString(payload, offset, Math.Min(20, payload.Length - offset))}");
        portInfo.TalkListenLabel = ParseNullTerminatedString(payload, offset, 20);
        offset += 20;

        // Talk & Listen alias (talkAndListenAlias): 10 words (20 bytes)
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: TalkLabel raw bytes: {BitConverter.ToString(payload, offset, Math.Min(20, payload.Length - offset))}");
        portInfo.TalkLabel = ParseNullTerminatedString(payload, offset, 20);
        offset += 20;

        // Number of keys: 1 byte
        portInfo.NumberOfKeys = payload[offset];
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: NumberOfKeys = {portInfo.NumberOfKeys} (byte: {payload[offset]:X2})");
        offset += 1;

        // Answer back timeout: 1 byte
        portInfo.AnswerBackTimeout = payload[offset];
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: AnswerBackTimeout = {portInfo.AnswerBackTimeout} (byte: {payload[offset]:X2})");
        offset += 1;

        // Number of expansion panels: 1 byte
        portInfo.NumberOfExpansionPanels = payload[offset];
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: NumberOfExpansionPanels = {portInfo.NumberOfExpansionPanels} (byte: {payload[offset]:X2})");
        offset += 1;

        // Expansion panel start region: 1 byte
        portInfo.ExpansionPanelStartRegion = payload[offset];
        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Offset {offset}: ExpansionPanelStartRegion = {portInfo.ExpansionPanelStartRegion} (byte: {payload[offset]:X2})");
        offset += 1;

        System.Diagnostics.Debug.WriteLine($"[ParsePortInfo] Finished port at startOffset={startOffset}, advanced {offset - startOffset} bytes. New offset={offset}");

        // Parse expansion panels - read the actual NumberOfExpansionPanels count
        System.Diagnostics.Debug.WriteLine($"[VERIFY] offset={offset}, NumberOfExpansionPanels={portInfo.NumberOfExpansionPanels}, ExpansionPanelStartRegion={portInfo.ExpansionPanelStartRegion}");
        for (int j = 0; j < portInfo.NumberOfExpansionPanels; j++)
        {
            var expPanel = ExpansionPanelInfo.Parse(payload, offset);
            if (expPanel == null)
            {
                break;
            }
            portInfo.ExpansionPanels.Add(expPanel);
            offset += 12; // Each expansion panel is 12 bytes
        }

        return portInfo;
    }

    /// <summary>
    /// Parses a null-terminated UTF-16 big-endian (wide-char) string from the payload.
    /// </summary>
    /// <param name="payload">The payload bytes.</param>
    /// <param name="offset">The offset to start reading from.</param>
    /// <param name="byteLength">The total field size in bytes (2 bytes per character).</param>
    private static string ParseNullTerminatedString(byte[] payload, int offset, int byteLength)
    {
        try
        {
            int available = Math.Max(0, payload.Length - offset);
            int count = Math.Min(byteLength, available);
            // Dump the raw bytes for this field (limited to the provided byteLength)
            string hex = count > 0 ? BitConverter.ToString(payload, offset, Math.Min(count, 32)) : "";
            System.Diagnostics.Debug.WriteLine($"[ReplyPortInfo] ParseNullTerminatedString: offset={offset}, byteLength={byteLength}, available={available}, bytes={hex}...");

            // Check if the field starts with a null terminator (0x00 0x00 in UTF-16BE)
            // This indicates an empty string, not just the first character being empty
            if (offset + 1 < payload.Length && payload[offset] == 0x00 && payload[offset + 1] == 0x00)
            {
                System.Diagnostics.Debug.WriteLine("[ReplyPortInfo] Parsed string: <empty> (null terminator at start)");
                return string.Empty;
            }

            int charCount = byteLength / 2;
            int length = 0;
            for (int i = 0; i < charCount; i++)
            {
                int charOffset = offset + (i * 2);
                if (charOffset + 1 >= payload.Length)
                {
                    break;
                }

                ushort charValue = (ushort)((payload[charOffset] << 8) | payload[charOffset + 1]);
                if (charValue == 0)
                {
                    break;
                }
                length++;
            }

            if (length > 0)
            {
                string result = System.Text.Encoding.BigEndianUnicode.GetString(payload, offset, length * 2);
                System.Diagnostics.Debug.WriteLine($"[ReplyPortInfo] Parsed string (chars={length}): '{result}'");
                return result;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[ReplyPortInfo] Parsed string: <empty>");
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ReplyPortInfo] ParseNullTerminatedString exception: {ex}");
            return string.Empty;
        }
    }
}
