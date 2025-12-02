using HCILibrary.Enums;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Entity reference data for key assignment.
/// </summary>
public class KeyEntityReference
{
    /// <summary>
    /// System number of the entity.
    /// </summary>
    public byte SystemNumber { get; set; }

    /// <summary>
    /// Reserved byte, set to 0.
    /// </summary>
    public byte Reserved { get; set; }

    /// <summary>
    /// Port number (for EN_PORT, 0-1023).
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Conference number (for EN_CONF, 1-199 or 16-bit for HX).
    /// </summary>
    public ushort ConferenceNumber { get; set; }

    /// <summary>
    /// Group number (for EN_GROUP, 0-100).
    /// </summary>
    public ushort GroupNumber { get; set; }

    /// <summary>
    /// IFB number (for EN_IFB, 0-99).
    /// </summary>
    public byte IfbNumber { get; set; }

    /// <summary>
    /// Converts the entity reference to a 4-byte array based on entity type.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <returns>4-byte array representing the entity reference.</returns>
    public byte[] ToBytes(KeyEntityType entityType)
    {
        var bytes = new byte[4];

        bytes[0] = SystemNumber;
        bytes[1] = Reserved;

        switch (entityType)
        {
            case KeyEntityType.Port:
                bytes[2] = (byte)((PortNumber >> 8) & 0xFF);
                bytes[3] = (byte)(PortNumber & 0xFF);
                break;

            case KeyEntityType.Conference:
                bytes[2] = (byte)((ConferenceNumber >> 8) & 0xFF);
                bytes[3] = (byte)(ConferenceNumber & 0xFF);
                break;

            case KeyEntityType.Group:
                bytes[2] = (byte)((GroupNumber >> 8) & 0xFF);
                bytes[3] = (byte)(GroupNumber & 0xFF);
                break;

            case KeyEntityType.Ifb:
                bytes[2] = IfbNumber;
                bytes[3] = 0;
                break;

            case KeyEntityType.Null:
            default:
                // All zeros for null
                break;
        }

        return bytes;
    }

    /// <summary>
    /// Creates an entity reference for a port.
    /// </summary>
    public static KeyEntityReference ForPort(byte systemNumber, ushort portNumber)
    {
        return new KeyEntityReference { SystemNumber = systemNumber, PortNumber = portNumber };
    }

    /// <summary>
    /// Creates an entity reference for a conference.
    /// </summary>
    public static KeyEntityReference ForConference(byte systemNumber, ushort conferenceNumber)
    {
        return new KeyEntityReference { SystemNumber = systemNumber, ConferenceNumber = conferenceNumber };
    }

    /// <summary>
    /// Creates an entity reference for a fixed group.
    /// </summary>
    public static KeyEntityReference ForGroup(byte systemNumber, ushort groupNumber)
    {
        return new KeyEntityReference { SystemNumber = systemNumber, GroupNumber = groupNumber };
    }

    /// <summary>
    /// Creates an entity reference for an IFB.
    /// </summary>
    public static KeyEntityReference ForIfb(byte systemNumber, byte ifbNumber)
    {
        return new KeyEntityReference { SystemNumber = systemNumber, IfbNumber = ifbNumber };
    }
}

/// <summary>
/// Represents a single remote key action entry (Action Type 1).
/// </summary>
public class RemoteKeyAction
{
    /// <summary>
    /// Region of panel where key is located.
    /// </summary>
    public byte Region { get; set; }

    /// <summary>
    /// Page of panel where key is located.
    /// </summary>
    public byte Page { get; set; }

    /// <summary>
    /// Key position number.
    /// </summary>
    public byte Key { get; set; }

    /// <summary>
    /// Reserved, set to 0.
    /// </summary>
    public byte Reserved { get; set; }

    /// <summary>
    /// Entity type for the key assignment.
    /// </summary>
    public KeyEntityType EntityType { get; set; }

    /// <summary>
    /// Entity reference data.
    /// </summary>
    public KeyEntityReference EntityReference { get; set; } = new();

    /// <summary>
    /// Key activation type.
    /// </summary>
    public KeyActivationType KeyActivation { get; set; } = KeyActivationType.TalkAndListen;

    /// <summary>
    /// Latch mode (not currently supported, set to 0).
    /// </summary>
    public KeyLatchMode LatchMode { get; set; } = KeyLatchMode.LatchNonLatch;

    /// <summary>
    /// Converts the remote key action to bytes.
    /// Entry size: Region(1) + Page(1) + Key(1) + Reserved(1) + EntityType(2) + EntityRef(4) + KeyActivation(1) + LatchMode(1) = 12 bytes
    /// </summary>
    public byte[] ToBytes()
    {
        var bytes = new byte[12];
        int offset = 0;

        // Region: 1 byte
        bytes[offset++] = Region;

        // Page: 1 byte
        bytes[offset++] = Page;

        // Key: 1 byte
        bytes[offset++] = Key;

        // Reserved: 1 byte
        bytes[offset++] = Reserved;

        // Entity Type: 2 bytes (big-endian)
        bytes[offset++] = (byte)(((ushort)EntityType >> 8) & 0xFF);
        bytes[offset++] = (byte)((ushort)EntityType & 0xFF);

        // Entity Reference: 4 bytes
        var entityRefBytes = EntityReference.ToBytes(EntityType);
        Array.Copy(entityRefBytes, 0, bytes, offset, 4);
        offset += 4;

        // Key Activation: 1 byte
        bytes[offset++] = (byte)KeyActivation;

        // Latch Mode: 1 byte
        bytes[offset++] = (byte)LatchMode;

        return bytes;
    }
}
