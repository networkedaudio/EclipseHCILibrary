namespace HCIExplorer.Models;

/// <summary>
/// Represents a single row in the Frame Status DataGrid.
/// </summary>
public class FrameStatusEntry
{
    /// <summary>
    /// The name of the status item (e.g., "External PSU 1").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The current value/state of the status item (e.g., "OK", "FAILED").
    /// </summary>
    public string Status { get; set; } = string.Empty;
}
