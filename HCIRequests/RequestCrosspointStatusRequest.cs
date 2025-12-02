using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Crosspoint Status (HCIv2) - Message ID 0x000D.
/// This message asks the matrix for the connection status for each of the specified ports.
/// </summary>
public class RequestCrosspointStatusRequest : HCIRequest
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
    /// Maximum number of ports that can be queried (1-495).
    /// </summary>
    public const int MaxPorts = 495;

    /// <summary>
    /// The list of port numbers to query (0-1023 each).
    /// </summary>
    public List<ushort> Ports { get; } = new();

    /// <summary>
    /// Creates a new Request Crosspoint Status request.
    /// </summary>
    public RequestCrosspointStatusRequest()
        : base(HCIMessageID.RequestCrosspointStatus)
    {
    }

    /// <summary>
    /// Adds a port to the query list.
    /// </summary>
    /// <param name="port">The port number (0-1023).</param>
    public void AddPort(ushort port)
    {
        if (Ports.Count >= MaxPorts)
        {
            throw new InvalidOperationException($"Cannot add more than {MaxPorts} ports.");
        }
        if (port > 1023)
        {
            throw new ArgumentOutOfRangeException(nameof(port), "Port number must be 0-1023.");
        }
        Ports.Add(port);
    }

    /// <summary>
    /// Adds multiple ports to the query list.
    /// </summary>
    /// <param name="ports">The port numbers to add (0-1023 each).</param>
    public void AddPorts(IEnumerable<ushort> ports)
    {
        foreach (var port in ports)
        {
            AddPort(port);
        }
    }

    /// <summary>
    /// Generates the payload for this request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload: ProtocolTag(4) + ProtocolSchema(1) + Count(2) + Port(2) * Count
        var payload = new byte[4 + 1 + 2 + (Ports.Count * 2)];

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Count: 16 bit word (big-endian)
        ushort count = (ushort)Ports.Count;
        payload[offset++] = (byte)(count >> 8);
        payload[offset++] = (byte)(count & 0xFF);

        // Ports: 16 bit word each (big-endian)
        foreach (var port in Ports)
        {
            payload[offset++] = (byte)(port >> 8);
            payload[offset++] = (byte)(port & 0xFF);
        }

        return payload;
    }
}
