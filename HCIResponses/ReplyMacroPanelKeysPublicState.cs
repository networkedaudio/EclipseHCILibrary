using HCILibrary.Models;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Reply Macro Panel Keys Public State (HCIv2) - Message ID 0x0173 (371).
/// This message is either sent in response to a Request Macro Panel Keys Public State
/// or it is sent out unsolicited if a panel comes online once the matrix is up
/// and the matrix has received a Request Macro Panel Keys Public State message
/// with the panel field set to indicate 'get all'.
/// </summary>
public class ReplyMacroPanelKeysPublicState
{
    /// <summary>
    /// Protocol schema version.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// List of macro key state entries.
    /// </summary>
    public List<MacroPanelKeyStateEntry> KeyStates { get; set; } = new();

    /// <summary>
    /// Decodes the payload into a ReplyMacroPanelKeysPublicState.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags).</param>
    /// <returns>The decoded reply.</returns>
    public static ReplyMacroPanelKeysPublicState Decode(byte[] payload)
    {
        var reply = new ReplyMacroPanelKeysPublicState();

        if (payload.Length < 6)
            return reply;

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE) - skip
        offset += 4;

        // Protocol Schema: 1 byte
        reply.ProtocolSchema = payload[offset++];

        // Count: 1 byte
        if (offset >= payload.Length)
            return reply;
        byte count = payload[offset++];

        // Parse key entries (9 bytes each)
        for (int i = 0; i < count && offset + 9 <= payload.Length; i++)
        {
            // Port Number: 2 bytes (big-endian)
            ushort portNumber = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Region: 1 byte
            byte region = payload[offset++];

            // Page: 1 byte
            byte page = payload[offset++];

            // Key: 1 byte
            byte key = payload[offset++];

            // Macro Key Group System: 1 byte
            byte macroKeyGroupSystem = payload[offset++];

            // Macro Key Group ID: 2 bytes (big-endian)
            ushort macroKeyGroupId = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Pressed State: 1 byte (0 = Off, 1 = On)
            bool pressedState = payload[offset++] == 1;

            reply.KeyStates.Add(new MacroPanelKeyStateEntry(
                portNumber, region, page, key,
                macroKeyGroupSystem, macroKeyGroupId, pressedState));
        }

        return reply;
    }
}
