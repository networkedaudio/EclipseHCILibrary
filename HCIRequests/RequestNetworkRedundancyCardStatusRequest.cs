using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Network Redundancy Card Status (HCIv2) - Message ID 0x018A (394).
/// This message is used to request the main/standby state of a card.
/// </summary>
public class RequestNetworkRedundancyCardStatusRequest : HCIRequest
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
    /// Actual slot number as per the silk screen on the physical rack.
    /// Use 0xFF to request all E-IPA slots.
    /// </summary>
    public byte Slot { get; set; } = 0xFF;

    /// <summary>
    /// Creates a new Request Network Redundancy Card Status request for all E-IPA slots.
    /// </summary>
    public RequestNetworkRedundancyCardStatusRequest() 
        : base(HCIMessageID.RequestNetworkRedundancyCardStatus)
    {
        ExpectedReplyMessageID = HCIMessageID.ReplyNetworkRedundancyCardStatus;
    }

    /// <summary>
    /// Creates a new Request Network Redundancy Card Status request for a specific slot.
    /// </summary>
    /// <param name="slot">The slot number to query, or 0xFF for all E-IPA slots.</param>
    public RequestNetworkRedundancyCardStatusRequest(byte slot) 
        : base(HCIMessageID.RequestNetworkRedundancyCardStatus)
    {
        Slot = slot;
        ExpectedReplyMessageID = HCIMessageID.ReplyNetworkRedundancyCardStatus;
    }

    /// <summary>
    /// Creates a request to get the network redundancy card status for all E-IPA slots.
    /// </summary>
    /// <returns>A configured request to query all E-IPA slots.</returns>
    public static RequestNetworkRedundancyCardStatusRequest ForAllSlots()
    {
        return new RequestNetworkRedundancyCardStatusRequest(0xFF);
    }

    /// <summary>
    /// Creates a request to get the network redundancy card status for a specific slot.
    /// </summary>
    /// <param name="slot">The slot number to query.</param>
    /// <returns>A configured request to query the specified slot.</returns>
    public static RequestNetworkRedundancyCardStatusRequest ForSlot(byte slot)
    {
        return new RequestNetworkRedundancyCardStatusRequest(slot);
    }

    /// <inheritdoc/>
    protected override byte[] GeneratePayload()
    {
        // Payload: Protocol Tag (4) + Protocol Schema (1) + Slot (1) = 6 bytes
        var payload = new byte[6];
        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Slot: 1 byte
        payload[offset++] = Slot;

        return payload;
    }
}
