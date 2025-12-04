using Microsoft.Extensions.Configuration;

namespace HCILibrary.Tests;

/// <summary>
/// Fixture for sharing HCI connection across tests.
/// Loads configuration and manages connection lifecycle.
/// </summary>
public class HCIConnectionFixture : IAsyncLifetime
{
    private readonly TestConfiguration _config;
    
    public HCIConnection? Connection { get; private set; }
    public TestConfiguration Config => _config;
    public bool IsConnected => Connection?.IsConnected ?? false;

    public HCIConnectionFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testsettings.json", optional: false, reloadOnChange: true)
            .Build();

        _config = new TestConfiguration();
        configuration.Bind(_config);
    }

    public async Task InitializeAsync()
    {
        Connection = new HCIConnection(
            _config.EclipseHX.IpAddress,
            _config.EclipseHX.StartPort,
            _config.EclipseHX.EndPort,
            _config.EclipseHX.ConnectionTimeoutMs
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
