using System.Net;
using System.Text;
using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request IP Panel Settings Assign (HCIv2) - Message ID 0x00F7 (247), Sub ID 2.
/// This message is sent to apply IP settings to a specified IP V-Series panel.
/// 
/// Note: This does not work across different networks, unless the panel being
/// configured is already connected to the matrix (i.e., it is a re-assignment).
/// These are IVC type IP panel connections, not AES67.
/// </summary>
public class RequestIPPanelSettingsAssignRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Sub ID for Panel Configuration Request.
    /// </summary>
    public const byte SubIdPanelConfigurationRequest = 2;

    /// <summary>
    /// Entry data length (54 bytes as per protocol).
    /// </summary>
    public const byte EntryLength = 54;

    /// <summary>
    /// The Sub ID for the request. Default is 2 (Panel Configuration Request).
    /// </summary>
    public byte SubId { get; set; } = SubIdPanelConfigurationRequest;

    /// <summary>
    /// Setting mask byte. Set to 1.
    /// </summary>
    public byte SettingMask { get; set; } = 1;

    /// <summary>
    /// Card number. Ignored when setting mask == 1.
    /// </summary>
    public byte CardNumber { get; set; }

    /// <summary>
    /// Card port offset. Ignored when setting mask == 1.
    /// </summary>
    public byte CardPortOffset { get; set; }

    /// <summary>
    /// Login Username (10 bytes, ASCII).
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Login password (10 bytes, ASCII).
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Login server IP address.
    /// </summary>
    public IPAddress ServerIPAddress { get; set; } = IPAddress.Any;

    /// <summary>
    /// Login server port number.
    /// </summary>
    public ushort ServerPort { get; set; }

    /// <summary>
    /// Connection Type: LAN, WAN, Internet, or Default.
    /// Note: Default should be used when possible, otherwise panel may need
    /// to connect twice to apply non-map default type.
    /// </summary>
    public IPPanelConnectionType ConnectionType { get; set; } = IPPanelConnectionType.Default;

    /// <summary>
    /// Network options byte. Set to 1.
    /// </summary>
    public byte NetworkOptions { get; set; } = 1;

    /// <summary>
    /// Panel IP Address.
    /// </summary>
    public IPAddress PanelIPAddress { get; set; } = IPAddress.Any;

    /// <summary>
    /// Panel Subnet mask.
    /// </summary>
    public IPAddress NetworkMask { get; set; } = IPAddress.Any;

    /// <summary>
    /// External Login server IP address (Network Gateway).
    /// </summary>
    public IPAddress NetworkGateway { get; set; } = IPAddress.Any;

    /// <summary>
    /// Panel DNS server IP address.
    /// </summary>
    public IPAddress DNSServerAddress { get; set; } = IPAddress.Any;

    /// <summary>
    /// Panel MAC address (6 bytes).
    /// </summary>
    public byte[] MACAddress { get; set; } = new byte[6];

    /// <summary>
    /// Creates a new Request IP Panel Settings Assign.
    /// </summary>
    public RequestIPPanelSettingsAssignRequest()
        : base(HCIMessageID.RequestIPPanelSettingsAssign)
    {
    }

    /// <summary>
    /// Generates the payload for the request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // Protocol Tag: 4 bytes (0xABBACEDE)
        // Protocol Schema: 1 byte (set to 1)
        // Sub ID: 1 byte (2 = Panel Configuration Request)
        // Entry Length: 1 byte (54)
        // Setting Mask: 1 byte (1)
        // Card Number: 1 byte (ignored when setting mask == 1)
        // Card Port Offset: 1 byte (ignored when setting mask == 1)
        // Username: 10 bytes (ASCII)
        // Password: 10 bytes (ASCII)
        // Server IP Address: 4 bytes
        // Server Port: 2 bytes (big-endian)
        // Connection Type: 1 byte
        // Network Options: 1 byte
        // Panel IP Address: 4 bytes
        // Network Mask: 4 bytes
        // Network Gateway: 4 bytes
        // DNS Server Address: 4 bytes
        // MAC Address: 6 bytes

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // Sub ID: 1 byte
        ms.WriteByte(SubId);

        // Entry Length: 1 byte
        ms.WriteByte(EntryLength);

        // Setting Mask: 1 byte
        ms.WriteByte(SettingMask);

        // Card Number: 1 byte
        ms.WriteByte(CardNumber);

        // Card Port Offset: 1 byte
        ms.WriteByte(CardPortOffset);

        // Username: 10 bytes (ASCII, null-padded)
        WriteFixedString(ms, Username, 10);

        // Password: 10 bytes (ASCII, null-padded)
        WriteFixedString(ms, Password, 10);

        // Server IP Address: 4 bytes
        WriteIPAddress(ms, ServerIPAddress);

        // Server Port: 2 bytes (big-endian)
        ms.WriteByte((byte)(ServerPort >> 8));
        ms.WriteByte((byte)(ServerPort & 0xFF));

        // Connection Type: 1 byte
        ms.WriteByte((byte)ConnectionType);

        // Network Options: 1 byte
        ms.WriteByte(NetworkOptions);

        // Panel IP Address: 4 bytes
        WriteIPAddress(ms, PanelIPAddress);

        // Network Mask: 4 bytes
        WriteIPAddress(ms, NetworkMask);

        // Network Gateway: 4 bytes
        WriteIPAddress(ms, NetworkGateway);

        // DNS Server Address: 4 bytes
        WriteIPAddress(ms, DNSServerAddress);

        // MAC Address: 6 bytes
        var mac = MACAddress ?? new byte[6];
        if (mac.Length < 6)
        {
            var padded = new byte[6];
            Array.Copy(mac, padded, Math.Min(mac.Length, 6));
            mac = padded;
        }
        ms.Write(mac, 0, 6);

        return ms.ToArray();
    }

    /// <summary>
    /// Writes a fixed-length ASCII string to the stream, null-padded if shorter.
    /// </summary>
    private static void WriteFixedString(MemoryStream ms, string value, int length)
    {
        var bytes = Encoding.ASCII.GetBytes(value ?? string.Empty);
        var padded = new byte[length];
        Array.Copy(bytes, padded, Math.Min(bytes.Length, length));
        ms.Write(padded, 0, length);
    }

    /// <summary>
    /// Writes an IP address to the stream (4 bytes).
    /// </summary>
    private static void WriteIPAddress(MemoryStream ms, IPAddress address)
    {
        var bytes = address?.GetAddressBytes() ?? new byte[4];
        if (bytes.Length != 4)
        {
            // Handle IPv6 or invalid addresses - use 0.0.0.0
            bytes = new byte[4];
        }
        ms.Write(bytes, 0, 4);
    }
}
