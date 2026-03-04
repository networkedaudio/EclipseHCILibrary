using HCILibrary.Enums;
using System.Diagnostics;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents the status of a single panel/endpoint from Reply Panel Status (0x001E).
/// Compact 4-byte format: PanelNumLSB(1) + PanelType(1) + Condition(1) + PanelNumMSB(1).
/// </summary>
public class PanelStatus
{
    /// <summary>
    /// Port number (16-bit, split across bytes 0 and 3 of the entry).
    /// </summary>
    public ushort PanelNumber { get; set; }

    /// <summary>
    /// Panel type (8-bit compact type from the status message).
    /// </summary>
    public PanelType PanelType { get; set; }

    /// <summary>
    /// Panel state (bits 0-6 of condition byte).
    /// </summary>
    public PanelState State { get; set; }

    /// <summary>
    /// Whether this is an AoIP device (bit 7 of condition byte).
    /// </summary>
    public bool IsAoipDevice { get; set; }
}

/// <summary>
/// Reply Panel Status (0x001E).
/// Sent in response to Request Panel Status or when panel state changes.
/// A complete list is generated in response to a request; only changed panels
/// are reported in automatically generated messages.
/// Entry format: 4 bytes each — PanelNumLSB(1) + PanelType(1) + Condition(1) + PanelNumMSB(1).
/// </summary>
public class ReplyPanelStatus
{
    /// <summary>
    /// The list of panel statuses.
    /// </summary>
    public List<PanelStatus> Panels { get; } = new();

    /// <summary>
    /// Parses a Reply Panel Status response from the payload bytes.
    /// Payload starts directly with Count(2) followed by 4-byte entries.
    /// </summary>
    /// <param name="payload">The payload bytes (no protocol tag).</param>
    /// <returns>The parsed response.</returns>
    public static ReplyPanelStatus Parse(byte[] payload)
    {
        var result = new ReplyPanelStatus();

        if (payload == null || payload.Length < 2)
        {
            Debug.WriteLine($"[ReplyPanelStatus] Payload too short: {payload?.Length ?? 0} bytes");
            return result;
        }

        int offset = 0;

        // Count (16-bit, big-endian)
        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        Debug.WriteLine($"[ReplyPanelStatus] Count={count}, remaining={payload.Length - offset} bytes, ~{(count > 0 ? (payload.Length - offset) / count : 0)} bytes/entry");

        // Panel data: 4 bytes per entry
        for (int i = 0; i < count; i++)
        {
            if (offset + 4 > payload.Length)
            {
                Debug.WriteLine($"[ReplyPanelStatus] Truncated at entry {i}, offset {offset}");
                break;
            }

            // Panel Num LSB: 1 byte (least significant 8 bits of panel number)
            byte panelNumLsb = payload[offset++];

            // Panel Type: 1 byte
            byte panelType = payload[offset++];

            // Condition: 1 byte (bits 0-6 = state, bit 7 = AoIP)
            byte condition = payload[offset++];

            // Panel Num MSB: 1 byte (most significant 8 bits of panel number)
            byte panelNumMsb = payload[offset++];

            ushort panelNumber = (ushort)((panelNumMsb << 8) | panelNumLsb);

            result.Panels.Add(new PanelStatus
            {
                PanelNumber = panelNumber,
                PanelType = (PanelType)panelType,
                State = (PanelState)(condition & 0x7F),
                IsAoipDevice = (condition & 0x80) != 0
            });
        }

        Debug.WriteLine($"[ReplyPanelStatus] Parsed {result.Panels.Count} of {count} panel(s)");
        return result;
    }

    /// <summary>
    /// Gets the status for a specific panel, or null if not found.
    /// </summary>
    public PanelStatus? GetPanel(ushort panelNumber)
    {
        return Panels.Find(p => p.PanelNumber == panelNumber);
    }

    /// <summary>
    /// Gets all panels that are online (in good state).
    /// </summary>
    public IEnumerable<PanelStatus> GetOnlinePanels()
    {
        return Panels.Where(p => p.State == PanelState.Good);
    }

    /// <summary>
    /// Gets all panels that are offline or faulty.
    /// </summary>
    public IEnumerable<PanelStatus> GetOfflinePanels()
    {
        return Panels.Where(p => p.State != PanelState.Good);
    }
}
