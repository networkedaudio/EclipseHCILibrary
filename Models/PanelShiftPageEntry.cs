namespace HCILibrary.Models;

/// <summary>
/// Represents a single panel shift page action entry.
/// Used in RequestPanelShiftPageAction messages.
/// </summary>
public class PanelShiftPageEntry
{
    /// <summary>
    /// Card slot number for beltpack roles use 0xFF.
    /// </summary>
    public const byte BeltpackCardSlot = 0xFF;

    /// <summary>
    /// Card slot number.
    /// For beltpack roles, use 0xFF.
    /// </summary>
    public byte CardSlot { get; set; }

    /// <summary>
    /// Zero-based offset into panel card.
    /// For beltpack roles, this is the offset from the first role ID (600).
    /// </summary>
    public byte PortOffset { get; set; }

    /// <summary>
    /// The page number to make the currently selected page on this panel.
    /// Only included if Action is SetCurrentPage.
    /// 0 = main page, 1 = first shift page, etc. Max page is 8.
    /// </summary>
    public byte PageNumber { get; set; }

    /// <summary>
    /// Creates a new PanelShiftPageEntry.
    /// </summary>
    public PanelShiftPageEntry()
    {
    }

    /// <summary>
    /// Creates a new PanelShiftPageEntry for a panel card.
    /// </summary>
    /// <param name="cardSlot">Card slot number.</param>
    /// <param name="portOffset">Zero-based offset into panel card.</param>
    /// <param name="pageNumber">Page number (0 = main, 1-8 = shift pages).</param>
    public PanelShiftPageEntry(byte cardSlot, byte portOffset, byte pageNumber = 0)
    {
        CardSlot = cardSlot;
        PortOffset = portOffset;
        PageNumber = pageNumber;
    }

    /// <summary>
    /// Creates a new PanelShiftPageEntry for a beltpack role.
    /// </summary>
    /// <param name="roleOffset">Offset from the first role ID (600).</param>
    /// <param name="pageNumber">Page number (0 = main, 1-8 = shift pages).</param>
    /// <returns>A new entry configured for beltpack roles.</returns>
    public static PanelShiftPageEntry ForBeltpackRole(byte roleOffset, byte pageNumber = 0)
    {
        return new PanelShiftPageEntry
        {
            CardSlot = BeltpackCardSlot,
            PortOffset = roleOffset,
            PageNumber = pageNumber
        };
    }
}
