using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Rack Properties: Active Configuration Bank (Message ID 0x002C, Sub Message ID 5).
/// Contains the currently active configuration bank number.
/// </summary>
public class ReplyRackPropertiesConfigBank
{
    /// <summary>
    /// Gets or sets the protocol schema version.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// Gets or sets the Sub Message ID.
    /// Should be <see cref="RackPropertySubMessageId.ConfigBankReply"/> (5).
    /// </summary>
    public RackPropertySubMessageId SubMessageId { get; set; }

    /// <summary>
    /// Gets or sets whether the request was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the active configuration bank number (0-4).
    /// 0 indicates the embedded default map is currently in use.
    /// </summary>
    public byte BankNumber { get; set; }

    /// <summary>
    /// Gets whether the embedded default map is in use.
    /// </summary>
    public bool IsDefaultMap => BankNumber == 0;

    /// <summary>
    /// Decodes a Reply Rack Properties: Active Configuration Bank message from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes.</param>
    /// <returns>The decoded ReplyRackPropertiesConfigBank.</returns>
    public static ReplyRackPropertiesConfigBank Decode(byte[] payload)
    {
        var result = new ReplyRackPropertiesConfigBank();

        if (payload == null || payload.Length < 1)
        {
            return result;
        }

        int offset = 0;

        // Check for HCIv2 marker (0xAB 0xBA 0xCE 0xDE)
        if (payload.Length >= 4 &&
            payload[0] == 0xAB && payload[1] == 0xBA &&
            payload[2] == 0xCE && payload[3] == 0xDE)
        {
            offset = 4;

            // Read schema byte
            if (offset < payload.Length)
            {
                result.Schema = payload[offset++];
            }
        }

        // Sub Message ID (2 bytes, big-endian)
        if (offset + 2 > payload.Length)
        {
            return result;
        }

        result.SubMessageId = (RackPropertySubMessageId)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Success (1 byte)
        if (offset < payload.Length)
        {
            result.Success = payload[offset++] == 1;
        }

        // Bank Number (1 byte)
        if (offset < payload.Length)
        {
            result.BankNumber = payload[offset++];
        }

        return result;
    }

    /// <summary>
    /// Returns a string representation of this rack properties config bank reply.
    /// </summary>
    public override string ToString()
    {
        if (Success)
        {
            return IsDefaultMap
                ? "Active Bank: Default Map (embedded)"
                : $"Active Bank: {BankNumber}";
        }
        return "Config Bank Request Failed";
    }
}
