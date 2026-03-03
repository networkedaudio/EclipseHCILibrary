namespace EclipseHXSNMP.Models;

/// <summary>
/// Represents a node in the MIB tree for the browser UI.
/// </summary>
public class MibTreeNode
{
    /// <summary>
    /// Display name for this node (e.g., "cardTable", "psuCpuTemperature").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Full OID string (e.g., "1.3.6.1.4.1.99999.1.1.2").
    /// </summary>
    public string Oid { get; set; } = string.Empty;

    /// <summary>
    /// The value retrieved from the SNMP agent, or null if this is a branch node.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Whether this node is expanded in the tree view.
    /// </summary>
    public bool IsExpanded { get; set; }

    /// <summary>
    /// Child nodes.
    /// </summary>
    public List<MibTreeNode> Children { get; set; } = new();

    /// <summary>
    /// Whether this node has any children.
    /// </summary>
    public bool HasChildren => Children.Count > 0;

    /// <summary>
    /// Whether this is a leaf node with a value.
    /// </summary>
    public bool IsLeaf => !HasChildren && Value != null;
}
