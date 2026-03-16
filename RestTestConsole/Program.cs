using System.Text.Json;
using RESTLibrary;

namespace RestTestConsole;

/// <summary>
/// Interactive console for testing the LQ and Arcadia REST APIs.
/// </summary>
internal static class Program
{
    private static readonly JsonSerializerOptions PrettyJson = new()
    {
        WriteIndented = true
    };

    private static LQConnection? _lq;
    private static ArcadiaConnection? _arcadia;

    private static async Task Main()
    {
        Console.Title = "REST API Test Console — LQ / Arcadia";
        WriteHeader();

        while (true)
        {
            WriteColor("\n[Main] ", ConsoleColor.DarkGray);
            Console.Write("(D)iscover  (L)Q  (A)rcadia  (Q)uit > ");
            var key = Console.ReadKey(true).Key;
            Console.WriteLine(key);

            switch (key)
            {
                case ConsoleKey.D:
                    await RunDiscoveryAsync();
                    break;
                case ConsoleKey.L:
                    await RunLqMenuAsync();
                    break;
                case ConsoleKey.A:
                    await RunArcadiaMenuAsync();
                    break;
                case ConsoleKey.Q:
                    Cleanup();
                    return;
            }
        }
    }

    // ──────────────────────── Discovery ────────────────────────

    private static async Task RunDiscoveryAsync()
    {
        WriteColor("\n── mDNS Device Discovery ──\n", ConsoleColor.Yellow);
        WriteColor("Scanning for Clear-Com devices (3 seconds)...\n", ConsoleColor.DarkGray);

        try
        {
            var devices = await DeviceDiscovery.DiscoverAllAsync(TimeSpan.FromSeconds(3));

            if (devices.Count == 0)
            {
                WriteColor("  No devices found.\n", ConsoleColor.Yellow);
                return;
            }

            WriteColor($"  Found {devices.Count} device(s):\n\n", ConsoleColor.Green);

            int index = 1;
            foreach (var device in devices.OrderBy(d => d.Hostname))
            {
                string type = device.IsArcadia ? "Arcadia" : device.IsLQ ? "LQ" : "Unknown";
                var color = device.IsArcadia ? ConsoleColor.Magenta : device.IsLQ ? ConsoleColor.Cyan : ConsoleColor.Gray;
                WriteColor($"  {index,2}) ", ConsoleColor.White);
                WriteColor($"[{type,-7}] ", color);
                Console.WriteLine($"{device.Hostname,-30} {device.Address,-16} :{device.Port}");
                index++;
            }
        }
        catch (Exception ex)
        {
            WriteColor($"  Discovery failed: {ex.Message}\n", ConsoleColor.Red);
        }
    }

    // ─────────────────────────────── LQ ───────────────────────────────

    private static async Task RunLqMenuAsync()
    {
        if (_lq == null || !_lq.IsAuthenticated)
        {
            _lq?.Dispose();
            _lq = await ConnectLqAsync();
            if (_lq == null) return;
        }

        while (true)
        {
            Console.WriteLine();
            WriteColor("═══ LQ Menu ═══\n", ConsoleColor.Cyan);
            Console.WriteLine(" 1) Version              2) Devices");
            Console.WriteLine(" 3) Device by ID         4) Device Live Status");
            Console.WriteLine(" 5) Capabilities         6) Endpoints (all)");
            Console.WriteLine(" 7) Endpoints on device  8) Connections");
            Console.WriteLine(" 9) Connection by ID    10) Connection Live Status");
            Console.WriteLine("11) Interfaces (all)    12) Interface on device");
            Console.WriteLine("13) Roles               14) Calls (all)");
            Console.WriteLine("15) Calls on device     16) IVP Users");
            Console.WriteLine(" 0) Back");
            Console.Write("\n> ");
            var input = Console.ReadLine()?.Trim();

            switch (input)
            {
                case "0": return;
                case "1": await CallAsync("Version", () => _lq.GetVersionAsync()); break;
                case "2": await CallAsync("Devices", () => _lq.GetDevicesAsync()); break;
                case "3":
                    var did = Prompt("Device ID");
                    await CallAsync("Device", () => _lq.GetDeviceByIdAsync(did));
                    break;
                case "4":
                    did = Prompt("Device ID");
                    await CallAsync("Live Status", () => _lq.GetDevicesLiveStatusAsync(did));
                    break;
                case "5": await CallAsync("Capabilities", () => _lq.GetDevicesCapabilitiesAsync()); break;
                case "6": await CallAsync("Endpoints", () => _lq.GetEndpointsOnAllDevicesAsync()); break;
                case "7":
                    did = Prompt("Device ID");
                    await CallAsync("Endpoints", () => _lq.GetEndpointsOnDeviceAsync(did));
                    break;
                case "8": await CallAsync("Connections", () => _lq.GetConnectionsAsync()); break;
                case "9":
                    var cid = Prompt("Connection ID");
                    await CallAsync("Connection", () => _lq.GetConnectionByIdAsync(cid));
                    break;
                case "10": await CallAsync("Connection Live Status", () => _lq.GetConnectionsLiveStatusAsync()); break;
                case "11": await CallAsync("Interfaces", () => _lq.GetInterfacesOnAllDevicesAsync()); break;
                case "12":
                    did = Prompt("Device ID");
                    await CallAsync("Interfaces", () => _lq.GetInterfacesOnDeviceAsync(did));
                    break;
                case "13": await CallAsync("Roles", () => _lq.GetRolesAsync()); break;
                case "14": await CallAsync("All Calls", () => _lq.GetAllCallsAsync()); break;
                case "15":
                    did = Prompt("Device ID");
                    await CallAsync("Calls", () => _lq.GetCallsForDeviceAsync(did));
                    break;
                case "16": await CallAsync("IVP Users", () => _lq.GetIVPUsersAsync()); break;
                default:
                    WriteColor("Invalid choice.\n", ConsoleColor.Yellow);
                    break;
            }
        }
    }

    private static async Task<LQConnection?> ConnectLqAsync()
    {
        WriteColor("\n── Connect to LQ ──\n", ConsoleColor.Cyan);
        var host = Prompt("Host/IP");
        var user = Prompt("Username", "admin");
        var pass = PromptPassword("Password");
        var portStr = Prompt("Port", "443");
        int.TryParse(portStr, out int port);

        var lq = new LQConnection(host, user, pass, port);
        lq.ErrorOccurred += (_, msg) => WriteColor($"  [Error] {msg}\n", ConsoleColor.Red);

        WriteColor("Authenticating...", ConsoleColor.DarkGray);
        if (await lq.AuthenticateAsync())
        {
            WriteColor(" OK\n", ConsoleColor.Green);
            return lq;
        }

        WriteColor(" FAILED\n", ConsoleColor.Red);
        lq.Dispose();
        return null;
    }

    // ─────────────────────────── Arcadia ───────────────────────────

    private static async Task RunArcadiaMenuAsync()
    {
        if (_arcadia == null || !_arcadia.IsAuthenticated)
        {
            _arcadia?.Dispose();
            _arcadia = await ConnectArcadiaAsync();
            if (_arcadia == null) return;
        }

        while (true)
        {
            Console.WriteLine();
            WriteColor("═══ Arcadia Menu ═══\n", ConsoleColor.Magenta);
            Console.WriteLine(" 1) Version              2) Devices");
            Console.WriteLine(" 3) Device by ID         4) Device Live Status");
            Console.WriteLine(" 5) Capabilities         6) Endpoints (all)");
            Console.WriteLine(" 7) Endpoints on device  8) Connections");
            Console.WriteLine(" 9) Connection by ID    10) Connection Live Status");
            Console.WriteLine("11) Interfaces (all)    12) Interface on device");
            Console.WriteLine("13) Ports (all)         14) Ports on interface");
            Console.WriteLine("15) External devices    16) Users (v1)");
            Console.WriteLine("17) Users (v2)          18) Rolesets");
            Console.WriteLine("19) Events              20) Keysets");
            Console.WriteLine("21) Calls (all)         22) Calls on device");
            Console.WriteLine("23) Backup              24) Resource Allocation");
            Console.WriteLine("25) IVP Users           26) Entities");
            Console.WriteLine(" 0) Back");
            Console.Write("\n> ");
            var input = Console.ReadLine()?.Trim();

            switch (input)
            {
                case "0": return;
                case "1": await CallAsync("Version", () => _arcadia.GetVersionAsync()); break;
                case "2": await CallAsync("Devices", () => _arcadia.GetDevicesAsync()); break;
                case "3":
                    var did = Prompt("Device ID");
                    await CallAsync("Device", () => _arcadia.GetDeviceByIdAsync(did));
                    break;
                case "4":
                    did = Prompt("Device ID");
                    await CallAsync("Live Status", () => _arcadia.GetDevicesLiveStatusAsync(did));
                    break;
                case "5": await CallAsync("Capabilities", () => _arcadia.GetDevicesCapabilitiesAsync()); break;
                case "6": await CallAsync("Endpoints", () => _arcadia.GetEndpointsOnAllDevicesAsync()); break;
                case "7":
                    did = Prompt("Device ID");
                    await CallAsync("Endpoints", () => _arcadia.GetEndpointsOnDeviceAsync(did));
                    break;
                case "8": await CallAsync("Connections", () => _arcadia.GetConnectionsAsync()); break;
                case "9":
                    var cid = Prompt("Connection ID");
                    await CallAsync("Connection", () => _arcadia.GetConnectionByIdAsync(cid));
                    break;
                case "10": await CallAsync("Connection Live", () => _arcadia.GetConnectionsLiveStatusAsync()); break;
                case "11": await CallAsync("Interfaces", () => _arcadia.GetInterfacesOnAllDevicesAsync()); break;
                case "12":
                    did = Prompt("Device ID");
                    await CallAsync("Interfaces", () => _arcadia.GetInterfacesOnDeviceAsync(did));
                    break;
                case "13": await CallAsync("Ports", () => _arcadia.GetPortsOnDeviceAsync()); break;
                case "14":
                    did = Prompt("Device ID");
                    var iid = Prompt("Interface ID");
                    await CallAsync("Ports", () => _arcadia.GetPortsOnInterfaceOnDeviceAsync(did, iid));
                    break;
                case "15": await CallAsync("External Devices", () => _arcadia.GetExternalDevicesAsync()); break;
                case "16": await CallAsync("Users (v1)", () => _arcadia.GetUsers1Async()); break;
                case "17": await CallAsync("Users (v2)", () => _arcadia.GetUsers2Async()); break;
                case "18": await CallAsync("Rolesets", () => _arcadia.GetRolesetsAsync()); break;
                case "19": await CallAsync("Events", () => _arcadia.GetEventsAsync()); break;
                case "20": await CallAsync("Keysets", () => _arcadia.GetKeysetsV2Async()); break;
                case "21": await CallAsync("All Calls", () => _arcadia.GetAllCallsAsync()); break;
                case "22":
                    did = Prompt("Device ID");
                    await CallAsync("Calls", () => _arcadia.GetCallsForDeviceAsync(did));
                    break;
                case "23": await CallAsync("Backup", () => _arcadia.BackupAsync()); break;
                case "24": await CallAsync("Resource Allocation", () => _arcadia.GetResourceAllocationOptionsAsync()); break;
                case "25": await CallAsync("IVP Users", () => _arcadia.GetIVPUsersAsync()); break;
                case "26": await CallAsync("Entities", () => _arcadia.GetEntitiesAsync()); break;
                default:
                    WriteColor("Invalid choice.\n", ConsoleColor.Yellow);
                    break;
            }
        }
    }

    private static async Task<ArcadiaConnection?> ConnectArcadiaAsync()
    {
        WriteColor("\n── Connect to Arcadia ──\n", ConsoleColor.Magenta);
        var host = Prompt("Host/IP");
        var user = Prompt("Username", "admin");
        var pass = PromptPassword("Password");
        var portStr = Prompt("Port", "443");
        int.TryParse(portStr, out int port);

        var arcadia = new ArcadiaConnection(host, user, pass, port);
        arcadia.ErrorOccurred += (_, msg) => WriteColor($"  [Error] {msg}\n", ConsoleColor.Red);

        WriteColor("Authenticating...", ConsoleColor.DarkGray);
        if (await arcadia.AuthenticateAsync())
        {
            WriteColor(" OK\n", ConsoleColor.Green);
            return arcadia;
        }

        WriteColor(" FAILED\n", ConsoleColor.Red);
        arcadia.Dispose();
        return null;
    }

    // ─────────────────────── Helpers ───────────────────────

    private static async Task CallAsync(string label, Func<Task<RESTLibrary.Models.RestResult<JsonElement>>> call)
    {
        WriteColor($"\n── {label} ──\n", ConsoleColor.DarkYellow);
        try
        {
            var result = await call();
            WriteColor($"  Status: {result.StatusCode}\n", result.IsSuccess ? ConsoleColor.Green : ConsoleColor.Red);

            if (result.IsSuccess && result.Data.ValueKind != JsonValueKind.Undefined)
            {
                var pretty = JsonSerializer.Serialize(result.Data, PrettyJson);
                Console.WriteLine(pretty);
            }
            else if (!result.IsSuccess)
            {
                WriteColor($"  Error: {result.Error}\n", ConsoleColor.Red);
                if (!string.IsNullOrEmpty(result.RawJson))
                    Console.WriteLine(result.RawJson);
            }
        }
        catch (Exception ex)
        {
            WriteColor($"  Exception: {ex.Message}\n", ConsoleColor.Red);
        }
    }

    private static string Prompt(string label, string? defaultValue = null)
    {
        if (defaultValue != null)
            Console.Write($"  {label} [{defaultValue}]: ");
        else
            Console.Write($"  {label}: ");

        var input = Console.ReadLine()?.Trim();
        return string.IsNullOrEmpty(input) && defaultValue != null ? defaultValue : input ?? string.Empty;
    }

    private static string PromptPassword(string label)
    {
        Console.Write($"  {label}: ");
        var password = string.Empty;
        ConsoleKeyInfo key;
        while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[..^1];
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password += key.KeyChar;
                Console.Write("*");
            }
        }
        Console.WriteLine();
        return password;
    }

    private static void WriteHeader()
    {
        WriteColor(@"
  ╔══════════════════════════════════════════╗
  ║   Clear-Com REST API Test Console        ║
  ║   LQ & Arcadia                           ║
  ╚══════════════════════════════════════════╝
", ConsoleColor.Cyan);
    }

    private static void WriteColor(string text, ConsoleColor color)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = prev;
    }

    private static void Cleanup()
    {
        _lq?.Dispose();
        _arcadia?.Dispose();
    }
}
