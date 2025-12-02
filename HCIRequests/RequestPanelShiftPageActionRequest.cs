using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Panel Shift Page Action (HCIv2) - Message ID 0x0153 (339).
/// This message is used to request the current page of a specified panel,
/// or to request the change of current page of a specified panel.
/// </summary>
public class RequestPanelShiftPageActionRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// The action to perform: Get or Set current page.
    /// 0 = Get Current Page, 1 = Set Current Page.
    /// </summary>
    public PanelShiftPageActionType Action { get; set; } = PanelShiftPageActionType.GetCurrentPage;

    /// <summary>
    /// The list of panel entries to get or set page for.
    /// </summary>
    public List<PanelShiftPageEntry> Entries { get; set; } = new();

    /// <summary>
    /// Creates a new Request Panel Shift Page Action.
    /// </summary>
    public RequestPanelShiftPageActionRequest()
        : base(HCIMessageID.RequestPanelShiftPageAction)
    {
    }

    /// <summary>
    /// Creates a new Request Panel Shift Page Action with the specified action type.
    /// </summary>
    /// <param name="action">The action type (Get or Set).</param>
    public RequestPanelShiftPageActionRequest(PanelShiftPageActionType action)
        : base(HCIMessageID.RequestPanelShiftPageAction)
    {
        Action = action;
    }

    /// <summary>
    /// Adds a panel entry to the request.
    /// </summary>
    /// <param name="cardSlot">Card slot number (0xFF for beltpack roles).</param>
    /// <param name="portOffset">Zero-based offset into panel card or role offset.</param>
    /// <param name="pageNumber">Page number (only used for Set action, 0 = main, 1-8 = shift pages).</param>
    public void AddEntry(byte cardSlot, byte portOffset, byte pageNumber = 0)
    {
        Entries.Add(new PanelShiftPageEntry(cardSlot, portOffset, pageNumber));
    }

    /// <summary>
    /// Adds a beltpack role entry to the request.
    /// </summary>
    /// <param name="roleOffset">Offset from the first role ID (600).</param>
    /// <param name="pageNumber">Page number (only used for Set action, 0 = main, 1-8 = shift pages).</param>
    public void AddBeltpackRoleEntry(byte roleOffset, byte pageNumber = 0)
    {
        Entries.Add(PanelShiftPageEntry.ForBeltpackRole(roleOffset, pageNumber));
    }

    /// <summary>
    /// Generates the payload for the request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // Protocol Tag: 4 bytes (0xABBACEDE)
        // Protocol Schema: 1 byte (set to 1)
        // Action: 1 byte (0 = Get, 1 = Set)
        // Count: 2 bytes (big-endian) - number of entries
        // For each entry:
        //   Card Slot: 1 byte
        //   Port Offset: 1 byte
        //   Page Number: 1 byte (only if Action == Set)

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // Action: 1 byte
        ms.WriteByte((byte)Action);

        // Count: 2 bytes (big-endian)
        ushort count = (ushort)Entries.Count;
        ms.WriteByte((byte)(count >> 8));
        ms.WriteByte((byte)(count & 0xFF));

        // Entries
        foreach (var entry in Entries)
        {
            // Card Slot: 1 byte
            ms.WriteByte(entry.CardSlot);

            // Port Offset: 1 byte
            ms.WriteByte(entry.PortOffset);

            // Page Number: 1 byte (only if Action == Set)
            if (Action == PanelShiftPageActionType.SetCurrentPage)
            {
                ms.WriteByte(entry.PageNumber);
            }
        }

        return ms.ToArray();
    }
}
