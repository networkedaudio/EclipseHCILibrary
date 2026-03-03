using System.Xml.Serialization;

namespace EclipseHXSNMP.Models;

/// <summary>
/// Root configuration containing all matrix connections.
/// Serialized to/from XML.
/// </summary>
[XmlRoot("MatrixConfiguration")]
public class MatrixConfiguration
{
    /// <summary>
    /// List of configured matrix connections.
    /// </summary>
    [XmlArray("Matrices")]
    [XmlArrayItem("Matrix")]
    public List<MatrixConnection> Matrices { get; set; } = new();
}
