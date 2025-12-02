namespace HCILibrary.Enums;

/// <summary>
/// Action type for Panel Shift Page Action request.
/// </summary>
public enum PanelShiftPageActionType : byte
{
    /// <summary>
    /// Get the current page of the panel.
    /// </summary>
    GetCurrentPage = 0,

    /// <summary>
    /// Set the current page of the panel.
    /// </summary>
    SetCurrentPage = 1
}
