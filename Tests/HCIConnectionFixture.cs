using System.Text.Json;

namespace HCILibrary.Tests;

/// <summary>
/// Fixture for sharing HCI connection and configuration across tests.
/// </summary>
public class HCIConnectionFixture : IAsyncLifetime
{
    private readonly TestConfiguration _config;
    
    public HCIConnection? Connection { get; private set; }
    public TestConfiguration Config => _config;
    public bool IsConnected => Connection?.IsConnected ?? false;

    public HCIConnectionFixture()
    {
        _config = new TestConfiguration();
        
        // Load configuration from testsettings.json if it exists
        var settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "testsettings.json");
        if (File.Exists(settingsPath))
        {
            try
            {
                var json = File.ReadAllText(settingsPath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                
                if (root.TryGetProperty("EclipseHX", out var eclipseHx))
                {
                    if (eclipseHx.TryGetProperty("IpAddress", out var ip))
                        _config.IpAddress = ip.GetString() ?? _config.IpAddress;
                    if (eclipseHx.TryGetProperty("StartPort", out var sp))
                        _config.StartPort = sp.GetInt32();
                    if (eclipseHx.TryGetProperty("EndPort", out var ep))
                        _config.EndPort = ep.GetInt32();
                    if (eclipseHx.TryGetProperty("ConnectionTimeoutMs", out var ct))
                        _config.ConnectionTimeoutMs = ct.GetInt32();
                    if (eclipseHx.TryGetProperty("RequestTimeoutMs", out var rt))
                        _config.RequestTimeoutMs = rt.GetInt32();
                }
                
                if (root.TryGetProperty("TestPanel", out var panel))
                {
                    if (panel.TryGetProperty("Slot", out var slot))
                        _config.Slot = (byte)slot.GetInt32();
                    if (panel.TryGetProperty("Port", out var port))
                        _config.Port = (byte)port.GetInt32();
                }
            }
            catch
            {
                // Use defaults if JSON parsing fails
            }
        }
    }

    public async Task InitializeAsync()
    {
        Connection = new HCIConnection(
            _config.IpAddress,
            _config.StartPort,
            _config.EndPort,
            _config.ConnectionTimeoutMs
        );

        // Try to connect, but don't fail if not available
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await Connection.ConnectAsync(cts.Token);
        }
        catch
        {
            // Connection may not be available during unit testing
        }
    }

    public async Task DisposeAsync()
    {
        if (Connection != null)
        {
            await Connection.DisconnectAsync();
            Connection.Dispose();
        }
    }
}

/// <summary>
/// Collection definition for tests that share the HCI connection.
/// </summary>
[CollectionDefinition("HCI Connection")]
public class HCIConnectionCollection : ICollectionFixture<HCIConnectionFixture>
{
}
