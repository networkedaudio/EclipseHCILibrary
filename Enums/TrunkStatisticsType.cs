namespace HCILibrary.Enums;

/// <summary>
/// Indicates the category of trunk information in a trunk usage statistics record.
/// </summary>
[Flags]
public enum TrunkStatisticsType : byte
{
    /// <summary>
    /// Fibre trunks.
    /// </summary>
    FibreTrunks = 0x01,

    /// <summary>
    /// Tx trunks.
    /// </summary>
    TxTrunks = 0x02,

    /// <summary>
    /// Reserved trunks.
    /// </summary>
    ReservedTrunks = 0x04,

    /// <summary>
    /// Fibre Shared Resources. When this type is set and the Matrix ID is the local matrix,
    /// a different set of fields (Type 2) is used.
    /// </summary>
    FibreSharedResources = 0x07
}
