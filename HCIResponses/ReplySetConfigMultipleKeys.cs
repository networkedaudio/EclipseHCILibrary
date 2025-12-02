namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Set Config for Multiple Keys (0x00CE).
/// Replies to the Set Config for Multiple Keys message for selected panel.
/// </summary>
public class ReplySetConfigMultipleKeys
{
    /// <summary>
    /// Protocol schema version (1 or 2).
    /// </summary>
    public byte Schema { get; set; }

    /// <summary>
    /// Card slot number.
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    /// Port offset from first port of the card.
    /// </summary>
    public byte Port { get; set; }

    /// <summary>
    /// Endpoint type (only present with Schema 2).
    /// Used when slot and offset target a role.
    /// Set to 0 if not an assignment to a role.
    /// The value in the reply is the value received in the request.
    /// </summary>
    public ushort EndpointType { get; set; }

    /// <summary>
    /// Number of keys processed.
    /// </summary>
    public ushort KeysProcessed { get; set; }

    /// <summary>
    /// Parses the payload of a Reply Set Config Multiple Keys message.
    /// </summary>
    /// <param name="payload">The message payload (after protocol schema byte).</param>
    /// <param name="schema">The protocol schema version.</param>
    /// <returns>The parsed ReplySetConfigMultipleKeys, or null if parsing fails.</returns>
    public static ReplySetConfigMultipleKeys? Parse(byte[] payload, byte schema = 1)
    {
        // Minimum payload:
        // Schema 1: Slot(1) + Port(1) + Keys(2) = 4 bytes
        // Schema 2: Slot(1) + Port(1) + EndpointType(2) + Keys(2) = 6 bytes
        int minLength = schema >= 2 ? 6 : 4;

        if (payload == null || payload.Length < minLength)
        {
            return null;
        }

        var result = new ReplySetConfigMultipleKeys
        {
            Schema = schema
        };

        int offset = 0;

        // Slot: 1 byte
        result.Slot = payload[offset++];

        // Port: 1 byte
        result.Port = payload[offset++];

        // Endpoint Type: 2 bytes (Schema 2 only, big-endian)
        if (schema >= 2)
        {
            result.EndpointType = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;
        }

        // Keys processed: 2 bytes (big-endian)
        result.KeysProcessed = (ushort)((payload[offset] << 8) | payload[offset + 1]);

        return result;
    }
}
