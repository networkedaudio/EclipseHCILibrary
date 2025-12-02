using HCILibrary.Enums;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Represents a single audio monitor action for Request/Reply Audio Monitor Actions message.
/// A port monitor mirrors crosspoints to a target port to another monitor port.
/// </summary>
public class AudioMonitorAction
{
    /// <summary>
    /// Direction of the action: false = delete (remove monitor), true = add (create monitor).
    /// </summary>
    public bool IsAdd { get; set; }

    /// <summary>
    /// Monitor port number - the port to which crosspoints to the target port are mirrored (0-2047, 11 bits).
    /// </summary>
    public ushort MonitorPort { get; set; }

    /// <summary>
    /// Target port number - the port being monitored (0-2047, 11 bits).
    /// </summary>
    public ushort TargetPort { get; set; }

    /// <summary>
    /// Creates a new audio monitor action.
    /// </summary>
    public AudioMonitorAction()
    {
    }

    /// <summary>
    /// Creates a new audio monitor action with specified parameters.
    /// </summary>
    /// <param name="isAdd">true to add monitor, false to delete monitor.</param>
    /// <param name="monitorPort">The monitor port number (0-2047).</param>
    /// <param name="targetPort">The target port number being monitored (0-2047).</param>
    public AudioMonitorAction(bool isAdd, ushort monitorPort, ushort targetPort)
    {
        IsAdd = isAdd;
        MonitorPort = monitorPort;
        TargetPort = targetPort;
    }

    /// <summary>
    /// Generates the 4-word (8 byte) action data for this audio monitor action.
    /// </summary>
    /// <returns>The action data bytes (8 bytes).</returns>
    public byte[] ToBytes()
    {
        var bytes = new byte[8];

        // Word 0:
        // bit 0: direction bit, 0 = delete, 1 = add
        // bits 1-15: set to 0
        ushort word0 = IsAdd ? (ushort)0x0001 : (ushort)0x0000;
        bytes[0] = (byte)(word0 >> 8);
        bytes[1] = (byte)(word0 & 0xFF);

        // Word 1:
        // bits 0-10: Monitor port number (zero based)
        // bits 11-15: set to 0
        ushort word1 = (ushort)(MonitorPort & 0x07FF); // Mask to 11 bits
        bytes[2] = (byte)(word1 >> 8);
        bytes[3] = (byte)(word1 & 0xFF);

        // Word 2:
        // bits 0-10: Target port number (zero based)
        // bits 11-15: set to 0
        ushort word2 = (ushort)(TargetPort & 0x07FF); // Mask to 11 bits
        bytes[4] = (byte)(word2 >> 8);
        bytes[5] = (byte)(word2 & 0xFF);

        // Word 3:
        // bits 0-15: set to 0
        bytes[6] = 0x00;
        bytes[7] = 0x00;

        return bytes;
    }

    /// <summary>
    /// Parses audio monitor action data from bytes.
    /// </summary>
    /// <param name="data">The source byte array.</param>
    /// <param name="offset">The offset to start reading from.</param>
    /// <returns>A new AudioMonitorAction instance.</returns>
    public static AudioMonitorAction FromBytes(byte[] data, int offset)
    {
        // Word 0: direction bit in bit 0
        ushort word0 = (ushort)((data[offset] << 8) | data[offset + 1]);
        bool isAdd = (word0 & 0x0001) != 0;

        // Word 1: Monitor port number in bits 0-10
        ushort word1 = (ushort)((data[offset + 2] << 8) | data[offset + 3]);
        ushort monitorPort = (ushort)(word1 & 0x07FF);

        // Word 2: Target port number in bits 0-10
        ushort word2 = (ushort)((data[offset + 4] << 8) | data[offset + 5]);
        ushort targetPort = (ushort)(word2 & 0x07FF);

        // Word 3: reserved (ignored)

        return new AudioMonitorAction(isAdd, monitorPort, targetPort);
    }
}
