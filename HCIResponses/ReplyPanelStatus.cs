using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents the status of a single panel/endpoint.
/// </summary>
public class PanelStatus
{
    /// <summary>
    /// Panel number (16-bit, combined from LSB and MSB bytes).
    /// </summary>
    public ushort PanelNumber { get; set; }

    /// <summary>
    /// Panel type.
    /// </summary>
    public PanelType PanelType { get; set; }

    /// <summary>
    /// Panel state (unknown, good, faulty, reserved).
    /// </summary>
    public PanelState State { get; set; }

    /// <summary>
    /// Indicates if this is an AoIP (Audio over IP) device.
    /// </summary>
    public bool IsAoipDevice { get; set; }
}

/// <summary>
/// Reply Panel Status (0x001E).
/// Sent in response to Request Panel Status or when panel state changes.
/// A complete list is generated in response to a request; only changed panels
/// are reported in automatically generated messages.
/// </summary>
public class ReplyPanelStatus
{
    /// <summary>
    /// Protocol schema version from the response.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The list of panel statuses.
    /// </summary>
    public List<PanelStatus> Panels { get; } = new();

    /// <summary>
    /// Parses a Reply Panel Status response from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags, starting at protocol tag).</param>
    /// <returns>The parsed response.</returns>
    public static ReplyPanelStatus Parse(byte[] payload)
    {
        var result = new ReplyPanelStatus();
        int offset = 0;

        // Skip protocol tag (4 bytes) - already validated by caller
        offset += 4;

        // Protocol schema (1 byte)
        result.ProtocolSchema = payload[offset++];

        // Count (16-bit, big-endian)
        ushort count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Panel data (4 bytes each)
        for (int i = 0; i < count; i++)
        {
            if (offset + 4 <= payload.Length)
            {
                byte panelNumLsb = payload[offset++];
                byte panelType = payload[offset++];
                byte condition = payload[offset++];
                byte panelNumMsb = payload[offset++];

                var status = new PanelStatus
                {
                    PanelNumber = (ushort)((panelNumMsb << 8) | panelNumLsb),
                    PanelType = (PanelType)panelType,
                    State = (PanelState)(condition & 0x7F),  // bits 0-6
                    IsAoipDevice = (condition & 0x80) != 0   // bit 7
                };
                result.Panels.Add(status);
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the status for a specific panel, or null if not found.
    /// </summary>
    /// <param name="panelNumber">The panel number to look up.</param>
    /// <returns>The panel status, or null if not in the response.</returns>
    public PanelStatus? GetPanel(ushort panelNumber)
    {
        return Panels.Find(p => p.PanelNumber == panelNumber);
    }

    /// <summary>
    /// Gets all panels that are online (in good state).
    /// </summary>
    /// <returns>List of online panels.</returns>
    public IEnumerable<PanelStatus> GetOnlinePanels()
    {
        return Panels.Where(p => p.State == PanelState.Good);
    }

    /// <summary>
    /// Gets all panels that are offline or faulty.
    /// </summary>
    /// <returns>List of offline/faulty panels.</returns>
    public IEnumerable<PanelStatus> GetOfflinePanels()
    {
        return Panels.Where(p => p.State != PanelState.Good);
    }
}
