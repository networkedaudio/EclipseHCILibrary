namespace HCILibrary.Enums;

/// <summary>
/// Role allocation status values.
/// </summary>
public enum RoleAllocationStatus : byte
{
    /// <summary>
    /// Role is free/unallocated.
    /// </summary>
    Free = 0,

    /// <summary>
    /// Role is allocated from pool.
    /// </summary>
    AllocatedFromPool = 1,

    /// <summary>
    /// Role is allocated with fixed association.
    /// </summary>
    AllocatedFixed = 2
}
