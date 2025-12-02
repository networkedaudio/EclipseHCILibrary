using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Panel Shift Page Action (HCIv2) - Message ID 0x0154 (340).
/// This message is used to respond to the Panel Page Action Request.
/// It contains the current active page of the specified panel.
/// </summary>
public class ReplyPanelShiftPageAction
{
    /// <summary>
    /// Card slot number for beltpack roles.
    /// </summary>
    public const byte BeltpackCardSlot = 0xFF;

    /// <summary>
    /// Protocol schema version (should be 1).
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The action that was performed: Get or Set current page.
    /// 0 = Get Current Page, 1 = Set Current Page.
    /// </summary>
    public PanelShiftPageActionType Action { get; set; }

    /// <summary>
    /// Card slot number.
    /// For beltpack roles, this is 0xFF.
    /// </summary>
    public byte CardSlot { get; set; }

    /// <summary>
    /// Zero-based offset into panel card.
    /// For beltpack roles, this is the offset from the first role ID (600).
    /// </summary>
    public byte PortOffset { get; set; }

    /// <summary>
    /// The current active page number on this panel.
    /// 0 = main page, 1 = first shift page, etc.
    /// </summary>
    public byte PageNumber { get; set; }

    /// <summary>
    /// Gets whether this is a beltpack role.
    /// </summary>
    public bool IsBeltpackRole => CardSlot == BeltpackCardSlot;

    /// <summary>
    /// Decodes a Reply Panel Shift Page Action message from the payload.
    /// </summary>
    /// <param name="payload">The message payload (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyPanelShiftPageAction Decode(byte[] payload)
    {
        var reply = new ReplyPanelShiftPageAction();

        if (payload == null || payload.Length < 9)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip validation, already checked
        offset += 4;

        // Protocol Schema: 1 byte
        if (offset < payload.Length)
            reply.ProtocolSchema = payload[offset++];

        // Action: 1 byte
        if (offset < payload.Length)
            reply.Action = (PanelShiftPageActionType)payload[offset++];

        // Card Slot: 1 byte
        if (offset < payload.Length)
            reply.CardSlot = payload[offset++];

        // Port Offset: 1 byte
        if (offset < payload.Length)
            reply.PortOffset = payload[offset++];

        // Page Number: 1 byte
        if (offset < payload.Length)
            reply.PageNumber = payload[offset++];

        return reply;
    }
}
