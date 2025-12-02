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
        // KernelVer(4) + BootVer(4) + TalkListenLabel(20) + FSVer(4) + TalkLabel(20) +
        // NumKeys(1) + AnswerBack(1) + NumExpPanels(1) + ExpStartRegion(1) = 69 bytes
        if (payload.Length < offset + 69)
        {
            return null;
        }

        var portInfo = new PortInfo();

        // Port number: 16 bit word
        portInfo.PortNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Panel type: 16 bit word
        portInfo.RawPanelType = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        portInfo.PanelType = Enum.IsDefined(typeof(PortPanelType), portInfo.RawPanelType)
            ? (PortPanelType)portInfo.RawPanelType
            : PortPanelType.Unknown;
        offset += 2;

        // Operational status: 1 byte
        portInfo.OperationalStatus = PanelOperationalStatus.Parse(payload[offset]);
        offset += 1;

        // Panel firmware: 8 bytes string
        portInfo.PanelFirmware = ParseNullTerminatedString(payload, offset, 8);
        offset += 8;

        // Kernel version: 4 bytes
        portInfo.KernelVersion = VersionInfo.Parse(payload, offset);
        offset += 4;

        // Boot version: 4 bytes
        portInfo.BootVersion = VersionInfo.Parse(payload, offset);
        offset += 4;

        // Talk & Listen label: 10 words (20 bytes)
        portInfo.TalkListenLabel = ParseNullTerminatedString(payload, offset, 20);
        offset += 20;

        // FileSystem version: 4 bytes
        portInfo.FileSystemVersion = VersionInfo.Parse(payload, offset);
        offset += 4;

        // Talk label: 10 words (20 bytes)
        portInfo.TalkLabel = ParseNullTerminatedString(payload, offset, 20);
        offset += 20;

        // Number of keys: 1 byte
        portInfo.NumberOfKeys = payload[offset];
        offset += 1;

        // Answer back timeout: 1 byte
        portInfo.AnswerBackTimeout = payload[offset];
        offset += 1;

        // Number of expansion panels: 1 byte
        portInfo.NumberOfExpansionPanels = payload[offset];
        offset += 1;

        // Expansion panel start region: 1 byte
        portInfo.ExpansionPanelStartRegion = payload[offset];
        offset += 1;

        // Parse expansion panels
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
    /// Parses a null-terminated string from the payload.
    /// </summary>
    private static string ParseNullTerminatedString(byte[] payload, int offset, int maxLength)
    {
        int length = 0;
        for (int i = 0; i < maxLength && offset + i < payload.Length; i++)
        {
            if (payload[offset + i] == 0)
            {
                break;
            }
            length++;
        }
        return length > 0 ? System.Text.Encoding.ASCII.GetString(payload, offset, length) : string.Empty;
    }
}
