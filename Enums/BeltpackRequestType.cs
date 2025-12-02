namespace HCILibrary.Enums;

/// <summary>
/// Beltpack information request type values.
/// </summary>
public enum BeltpackRequestType : byte
{
    /// <summary>
    /// Get all entries added via map or OTA (not HCI added entries).
    /// </summary>
    MapOrOtaEntries = 1,

    /// <summary>
    /// Only entries that have been added via HCI API.
    /// </summary>
    HciAddedEntries = 2
}
