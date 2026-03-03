namespace EclipseHXSNMP.Models;

/// <summary>
/// Represents a FreeSpeak II beltpack for SNMP exposure.
/// Combines data from Peripheral Info (firmware/labels) and Beltpack Status (live state).
/// </summary>
public class SnmpBeltpackEntry
{
    /// <summary>
    /// 1-based index for the SNMP table row.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Beltpack unique identifier (PMID).
    /// </summary>
    public uint Pmid { get; set; }

    /// <summary>
    /// Port or role number.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Panel type identifier.
    /// </summary>
    public ushort PanelType { get; set; }

    /// <summary>
    /// Whether the beltpack is online.
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// Talk and listen label.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Talk and listen alias.
    /// </summary>
    public string Alias { get; set; } = string.Empty;

    /// <summary>
    /// Number of keys on the beltpack.
    /// </summary>
    public byte NumberOfKeys { get; set; }

    /// <summary>
    /// Firmware version string (first version entry, if available).
    /// </summary>
    public string FirmwareVersion { get; set; } = string.Empty;

    /// <summary>
    /// Frequency type description (e.g., "1.9 GHz", "2.4 GHz", "Not Set").
    /// From live Beltpack Status (0x004C).
    /// </summary>
    public string Frequency { get; set; } = string.Empty;

    /// <summary>
    /// Wireless mode (e.g., "FS1", "FS2").
    /// From live Beltpack Status (0x004C).
    /// </summary>
    public string WirelessMode { get; set; } = string.Empty;

    /// <summary>
    /// Role number currently in use.
    /// From live Beltpack Status (0x004C).
    /// </summary>
    public ushort RoleNumber { get; set; }

    /// <summary>
    /// Antenna port the beltpack is connected to.
    /// From live Beltpack Status (0x004C).
    /// </summary>
    public ushort AntennaPort { get; set; }
}
