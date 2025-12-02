namespace HCILibrary.Models;

/// <summary>
/// Represents a single conference action for Request Conference Actions message.
/// </summary>
public class ConferenceAction
{
    /// <summary>
    /// Direction of the action: false = delete, true = add.
    /// </summary>
    public bool IsAdd { get; set; }

    /// <summary>
    /// Port number (0-1023, 10 bits).
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// false = listen, true = talk.
    /// </summary>
    public bool IsTalk { get; set; }

    /// <summary>
    /// Conference number (0-1023, 10 bits).
    /// </summary>
    public ushort ConferenceNumber { get; set; }

    /// <summary>
    /// Creates a new conference action.
    /// </summary>
    /// <param name="isAdd">true to add, false to delete.</param>
    /// <param name="portNumber">The port number (0-1023).</param>
    /// <param name="isTalk">true for talk, false for listen.</param>
    /// <param name="conferenceNumber">The conference number (0-1023).</param>
    public ConferenceAction(bool isAdd, ushort portNumber, bool isTalk, ushort conferenceNumber)
    {
        IsAdd = isAdd;
        PortNumber = portNumber;
        IsTalk = isTalk;
        ConferenceNumber = conferenceNumber;
    }

    /// <summary>
    /// Generates the 4-word (8 byte) action data.
    /// </summary>
    /// <returns>The action data bytes.</returns>
    public byte[] ToBytes()
    {
        var bytes = new byte[8];

        // Word 0:
        // bit 0: direction bit, 0 = delete, 1 = add
        // bit 1,2: port number, bits 8 & 9
        // bit 3: 0 = listen, 1 = talk
        // bits 4-9: 0
        // bit 10: 1
        // bits 11-12: conference number, bits 8 & 9
        // bits 13-15: 0
        ushort word0 = 0;
        if (IsAdd) word0 |= 0x0001;                                      // bit 0
        word0 |= (ushort)(((PortNumber >> 8) & 0x03) << 1);              // bits 1-2: port bits 8-9
        if (IsTalk) word0 |= 0x0008;                                     // bit 3
        word0 |= 0x0400;                                                  // bit 10: always 1
        word0 |= (ushort)(((ConferenceNumber >> 8) & 0x03) << 11);       // bits 11-12: conf bits 8-9

        // Word 1:
        // bits 0-7: port number, bits 0-7
        // bits 8-13: conference number, bits 0-7 (actually 6 bits here based on spec, but storing full byte)
        // bits 14,15: 0
        ushort word1 = 0;
        word1 |= (ushort)(PortNumber & 0xFF);                            // bits 0-7: port bits 0-7
        word1 |= (ushort)((ConferenceNumber & 0xFF) << 8);               // bits 8-15: conf bits 0-7

        // Words 2 & 3: set to 0
        ushort word2 = 0;
        ushort word3 = 0;

        // Write words as big-endian
        bytes[0] = (byte)(word0 >> 8);
        bytes[1] = (byte)(word0 & 0xFF);
        bytes[2] = (byte)(word1 >> 8);
        bytes[3] = (byte)(word1 & 0xFF);
        bytes[4] = (byte)(word2 >> 8);
        bytes[5] = (byte)(word2 & 0xFF);
        bytes[6] = (byte)(word3 >> 8);
        bytes[7] = (byte)(word3 & 0xFF);

        return bytes;
    }
}
