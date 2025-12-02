namespace HCILibrary.Models;

/// <summary>
/// Represents an input level action (port and level value pair).
/// </summary>
public class LevelAction
{
    /// <summary>
    /// Port number (0-1023).
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Level value (0-255).
    /// </summary>
    public byte LevelValue { get; set; }

    /// <summary>
    /// Creates a new input level action.
    /// </summary>
    public LevelAction()
    {
    }

    /// <summary>
    /// Creates a new input level action with the specified values.
    /// </summary>
    /// <param name="portNumber">Port number (0-1023).</param>
    /// <param name="levelValue">Level value (0-255).</param>
    public LevelAction(ushort portNumber, byte levelValue)
    {
        if (portNumber > 1023)
            throw new ArgumentOutOfRangeException(nameof(portNumber), "Port number must be 0-1023.");

        PortNumber = portNumber;
        LevelValue = levelValue;
    }

    /// <summary>
    /// Converts the action to bytes for the message payload.
    /// Returns 4 bytes: Port Number (2) + Level Value (2).
    /// </summary>
    /// <returns>The action as a byte array.</returns>
    public byte[] ToBytes()
    {
        var bytes = new byte[4];

        // Port number (16-bit, big-endian)
        bytes[0] = (byte)((PortNumber >> 8) & 0xFF);
        bytes[1] = (byte)(PortNumber & 0xFF);

        // Level value (16-bit, big-endian, but only 0-255 used)
        bytes[2] = 0x00;
        bytes[3] = LevelValue;

        return bytes;
    }
}
