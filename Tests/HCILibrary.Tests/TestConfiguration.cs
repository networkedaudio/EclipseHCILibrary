namespace HCILibrary.Tests;

/// <summary>
/// Configuration settings for EclipseHX matrix connection.
/// </summary>
public class EclipseHXSettings
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
}

/// <summary>
/// Settings for test panel identification.
/// </summary>
public class TestPanelSettings
{
    /// <summary>
    /// Card slot number.
    /// </summary>
    public byte Slot { get; set; } = 1;

    /// <summary>
    /// Port offset from first port of the card.
    /// </summary>
    public byte Port { get; set; } = 0;

    /// <summary>
    /// Key region for testing.
    /// </summary>
    public byte KeyRegion { get; set; } = 0;

    /// <summary>
    /// Key number for testing.
    /// </summary>
    public byte KeyNumber { get; set; } = 1;
}

/// <summary>
/// Settings for test targets (ports, conferences, etc.).
/// </summary>
public class TestTargetSettings
{
    /// <summary>
    /// Source port for crosspoint tests.
    /// </summary>
    public ushort SourcePort { get; set; } = 1;

    /// <summary>
    /// Destination port for crosspoint tests.
    /// </summary>
    public ushort DestinationPort { get; set; } = 2;

    /// <summary>
    /// Conference ID for conference tests.
    /// </summary>
    public ushort ConferenceId { get; set; } = 1;
}

/// <summary>
/// Root configuration for all test settings.
/// </summary>
public class TestConfiguration
{
    public EclipseHXSettings EclipseHX { get; set; } = new();
    public TestPanelSettings TestPanel { get; set; } = new();
    public TestTargetSettings TestTargets { get; set; } = new();
}
