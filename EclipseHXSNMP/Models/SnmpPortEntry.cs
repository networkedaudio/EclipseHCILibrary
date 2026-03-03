using HCILibrary.Enums;

namespace EclipseHXSNMP.Models;

/// <summary>
/// Represents a port/panel in the matrix for SNMP exposure.
/// </summary>
public class SnmpPortEntry
{
    /// <summary>
    /// 1-based index for the SNMP table row.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// The panel/port number.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// The panel type.
    /// </summary>
    public PanelType PanelType { get; set; }

    /// <summary>
    /// The panel state.
    /// </summary>
    public PanelState State { get; set; }

    /// <summary>
    /// Whether this is an AoIP device.
    /// </summary>
    public bool IsAoipDevice { get; set; }
}
