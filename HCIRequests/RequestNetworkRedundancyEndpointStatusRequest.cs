using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Network Redundancy Endpoint Status (HCIv2) - Message ID 0x0188 (392).
/// This message is used to request the network redundancy status for one or all endpoints.
/// </summary>
public class RequestNetworkRedundancyEndpointStatusRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version (set to 1).
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// Endpoint port number to query.
    /// Use 0xFFFF to get the status for all endpoints.
    /// </summary>
    public ushort Port { get; set; } = 0xFFFF;

    /// <summary>
    /// Creates a new Request Network Redundancy Endpoint Status request for all endpoints.
    /// </summary>
    public RequestNetworkRedundancyEndpointStatusRequest() 
        : base(HCIMessageID.RequestNetworkRedundancyEndpointStatus)
    {
        ExpectedReplyMessageID = HCIMessageID.ReplyNetworkRedundancyEndpointStatus;
    }

    /// <summary>
    /// Creates a new Request Network Redundancy Endpoint Status request for a specific endpoint.
    /// </summary>
    /// <param name="port">The endpoint port number to query, or 0xFFFF for all endpoints.</param>
    public RequestNetworkRedundancyEndpointStatusRequest(ushort port) 
        : base(HCIMessageID.RequestNetworkRedundancyEndpointStatus)
    {
        Port = port;
        ExpectedReplyMessageID = HCIMessageID.ReplyNetworkRedundancyEndpointStatus;
    }

    /// <summary>
    /// Creates a request to get the network redundancy status for all endpoints.
    /// </summary>
    /// <returns>A configured request to query all endpoints.</returns>
    public static RequestNetworkRedundancyEndpointStatusRequest ForAllEndpoints()
    {
        return new RequestNetworkRedundancyEndpointStatusRequest(0xFFFF);
    }

    /// <summary>
    /// Creates a request to get the network redundancy status for a specific endpoint.
    /// </summary>
    /// <param name="port">The endpoint port number to query.</param>
    /// <returns>A configured request to query the specified endpoint.</returns>
    public static RequestNetworkRedundancyEndpointStatusRequest ForEndpoint(ushort port)
    {
        return new RequestNetworkRedundancyEndpointStatusRequest(port);
    }

    /// <inheritdoc/>
    protected override byte[] GeneratePayload()
    {
        // Payload: Protocol Tag (4) + Protocol Schema (1) + Port (2) = 7 bytes
        var payload = new byte[7];
        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Port: 2 bytes (big-endian)
        payload[offset++] = (byte)(Port >> 8);
        payload[offset++] = (byte)(Port & 0xFF);

        return payload;
    }
}
