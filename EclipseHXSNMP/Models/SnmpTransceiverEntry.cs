namespace EclipseHXSNMP.Models;

/// <summary>
/// Represents a FreeSpeak II transceiver (antenna/splitter) for SNMP exposure.
/// Derived from Peripheral Info entries that have a physical slot.
/// </summary>
public class SnmpTransceiverEntry
{
    /// <summary>
    /// 1-based index for the SNMP table row.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Slot number of the transceiver card.
    /// </summary>
    public ushort SlotNumber { get; set; }

    /// <summary>
    /// Port or role number.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Panel type identifier.
    /// </summary>
    public ushort PanelType { get; set; }

    /// <summary>
    /// Whether the transceiver is online.
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// Talk and listen label.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Number of keys.
    /// </summary>
    public byte NumberOfKeys { get; set; }

    /// <summary>
    /// Number of expansion panels connected.
    /// </summary>
    public byte ExpansionPanels { get; set; }

    /// <summary>
    /// Firmware version string (first version entry, if available).
    /// </summary>
    public string FirmwareVersion { get; set; } = string.Empty;
}
