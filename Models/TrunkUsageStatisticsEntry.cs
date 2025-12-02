using HCILibrary.Enums;

namespace HCILibrary.Models;

/// <summary>
/// Represents a trunk usage statistics entry (Type 1).
/// Used for all entries except when Fibre Shared Resources type is set
/// and the Matrix ID is the local matrix.
/// </summary>
public class TrunkUsageStatisticsEntry
{
    /// <summary>
    /// ID of the trunk associated matrix.
    /// </summary>
    public byte MatrixId { get; set; }

    /// <summary>
    /// Indicates the category of trunk information in this record.
    /// </summary>
    public TrunkStatisticsType StatisticsType { get; set; }

    /// <summary>
    /// Current trunk usage.
    /// </summary>
    public ushort InUseCount { get; set; }

    /// <summary>
    /// Peak usage in the last 10 minutes.
    /// </summary>
    public ushort TenMinPeak { get; set; }

    /// <summary>
    /// Mean usage in the last 10 minutes.
    /// </summary>
    public ushort TenMinMean { get; set; }

    /// <summary>
    /// Peak usage since the last reset of the matrix.
    /// </summary>
    public ushort PeakSinceStart { get; set; }

    /// <summary>
    /// Total trunks in this category.
    /// </summary>
    public ushort Total { get; set; }

    public override string ToString()
    {
        return $"MatrixId={MatrixId}, Type={StatisticsType}, InUse={InUseCount}, 10MinPeak={TenMinPeak}, 10MinMean={TenMinMean}, PeakSinceStart={PeakSinceStart}, Total={Total}";
    }
}

/// <summary>
/// Represents a trunk usage statistics entry (Type 2) for Fibre Shared Resources
/// when the Matrix ID is the local matrix.
/// </summary>
public class TrunkUsageStatisticsFibreEntry
{
    /// <summary>
    /// ID of the trunk associated matrix.
    /// </summary>
    public byte MatrixId { get; set; }

    /// <summary>
    /// Indicates the category of trunk information in this record.
    /// Should be FibreSharedResources (0x07).
    /// </summary>
    public TrunkStatisticsType StatisticsType { get; set; }

    /// <summary>
    /// Allocated RX port pool count (entire pool).
    /// </summary>
    public ushort AllocatedRxResources { get; set; }

    /// <summary>
    /// Peak number of Rx fibre port resources used (entire pool) since the last matrix reset.
    /// </summary>
    public ushort PeakRxResources { get; set; }

    /// <summary>
    /// Current number of TDM ports allocated in the base fibre driver.
    /// </summary>
    public ushort TdmPortsAllocated { get; set; }

    /// <summary>
    /// Peak number of TDM ports allocated in the base fibre driver since the last matrix reset.
    /// </summary>
    public ushort PeakTdmPortsAllocated { get; set; }

    /// <summary>
    /// Total number of TDM ports available to the fibre driver based on the current configuration.
    /// </summary>
    public ushort TotalTdmPorts { get; set; }

    public override string ToString()
    {
        return $"MatrixId={MatrixId}, Type={StatisticsType}, AllocatedRx={AllocatedRxResources}, PeakRx={PeakRxResources}, TdmAllocated={TdmPortsAllocated}, PeakTdm={PeakTdmPortsAllocated}, TotalTdm={TotalTdmPorts}";
    }
}
