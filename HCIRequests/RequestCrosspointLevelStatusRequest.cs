using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Crosspoint Level Status (HCIv2) - Message ID 0x0027.
/// This message will request all non-zero crosspoint level information for the
/// specified destination port(s). This message will cause the matrix to generate
/// a Reply Crosspoint Level Status message.
/// </summary>
public class RequestCrosspointLevelStatusRequest : HCIRequest
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
    /// The list of destination port numbers to query (0-1023 each).
    /// </summary>
    public List<ushort> DestinationPorts { get; } = new();

    /// <summary>
    /// Creates a new Request Crosspoint Level Status request.
    /// </summary>
    public RequestCrosspointLevelStatusRequest()
        : base(HCIMessageID.RequestCrosspointLevelStatus)
    {
    }

    /// <summary>
    /// Adds a destination port to the query list.
    /// </summary>
    /// <param name="destinationPort">The destination port number (0-1023).</param>
    public void AddDestinationPort(ushort destinationPort)
    {
        if (destinationPort > 1023)
        {
            throw new ArgumentOutOfRangeException(nameof(destinationPort), "Port number must be 0-1023.");
        }
        DestinationPorts.Add(destinationPort);
    }

    /// <summary>
    /// Adds multiple destination ports to the query list.
    /// </summary>
    /// <param name="destinationPorts">The destination port numbers to add (0-1023 each).</param>
    public void AddDestinationPorts(IEnumerable<ushort> destinationPorts)
    {
        foreach (var port in destinationPorts)
        {
            AddDestinationPort(port);
        }
    }

    /// <summary>
    /// Generates the payload for this request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload: ProtocolTag(4) + ProtocolSchema(1) + Count(2) + DestPort(2) * Count
        var payload = new byte[4 + 1 + 2 + (DestinationPorts.Count * 2)];

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Count: 16 bit word (big-endian)
        ushort count = (ushort)DestinationPorts.Count;
        payload[offset++] = (byte)(count >> 8);
        payload[offset++] = (byte)(count & 0xFF);

        // Destination Ports: 16 bit word each (big-endian)
        foreach (var port in DestinationPorts)
        {
            payload[offset++] = (byte)(port >> 8);
            payload[offset++] = (byte)(port & 0xFF);
        }

        return payload;
    }
}
