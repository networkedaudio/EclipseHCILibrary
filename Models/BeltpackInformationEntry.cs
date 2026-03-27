using HCILibrary.Enums;

namespace HCILibrary.Models;

/// <summary>
/// Represents a beltpack information entry in the Reply Beltpack Information message.
/// </summary>
public class BeltpackInformationEntry
{
    /// <summary>
    /// Beltpack serial number.
    /// </summary>
    public uint SerialNumber { get; set; }

    /// <summary>
    /// Beltpack PMID (unique identifier).
    /// </summary>
    public uint Pmid { get; set; }

    /// <summary>
    /// Default role assigned to the beltpack.
    /// </summary>
    public ushort DefaultRole { get; set; }

    /// <summary>
    /// Gets the default role as a 1-indexed value (adds 1 to the 0-indexed DefaultRole).
    /// Use this for display purposes where roles are numbered starting from 1.
    /// </summary>
    public ushort DefaultRoleOneIndexed => (ushort)(DefaultRole + 1);

    /// <summary>
    /// Beltpack configuration mode.
    /// </summary>
    public BeltpackConfigMode Mode { get; set; }

    /// <summary>
    /// Gets whether the beltpack mode is set.
    /// </summary>
    public bool IsModeSet => Mode != BeltpackConfigMode.NotSet;

    /// <summary>
    /// Gets whether the beltpack is in pool mode.
    /// </summary>
    public bool IsPoolMode => Mode == BeltpackConfigMode.Pool;

    /// <summary>
    /// Gets whether the beltpack is in fixed role mode.
    /// </summary>
    public bool IsFixedRole => Mode == BeltpackConfigMode.FixedRole;

    /// <summary>
    /// Creates a new BeltpackInformationEntry.
    /// </summary>
    public BeltpackInformationEntry()
    {
    }

    /// <summary>
    /// Creates a new BeltpackInformationEntry with specified values.
    /// </summary>
    /// <param name="serialNumber">The beltpack serial number.</param>
    /// <param name="pmid">The beltpack PMID.</param>
    /// <param name="defaultRole">The default role.</param>
    /// <param name="mode">The configuration mode.</param>
    public BeltpackInformationEntry(uint serialNumber, uint pmid, ushort defaultRole, BeltpackConfigMode mode)
    {
        SerialNumber = serialNumber;
        Pmid = pmid;
        DefaultRole = defaultRole;
        Mode = mode;
    }
}
