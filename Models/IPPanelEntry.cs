using System.Net;
using System.Text;

namespace HCILibrary.Models;

/// <summary>
/// Represents a single IP panel entry from the discovered IP panel cache.
/// </summary>
public class IPPanelEntry
{
    /// <summary>
    /// Data length of entry (55 bytes as per protocol).
    /// </summary>
    public byte EntryLength { get; set; }

    /// <summary>
    /// Login Username (10 bytes, ASCII).
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Panel IP Address.
    /// </summary>
    public IPAddress PanelIPAddress { get; set; } = IPAddress.None;

    /// <summary>
    /// Login server port number.
    /// </summary>
    public ushort ServerPort { get; set; }

    /// <summary>
    /// Login server IP address.
    /// </summary>
    public IPAddress ServerIPAddress { get; set; } = IPAddress.None;

    /// <summary>
    /// Panel Subnet mask.
    /// </summary>
    public IPAddress NetworkMask { get; set; } = IPAddress.None;

    /// <summary>
    /// External Login server IP address (Network Gateway).
    /// </summary>
    public IPAddress NetworkGateway { get; set; } = IPAddress.None;

    /// <summary>
    /// Panel DNS server IP address.
    /// </summary>
    public IPAddress DNSServerAddress { get; set; } = IPAddress.None;

    /// <summary>
    /// Panel MAC address (6 bytes).
    /// </summary>
    public byte[] MACAddress { get; set; } = new byte[6];

    /// <summary>
    /// Gets the MAC address as a formatted string (e.g., "00:11:22:33:44:55").
    /// </summary>
    public string MACAddressString => BitConverter.ToString(MACAddress).Replace("-", ":");

    /// <summary>
    /// Network options byte.
    /// </summary>
    public byte NetOptions { get; set; }

    /// <summary>
    /// Connection Type: LAN, WAN or Internet VOIP connection type.
    /// </summary>
    public byte ConnectionType { get; set; }

    /// <summary>
    /// Login Status (2 bytes).
    /// </summary>
    public ushort LoginStatus { get; set; }

    /// <summary>
    /// Transaction Number (2 bytes).
    /// </summary>
    public ushort TransactionNumber { get; set; }

    /// <summary>
    /// V-Series Panel Type (2 bytes).
    /// </summary>
    public ushort PanelType { get; set; }

    /// <summary>
    /// Number of seconds since last boot.
    /// </summary>
    public uint PanelUpTime { get; set; }

    /// <summary>
    /// Number of seconds that current connection has been up.
    /// </summary>
    public uint ConnectionUpTime { get; set; }

    /// <summary>
    /// Decodes an IP panel entry from the payload at the specified offset.
    /// </summary>
    /// <param name="payload">The message payload.</param>
    /// <param name="offset">The starting offset for this entry.</param>
    /// <returns>The decoded entry and the new offset after reading.</returns>
    public static (IPPanelEntry Entry, int NewOffset) Decode(byte[] payload, int offset)
    {
        var entry = new IPPanelEntry();

        if (payload == null || offset >= payload.Length)
            return (entry, offset);

        // Entry Length: 1 byte
        entry.EntryLength = payload[offset++];

        // Username: 10 bytes (ASCII)
        if (offset + 10 <= payload.Length)
        {
            entry.Username = Encoding.ASCII.GetString(payload, offset, 10).TrimEnd('\0');
            offset += 10;
        }

        // Panel IP Address: 4 bytes
        if (offset + 4 <= payload.Length)
        {
            entry.PanelIPAddress = new IPAddress(new ReadOnlySpan<byte>(payload, offset, 4));
            offset += 4;
        }

        // Server Port: 2 bytes (big-endian)
        if (offset + 2 <= payload.Length)
        {
            entry.ServerPort = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Server IP Address: 4 bytes
        if (offset + 4 <= payload.Length)
        {
            entry.ServerIPAddress = new IPAddress(new ReadOnlySpan<byte>(payload, offset, 4));
            offset += 4;
        }

        // Network Mask: 4 bytes
        if (offset + 4 <= payload.Length)
        {
            entry.NetworkMask = new IPAddress(new ReadOnlySpan<byte>(payload, offset, 4));
            offset += 4;
        }

        // Network Gateway: 4 bytes
        if (offset + 4 <= payload.Length)
        {
            entry.NetworkGateway = new IPAddress(new ReadOnlySpan<byte>(payload, offset, 4));
            offset += 4;
        }

        // DNS Server Address: 4 bytes
        if (offset + 4 <= payload.Length)
        {
            entry.DNSServerAddress = new IPAddress(new ReadOnlySpan<byte>(payload, offset, 4));
            offset += 4;
        }

        // MAC Address: 6 bytes
        if (offset + 6 <= payload.Length)
        {
            Array.Copy(payload, offset, entry.MACAddress, 0, 6);
            offset += 6;
        }

        // Net Options: 1 byte
        if (offset < payload.Length)
            entry.NetOptions = payload[offset++];

        // Connection Type: 1 byte
        if (offset < payload.Length)
            entry.ConnectionType = payload[offset++];

        // Login Status: 2 bytes (big-endian)
        if (offset + 2 <= payload.Length)
        {
            entry.LoginStatus = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Transaction Number: 2 bytes (big-endian)
        if (offset + 2 <= payload.Length)
        {
            entry.TransactionNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Panel Type: 2 bytes (big-endian)
        if (offset + 2 <= payload.Length)
        {
            entry.PanelType = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Panel Up Time: 4 bytes (big-endian)
        if (offset + 4 <= payload.Length)
        {
            entry.PanelUpTime = (uint)((payload[offset] << 24) | (payload[offset + 1] << 16) |
                                       (payload[offset + 2] << 8) | payload[offset + 3]);
            offset += 4;
        }

        // Connection Up Time: 4 bytes (big-endian)
        if (offset + 4 <= payload.Length)
        {
            entry.ConnectionUpTime = (uint)((payload[offset] << 24) | (payload[offset + 1] << 16) |
                                            (payload[offset + 2] << 8) | payload[offset + 3]);
            offset += 4;
        }

        return (entry, offset);
    }
}
