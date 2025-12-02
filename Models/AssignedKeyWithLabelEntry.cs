using HCILibrary.Enums;

namespace HCILibrary.Models;

/// <summary>
/// Represents an assigned key entry with label in the Reply Assigned Keys (With Labels) message.
/// </summary>
public class AssignedKeyWithLabelEntry
{
    /// <summary>
    /// Region this key is on.
    /// </summary>
    public byte Region { get; set; }

    /// <summary>
    /// Key ID.
    /// </summary>
    public byte Key { get; set; }

    /// <summary>
    /// Page this key is on.
    /// </summary>
    public byte Page { get; set; }

    /// <summary>
    /// Entity type (e.g., EN_PORT, EN_GROUP).
    /// </summary>
    public ushort Entity { get; set; }

    /// <summary>
    /// Key state (bits 6-7 of Key Status).
    /// </summary>
    public byte KeyState { get; set; }

    /// <summary>
    /// Listen mode (bit 5 of Key Status).
    /// </summary>
    public bool ListenMode { get; set; }

    /// <summary>
    /// Pot assigned (bit 4 of Key Status).
    /// </summary>
    public bool PotAssigned { get; set; }

    /// <summary>
    /// Pot state (bits 0-3 of Key Status).
    /// </summary>
    public byte PotState { get; set; }

    /// <summary>
    /// Pot number.
    /// </summary>
    public ushort PotNumber { get; set; }

    /// <summary>
    /// Unpaged flag (bit 8 of Key Operation).
    /// </summary>
    public bool Unpaged { get; set; }

    /// <summary>
    /// Text mode - displayed (bit 7 of Key Operation).
    /// </summary>
    public bool TextMode { get; set; }

    /// <summary>
    /// Dual (double width) flag (bit 6 of Key Operation).
    /// </summary>
    public bool Dual { get; set; }

    /// <summary>
    /// Dial mode flag (bit 5 of Key Operation).
    /// </summary>
    public bool Dial { get; set; }

    /// <summary>
    /// Latch mode (bits 0-3 of Key Operation byte 1).
    /// </summary>
    public LatchMode LatchMode { get; set; }

    /// <summary>
    /// Interlock group number (bits 4-7 of Key Operation byte 1).
    /// </summary>
    public byte Group { get; set; }

    /// <summary>
    /// Deactivating - all interlock group keys may be off (bit 3 of Key Operation byte 2).
    /// </summary>
    public bool Deactivating { get; set; }

    /// <summary>
    /// Make/Break - interlock group make before break (bit 2 of Key Operation byte 2).
    /// </summary>
    public bool MakeBreak { get; set; }

    /// <summary>
    /// Cross page - keys interlocked across pages in same region (bit 1 of Key Operation byte 2).
    /// </summary>
    public bool CrossPage { get; set; }

    /// <summary>
    /// CMAPSi flag special 1 (bit 0 of Key Operation byte 2).
    /// </summary>
    public bool CmapsiSp1 { get; set; }

    /// <summary>
    /// CMAPSi flag special 2 (bit 7 of Key Operation byte 3).
    /// </summary>
    public bool CmapsiSp2 { get; set; }

    /// <summary>
    /// Key region number (bits 4-6 of Key Operation byte 3).
    /// </summary>
    public byte RegionValue { get; set; }

    /// <summary>
    /// Stacked group flag (bit 3 of Key Operation byte 3).
    /// </summary>
    public bool StackedGroup { get; set; }

    /// <summary>
    /// Page value - if page key, the page it switches to when pressed.
    /// </summary>
    public byte PageValue { get; set; }

    /// <summary>
    /// Key configuration (26 bytes).
    /// </summary>
    public byte[] KeyConfig { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// System number from key config.
    /// </summary>
    public byte SystemNumber { get; set; }

    /// <summary>
    /// Key label text.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Creates a new AssignedKeyWithLabelEntry.
    /// </summary>
    public AssignedKeyWithLabelEntry()
    {
    }

    public override string ToString()
    {
        return $"Key={Key}, Region={Region}, Page={Page}, Entity=0x{Entity:X4}, Label=\"{Label}\"";
    }
}
