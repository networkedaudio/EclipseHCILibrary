using System.Net;
using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Telephony Client Get State (HCIv2) - Message ID 0x011D (285).
/// This message is used to request the state of the specified, or all Telephony
/// Client ports in the matrix. Telephony client ports supported include TEL-14
/// and SIP client types.
/// </summary>
public class RequestTelephonyClientGetStateRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version.
    /// </summary>
    private const byte ProtocolSchema = 0x01;

    /// <summary>
    /// Special value to request all telephony port states.
    /// </summary>
    public const ushort AllPorts = 0xFFFF;

    /// <summary>
    /// Special IP address value to receive all client states.
    /// </summary>
    public static readonly IPAddress AllServers = new IPAddress(0xFFFFFFFF);

    /// <summary>
    /// The port number to get state for (0-495), or 0xFFFF for all ports.
    /// </summary>
    public ushort Port { get; set; } = AllPorts;

    /// <summary>
    /// The IPv4 IP address of the server.
    /// Set to 0xFFFFFFFF (255.255.255.255) to receive all client states.
    /// A monitoring application may use this option, whereas a Telephony Server
    /// would typically only request the state of clients configured to connect to it.
    /// </summary>
    public IPAddress ServerIPAddress { get; set; } = AllServers;

    /// <summary>
    /// Creates a new Request Telephony Client Get State request for all ports and all servers.
    /// </summary>
    public RequestTelephonyClientGetStateRequest()
        : base(HCIMessageID.RequestTelephonyClientGetState)
    {
    }

    /// <summary>
    /// Creates a new Request Telephony Client Get State request for a specific port.
    /// </summary>
    /// <param name="port">The port number (0-495), or 0xFFFF for all ports.</param>
    public RequestTelephonyClientGetStateRequest(ushort port)
        : base(HCIMessageID.RequestTelephonyClientGetState)
    {
        Port = port;
    }

    /// <summary>
    /// Creates a new Request Telephony Client Get State request for a specific port and server.
    /// </summary>
    /// <param name="port">The port number (0-495), or 0xFFFF for all ports.</param>
    /// <param name="serverIPAddress">The server IP address, or 255.255.255.255 for all servers.</param>
    public RequestTelephonyClientGetStateRequest(ushort port, IPAddress serverIPAddress)
        : base(HCIMessageID.RequestTelephonyClientGetState)
    {
        Port = port;
        ServerIPAddress = serverIPAddress;
    }

    /// <summary>
    /// Configures the request to get state for all telephony ports.
    /// </summary>
    /// <returns>This request instance for method chaining.</returns>
    public RequestTelephonyClientGetStateRequest ForAllPorts()
    {
        Port = AllPorts;
        return this;
    }

    /// <summary>
    /// Configures the request to get state for a specific port.
    /// </summary>
    /// <param name="port">The port number (0-495).</param>
    /// <returns>This request instance for method chaining.</returns>
    public RequestTelephonyClientGetStateRequest ForPort(ushort port)
    {
        Port = port;
        return this;
    }

    /// <summary>
    /// Configures the request to get state from all servers.
    /// </summary>
    /// <returns>This request instance for method chaining.</returns>
    public RequestTelephonyClientGetStateRequest FromAllServers()
    {
        ServerIPAddress = AllServers;
        return this;
    }

    /// <summary>
    /// Configures the request to get state from a specific server.
    /// </summary>
    /// <param name="serverIPAddress">The server IP address.</param>
    /// <returns>This request instance for method chaining.</returns>
    public RequestTelephonyClientGetStateRequest FromServer(IPAddress serverIPAddress)
    {
        ServerIPAddress = serverIPAddress;
        return this;
    }

    /// <summary>
    /// Generates the payload for this request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // ProtocolTag(4) + ProtocolSchema(1) + Count(2) + Port(2) + ServerIPAddress(4)
        var payload = new byte[4 + 1 + 2 + 2 + 4];

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Count: 16 bit word (big-endian) - always 1 for this request
        payload[offset++] = 0x00;
        payload[offset++] = 0x01;

        // Port: 2 bytes (big-endian)
        payload[offset++] = (byte)(Port >> 8);
        payload[offset++] = (byte)(Port & 0xFF);

        // Server IP Address: 4 bytes
        byte[] ipBytes = ServerIPAddress.GetAddressBytes();
        Array.Copy(ipBytes, 0, payload, offset, 4);

        return payload;
    }
}
