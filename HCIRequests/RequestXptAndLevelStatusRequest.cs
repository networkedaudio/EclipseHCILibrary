using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Xpt and Level Status (HCIv2) - Message ID 0x0096 (150).
/// Requests the broadcast of all crosspoints currently in the matrix
/// along with their current level.
/// </summary>
public class RequestXptAndLevelStatusRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol version (set to 1).
    /// </summary>
    public ushort Version { get; set; } = 1;

    /// <summary>
    /// List of port numbers for which crosspoints are being requested.
    /// </summary>
    public List<ushort> PortNumbers { get; set; } = new();

    /// <summary>
    /// Creates a new Request Xpt and Level Status.
    /// </summary>
    public RequestXptAndLevelStatusRequest()
        : base(HCIMessageID.RequestXptAndLevelStatus)
    {
    }

    /// <summary>
    /// Creates a new Request Xpt and Level Status with the specified port numbers.
    /// </summary>
    /// <param name="portNumbers">The port numbers for which crosspoints are being requested.</param>
    public RequestXptAndLevelStatusRequest(params ushort[] portNumbers)
        : base(HCIMessageID.RequestXptAndLevelStatus)
    {
        PortNumbers = new List<ushort>(portNumbers);
    }

    /// <summary>
    /// Creates a new Request Xpt and Level Status with the specified port numbers.
    /// </summary>
    /// <param name="portNumbers">The port numbers for which crosspoints are being requested.</param>
    public RequestXptAndLevelStatusRequest(IEnumerable<ushort> portNumbers)
        : base(HCIMessageID.RequestXptAndLevelStatus)
    {
        PortNumbers = new List<ushort>(portNumbers);
    }

    /// <summary>
    /// Adds a port number to the request.
    /// </summary>
    /// <param name="portNumber">The port number to add.</param>
    public void AddPort(ushort portNumber)
    {
        PortNumbers.Add(portNumber);
    }

    /// <summary>
    /// Adds multiple port numbers to the request.
    /// </summary>
    /// <param name="portNumbers">The port numbers to add.</param>
    public void AddPorts(params ushort[] portNumbers)
    {
        PortNumbers.AddRange(portNumbers);
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
        // Version: 2 bytes (big-endian, set to 1)
        // Count: 2 bytes (big-endian) - number of ports
        // For each port:
        //   Port Number: 2 bytes (big-endian)

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // Version: 2 bytes (big-endian)
        ms.WriteByte((byte)(Version >> 8));
        ms.WriteByte((byte)(Version & 0xFF));

        // Count: 2 bytes (big-endian)
        ushort count = (ushort)PortNumbers.Count;
        ms.WriteByte((byte)(count >> 8));
        ms.WriteByte((byte)(count & 0xFF));

        // Port Numbers
        foreach (var portNumber in PortNumbers)
        {
            ms.WriteByte((byte)(portNumber >> 8));
            ms.WriteByte((byte)(portNumber & 0xFF));
        }

        return ms.ToArray();
    }
}
