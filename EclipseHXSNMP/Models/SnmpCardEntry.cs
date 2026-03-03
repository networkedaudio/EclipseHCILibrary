using HCILibrary.Enums;

namespace EclipseHXSNMP.Models;

/// <summary>
/// Represents a card in the matrix for SNMP exposure.
/// </summary>
public class SnmpCardEntry
{
    /// <summary>
    /// 1-based index for the SNMP table row.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// The card type.
    /// </summary>
    public CardType CardType { get; set; }

    /// <summary>
    /// The card condition.
    /// </summary>
    public CardCondition Condition { get; set; }

    /// <summary>
    /// Whether this is slot 0 of the rack.
    /// </summary>
    public bool IsSlotZero { get; set; }

    /// <summary>
    /// The raw 16-bit status word.
    /// </summary>
    public ushort RawStatus { get; set; }
}
