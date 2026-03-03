using System.Xml.Serialization;

namespace EclipseHXSNMP.Models;

/// <summary>
/// Represents a single matrix connection configuration.
/// </summary>
public class MatrixConnection
{
    /// <summary>
    /// Display name for this matrix.
    /// </summary>
    [XmlAttribute]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// IP address of the matrix.
    /// </summary>
    [XmlAttribute]
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// SNMP port (default 161).
    /// </summary>
    [XmlAttribute]
    public int Port { get; set; } = 161;

    /// <summary>
    /// SNMP community string.
    /// </summary>
    [XmlAttribute]
    public string Community { get; set; } = "public";

    /// <summary>
    /// Whether this matrix is currently enabled for polling.
    /// </summary>
    [XmlAttribute]
    public bool Enabled { get; set; } = true;
}
