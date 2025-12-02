using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply IP Panel Settings Assign (HCIv2) - Message ID 0x00F8 (248), Sub ID 3.
/// This message is sent in reply to the IP Panel Settings Apply Request.
/// </summary>
public class ReplyIPPanelSettingsAssign
{
    /// <summary>
    /// Sub ID for Panel Configuration Reply.
    /// </summary>
    public const byte SubIdPanelConfigurationReply = 3;

    /// <summary>
    /// Protocol schema version (should be 1).
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The Sub ID for the reply. Should be 3 (Panel Configuration Reply).
    /// </summary>
    public byte SubId { get; set; }

    /// <summary>
    /// Status of the operation.
    /// 0 = Ok, -1 = length error, -2 = unpack error.
    /// </summary>
    public IPPanelSettingsAssignStatus Status { get; set; }

    /// <summary>
    /// Panel MAC address (6 bytes).
    /// </summary>
    public byte[] MACAddress { get; set; } = new byte[6];

    /// <summary>
    /// Gets the MAC address as a formatted string (e.g., "00:11:22:33:44:55").
    /// </summary>
    public string MACAddressString => BitConverter.ToString(MACAddress).Replace("-", ":");

    /// <summary>
    /// Gets whether the operation was successful.
    /// </summary>
    public bool IsSuccess => Status == IPPanelSettingsAssignStatus.Ok;

    /// <summary>
    /// Decodes a Reply IP Panel Settings Assign message from the payload.
    /// </summary>
    /// <param name="payload">The message payload (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyIPPanelSettingsAssign Decode(byte[] payload)
    {
        var reply = new ReplyIPPanelSettingsAssign();

        if (payload == null || payload.Length < 7)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip validation, already checked
        offset += 4;

        // Protocol Schema: 1 byte
        if (offset < payload.Length)
            reply.ProtocolSchema = payload[offset++];

        // Sub ID: 1 byte (should be 3)
        if (offset < payload.Length)
            reply.SubId = payload[offset++];

        // Status: 1 byte (signed)
        if (offset < payload.Length)
            reply.Status = (IPPanelSettingsAssignStatus)(sbyte)payload[offset++];

        // MAC Address: 6 bytes
        if (offset + 6 <= payload.Length)
        {
            Array.Copy(payload, offset, reply.MACAddress, 0, 6);
            offset += 6;
        }

        return reply;
    }
}
