using HCILibrary.Enums;

namespace HCILibrary.Models;

/// <summary>
/// Represents a role state entry in the Reply Role State message.
/// </summary>
public class RoleStateEntry
{
    /// <summary>
    /// Role ID (e.g., 600, 601).
    /// </summary>
    public ushort RoleNumber { get; set; }

    /// <summary>
    /// Current allocation status of the role.
    /// </summary>
    public RoleAllocationStatus AllocationStatus { get; set; }

    /// <summary>
    /// Port number of hardware to which the current role is allocated.
    /// Set to 0xFFFF when the role is not currently allocated.
    /// </summary>
    public ushort PhysicalPort { get; set; }

    /// <summary>
    /// Panel type associated with the key configuration in use with this role.
    /// May not match physical endpoint type. Set to 0 when role is not allocated.
    /// </summary>
    public ushort ConfiguredEndpointType { get; set; }

    /// <summary>
    /// Physical panel type of the endpoint currently using this role.
    /// Set to 0 when the role is not currently allocated.
    /// </summary>
    public ushort PhysicalEndpointType { get; set; }

    /// <summary>
    /// Gets whether the role is currently allocated.
    /// </summary>
    public bool IsAllocated => AllocationStatus != RoleAllocationStatus.Free;

    /// <summary>
    /// Gets whether the physical port is valid (not 0xFFFF).
    /// </summary>
    public bool HasPhysicalPort => PhysicalPort != 0xFFFF;

    /// <summary>
    /// Creates a new RoleStateEntry.
    /// </summary>
    public RoleStateEntry()
    {
    }

    /// <summary>
    /// Creates a new RoleStateEntry with the specified values.
    /// </summary>
    public RoleStateEntry(ushort roleNumber, RoleAllocationStatus allocationStatus, 
        ushort physicalPort, ushort configuredEndpointType, ushort physicalEndpointType)
    {
        RoleNumber = roleNumber;
        AllocationStatus = allocationStatus;
        PhysicalPort = physicalPort;
        ConfiguredEndpointType = configuredEndpointType;
        PhysicalEndpointType = physicalEndpointType;
    }

    public override string ToString()
    {
        return $"Role={RoleNumber}, Status={AllocationStatus}, PhysicalPort={PhysicalPort:X4}, " +
               $"ConfiguredType=0x{ConfiguredEndpointType:X4}, PhysicalType=0x{PhysicalEndpointType:X4}";
    }
}
