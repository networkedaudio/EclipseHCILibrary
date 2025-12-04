namespace HCILibrary.Tests;

/// <summary>
/// Configuration settings for EclipseHX matrix connection.
/// </summary>
public class TestConfiguration
{
    /// <summary>
    /// IP address of the EclipseHX matrix.
    /// </summary>
    public string IpAddress { get; set; } = "192.168.1.100";

    /// <summary>
    /// Starting port number for connection attempts.
    /// </summary>
    public int StartPort { get; set; } = 52020;

    /// <summary>
    /// Ending port number for connection attempts.
    /// </summary>
    public int EndPort { get; set; } = 52001;

    /// <summary>
    /// Connection timeout in milliseconds.
    /// </summary>
    public int ConnectionTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// Request timeout in milliseconds.
    /// </summary>
    public int RequestTimeoutMs { get; set; } = 10000;

    /// <summary>
    /// Test panel slot number.
    /// </summary>
    public byte Slot { get; set; } = 1;

    /// <summary>
    /// Test panel port offset.
    /// </summary>
    public byte Port { get; set; } = 0;
}
