using HCILibrary.Enums;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Represents a single crosspoint action for Request Crosspoint Actions message.
/// </summary>
public class CrosspointAction
{
    /// <summary>
    /// Direction of the action: false = delete, true = add.
    /// </summary>
    public bool IsAdd { get; set; }

    /// <summary>
    /// Source port number (0-1023, 10 bits).
    /// </summary>
    public ushort SourcePort { get; set; }

    /// <summary>
    /// Destination port number (0-1023, 10 bits).
    /// </summary>
    public ushort DestinationPort { get; set; }

    /// <summary>
    /// Enable/disable the crosspoint: false = enable, true = inhibit.
    /// </summary>
    public bool IsInhibit { get; set; }

    /// <summary>
    /// Crosspoint priority. Use Normal (1) for non-local CSU, LocalCSU2 (2) or LocalCSU3 (3) for local CSU.
    /// </summary>
    public CrosspointPriority Priority { get; set; } = CrosspointPriority.Normal;

    /// <summary>
    /// Creates a new crosspoint action.
    /// </summary>
    /// <param name="isAdd">true to add, false to delete.</param>
    /// <param name="sourcePort">The source port number (0-1023).</param>
    /// <param name="destinationPort">The destination port number (0-1023).</param>
    /// <param name="isInhibit">true to inhibit, false to enable.</param>
    /// <param name="priority">The crosspoint priority.</param>
    public CrosspointAction(bool isAdd, ushort sourcePort, ushort destinationPort, 
        bool isInhibit = false, CrosspointPriority priority = CrosspointPriority.Normal)
    {
        IsAdd = isAdd;
        SourcePort = sourcePort;
        DestinationPort = destinationPort;
        IsInhibit = isInhibit;
        Priority = priority;
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
        // bits 1,2: destination port number, bits 8,9
        // bits 3-9: set to 0
        // bit 10: set to 1
        // bits 11,12: source port number, bits 8,9
        // bit 13: set to 1
        // bits 14,15: set to 0
        ushort word0 = 0;
        if (IsAdd) word0 |= 0x0001;                                      // bit 0
        word0 |= (ushort)(((DestinationPort >> 8) & 0x03) << 1);         // bits 1-2: dest bits 8-9
        word0 |= 0x0400;                                                  // bit 10: always 1
        word0 |= (ushort)(((SourcePort >> 8) & 0x03) << 11);             // bits 11-12: src bits 8-9
        word0 |= 0x2000;                                                  // bit 13: always 1

        // Word 1:
        // bits 0-7: destination port number, bits 0-7
        // bits 8-15: source port number, bits 0-7
        ushort word1 = 0;
        word1 |= (ushort)(DestinationPort & 0xFF);                       // bits 0-7: dest bits 0-7
        word1 |= (ushort)((SourcePort & 0xFF) << 8);                     // bits 8-15: src bits 0-7

        // Word 2:
        // bits 0-15: set to 0
        ushort word2 = 0;

        // Word 3:
        // bit 0: set to 0
        // bit 1: set to 1
        // bit 2: set to 0
        // bits 3-9: set to 1 (0x03F8)
        // bit 10: set to 0
        // bit 11: enable/disable, 0 = enable, 1 = inhibit
        // bit 12: set to 0
        // bits 13-15: crosspoint priority
        ushort word3 = 0;
        word3 |= 0x0002;                                                  // bit 1: always 1
        word3 |= 0x03F8;                                                  // bits 3-9: all 1s
        if (IsInhibit) word3 |= 0x0800;                                  // bit 11: inhibit
        word3 |= (ushort)(((byte)Priority & 0x07) << 13);                // bits 13-15: priority

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
