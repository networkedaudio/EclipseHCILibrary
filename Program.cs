using HCILibrary;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Console.WriteLine("HCILibrary - TCP Connection Example");
Console.WriteLine("====================================");

// Example usage - replace with actual IP address
string ipAddress = "192.168.1.100";

using var connection = new HCIConnection(ipAddress, startPort: 52020, endPort: 52001);

// Subscribe to events
connection.MessageReceived += (sender, reply) =>
{
    Console.WriteLine($"Received message: ID={reply.MessageID}, Version={reply.Version}, Length={reply.MessageLength}");
    Console.WriteLine($"  Flags: {reply.Flags}");
    Console.WriteLine($"  Payload: {BitConverter.ToString(reply.Payload)}");
};

connection.ConnectionStateChanged += (sender, isConnected) =>
{
    Console.WriteLine($"Connection state changed: {(isConnected ? "Connected" : "Disconnected")}");
    if (isConnected)
    {
        Console.WriteLine($"  Connected on port: {connection.CurrentPort}");
    }
};

connection.ErrorOccurred += (sender, ex) =>
{
    Console.WriteLine($"Error occurred: {ex.Message}");
};

Console.WriteLine($"Attempting to connect to {ipAddress}...");

bool connected = await connection.ConnectAsync();

if (connected)
{
    Console.WriteLine("Connection successful!");
    Console.WriteLine("Press any key to disconnect and exit...");
    Console.ReadKey();
}
else
{
    Console.WriteLine("Failed to connect on any port.");
}

await connection.DisconnectAsync();
Console.WriteLine("Disconnected. Goodbye!");
