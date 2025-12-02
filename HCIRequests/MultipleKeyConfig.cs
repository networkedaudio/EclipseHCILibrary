using HCILibrary.Enums;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Key operation settings (4 bytes).
/// </summary>
public class KeyOperationSettings
{
    /// <summary>
    /// Whether the key is unpaged (true) or paged (false).
    /// </summary>
    public bool Unpaged { get; set; }

    /// <summary>
    /// Whether the key text is displayed.
    /// </summary>
    public bool TextMode { get; set; }

    /// <summary>
    /// Whether the key is double width.
    /// </summary>
    public bool Dual { get; set; }

    /// <summary>
    /// Whether dial mode is enabled.
    /// </summary>
    public bool DialMode { get; set; }

    /// <summary>
    /// The latch mode for the key.
    /// </summary>
    public LatchMode LatchMode { get; set; }

    /// <summary>
    /// Group number for interlock (0 = no group, 1-15 = group number).
    /// Keys on the same panel with the same group number will be deselected if this key is selected.
    /// </summary>
    public byte Group { get; set; }

    /// <summary>
    /// Whether all interlock group keys may be off.
    /// </summary>
    public bool Deactivating { get; set; }

    /// <summary>
    /// Whether interlock group uses make before break.
    /// </summary>
    public bool MakeBreak { get; set; }

    /// <summary>
    /// Whether keys are interlocked across pages in same region.
    /// </summary>
    public bool CrossPage { get; set; }

    /// <summary>
    /// Special 1: For IFB listen keys only. False = Return listen, True = Destination listen.
    /// </summary>
    public bool Special1 { get; set; }

    /// <summary>
    /// Special 2: Reserved, set to false.
    /// </summary>
    public bool Special2 { get; set; }

    /// <summary>
    /// If the key is a page key, this is the region it switches page on when pressed.
    /// </summary>
    public byte RegionValue { get; set; }

    /// <summary>
    /// If the key is a page key, this is the page it switches to when pressed.
    /// </summary>
    public byte PageValue { get; set; }

    /// <summary>
    /// Whether the key is a stacked group.
    /// </summary>
    public bool StackedGroup { get; set; }

    /// <summary>
    /// Converts the key operation settings to a 4-byte array.
    /// </summary>
    public byte[] ToBytes()
    {
        uint value = 0;

        // Bit 0: Unpaged
        if (Unpaged) value |= 0x00000001;

        // Bit 1: TextMode
        if (TextMode) value |= 0x00000002;

        // Bit 2: Dual
        if (Dual) value |= 0x00000004;

        // Bit 3: Dial
        if (DialMode) value |= 0x00000008;

        // Bits 4-7: LatchMode
        value |= (uint)((byte)LatchMode & 0x0F) << 4;

        // Bits 8-11: Group
        value |= (uint)(Group & 0x0F) << 8;

        // Bit 12: Deactivating
        if (Deactivating) value |= 0x00001000;

        // Bit 13: Make/Break
        if (MakeBreak) value |= 0x00002000;

        // Bit 14: Cross Page
        if (CrossPage) value |= 0x00004000;

        // Bit 15: Special 1
        if (Special1) value |= 0x00008000;

        // Bit 16: Special 2
        if (Special2) value |= 0x00010000;

        // Bits 17-19: Region Value
        value |= (uint)(RegionValue & 0x07) << 17;

        // Bits 20-23: Page Value
        value |= (uint)(PageValue & 0x0F) << 20;

        // Bit 24: Stacked Group
        if (StackedGroup) value |= 0x01000000;

        // Bits 25-31: Unused

        return new byte[]
        {
            (byte)(value & 0xFF),
            (byte)((value >> 8) & 0xFF),
            (byte)((value >> 16) & 0xFF),
            (byte)((value >> 24) & 0xFF)
        };
    }

    /// <summary>
    /// Parses a 4-byte array into KeyOperationSettings.
    /// </summary>
    public static KeyOperationSettings Parse(byte[] data, int offset = 0)
    {
        uint value = (uint)(data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16) | (data[offset + 3] << 24));

        return new KeyOperationSettings
        {
            Unpaged = (value & 0x00000001) != 0,
            TextMode = (value & 0x00000002) != 0,
            Dual = (value & 0x00000004) != 0,
            DialMode = (value & 0x00000008) != 0,
            LatchMode = (LatchMode)((value >> 4) & 0x0F),
            Group = (byte)((value >> 8) & 0x0F),
            Deactivating = (value & 0x00001000) != 0,
            MakeBreak = (value & 0x00002000) != 0,
            CrossPage = (value & 0x00004000) != 0,
            Special1 = (value & 0x00008000) != 0,
            Special2 = (value & 0x00010000) != 0,
            RegionValue = (byte)((value >> 17) & 0x07),
            PageValue = (byte)((value >> 20) & 0x0F),
            StackedGroup = (value & 0x01000000) != 0
        };
    }
}

/// <summary>
/// Key action settings (2 bytes).
/// </summary>
public class KeyActionSettings
{
    /// <summary>
    /// Whether force listen is enabled.
    /// </summary>
    public bool ForceListen { get; set; }

    /// <summary>
    /// Whether talk is enabled.
    /// </summary>
    public bool Talk { get; set; }

    /// <summary>
    /// Whether listen is enabled.
    /// </summary>
    public bool Listen { get; set; }

    /// <summary>
    /// Whether hold to talk is enabled.
    /// </summary>
    public bool HoldToTalk { get; set; }

    /// <summary>
    /// Whether the key is initially selected.
    /// </summary>
    public bool InitialState { get; set; }

    /// <summary>
    /// Whether assign locally is enabled.
    /// </summary>
    public bool AssignLocally { get; set; }

    /// <summary>
    /// Whether assign remotely is enabled.
    /// </summary>
    public bool AssignRemotely { get; set; }

    /// <summary>
    /// Whether the key is locally assigned.
    /// </summary>
    public bool LocallyAssigned { get; set; }

    /// <summary>
    /// Radio mode setting.
    /// </summary>
    public RadioMode Radio { get; set; }

    /// <summary>
    /// Converts the key action settings to a 2-byte array (big-endian).
    /// </summary>
    public byte[] ToBytes()
    {
        ushort value = 0;

        // Bit 0: Force Listen
        if (ForceListen) value |= 0x0001;

        // Bit 1: Talk
        if (Talk) value |= 0x0002;

        // Bit 2: Listen
        if (Listen) value |= 0x0004;

        // Bit 3: Hold to Talk
        if (HoldToTalk) value |= 0x0008;

        // Bit 4: Initial State
        if (InitialState) value |= 0x0010;

        // Bit 5: Assign Locally
        if (AssignLocally) value |= 0x0020;

        // Bit 6: Assign Remotely
        if (AssignRemotely) value |= 0x0040;

        // Bit 7: Locally Assigned
        if (LocallyAssigned) value |= 0x0080;

        // Bits 8-10: Radio
        value |= (ushort)(((byte)Radio & 0x07) << 8);

        // Bits 11-15: Reserved

        return new byte[]
        {
            (byte)((value >> 8) & 0xFF),
            (byte)(value & 0xFF)
        };
    }

    /// <summary>
    /// Parses a 2-byte array (big-endian) into KeyActionSettings.
    /// </summary>
    public static KeyActionSettings Parse(byte[] data, int offset = 0)
    {
        ushort value = (ushort)((data[offset] << 8) | data[offset + 1]);

        return new KeyActionSettings
        {
            ForceListen = (value & 0x0001) != 0,
            Talk = (value & 0x0002) != 0,
            Listen = (value & 0x0004) != 0,
            HoldToTalk = (value & 0x0008) != 0,
            InitialState = (value & 0x0010) != 0,
            AssignLocally = (value & 0x0020) != 0,
            AssignRemotely = (value & 0x0040) != 0,
            LocallyAssigned = (value & 0x0080) != 0,
            Radio = (RadioMode)((value >> 8) & 0x07)
        };
    }
}

/// <summary>
/// Key configuration (26 bytes).
/// </summary>
public class KeyConfigSettings
{
    /// <summary>
    /// System number.
    /// </summary>
    public byte SystemNumber { get; set; }

    /// <summary>
    /// Spare byte (unused).
    /// </summary>
    public byte Unused { get; set; }

    /// <summary>
    /// Specific use depends on entity.
    /// </summary>
    public ushort Specific { get; set; }

    /// <summary>
    /// Secondary DCC/DialCode (speed dial).
    /// </summary>
    public ushort SecondaryDcc { get; set; }

    /// <summary>
    /// Key action settings.
    /// </summary>
    public KeyActionSettings KeyAction { get; set; } = new();

    /// <summary>
    /// Unique ECS GUID number (16 bytes).
    /// </summary>
    public byte[] Guid { get; set; } = new byte[16];

    /// <summary>
    /// Converts the key config to a 26-byte array.
    /// </summary>
    public byte[] ToBytes()
    {
        var bytes = new byte[26];
        int offset = 0;

        // System Number: 1 byte
        bytes[offset++] = SystemNumber;

        // Unused: 1 byte
        bytes[offset++] = Unused;

        // Specific: 2 bytes (big-endian)
        bytes[offset++] = (byte)((Specific >> 8) & 0xFF);
        bytes[offset++] = (byte)(Specific & 0xFF);

        // Secondary DCC: 2 bytes (big-endian)
        bytes[offset++] = (byte)((SecondaryDcc >> 8) & 0xFF);
        bytes[offset++] = (byte)(SecondaryDcc & 0xFF);

        // Key Action: 2 bytes
        var keyActionBytes = KeyAction.ToBytes();
        bytes[offset++] = keyActionBytes[0];
        bytes[offset++] = keyActionBytes[1];

        // GUID: 16 bytes
        Array.Copy(Guid, 0, bytes, offset, Math.Min(Guid.Length, 16));

        return bytes;
    }

    /// <summary>
    /// Parses a 26-byte array into KeyConfigSettings.
    /// </summary>
    public static KeyConfigSettings Parse(byte[] data, int offset = 0)
    {
        var config = new KeyConfigSettings
        {
            SystemNumber = data[offset],
            Unused = data[offset + 1],
            Specific = (ushort)((data[offset + 2] << 8) | data[offset + 3]),
            SecondaryDcc = (ushort)((data[offset + 4] << 8) | data[offset + 5]),
            KeyAction = KeyActionSettings.Parse(data, offset + 6)
        };

        Array.Copy(data, offset + 8, config.Guid, 0, 16);

        return config;
    }
}

/// <summary>
/// Represents a single key entry for the Set Config Multiple Keys request.
/// </summary>
public class MultipleKeyConfigEntry
{
    /// <summary>
    /// Region this key is on.
    /// </summary>
    public byte Region { get; set; }

    /// <summary>
    /// This key's ID.
    /// </summary>
    public ushort KeyId { get; set; }

    /// <summary>
    /// Page this key is on.
    /// </summary>
    public byte Page { get; set; }

    /// <summary>
    /// Entity type (e.g., EN_PORT, EN_GROUP).
    /// The system number of the entity on the key must be set to a non-zero value.
    /// </summary>
    public ushort Entity { get; set; }

    /// <summary>
    /// Key operation settings.
    /// </summary>
    public KeyOperationSettings KeyOperation { get; set; } = new();

    /// <summary>
    /// Key configuration settings.
    /// </summary>
    public KeyConfigSettings KeyConfig { get; set; } = new();

    /// <summary>
    /// Converts the entry to bytes.
    /// Entry size: Region(1) + KeyId(2) + Page(1) + Entity(2) + KeyOperation(4) + KeyConfig(26) = 36 bytes
    /// </summary>
    public byte[] ToBytes()
    {
        var bytes = new byte[36];
        int offset = 0;

        // Region: 1 byte
        bytes[offset++] = Region;

        // Key ID: 2 bytes (big-endian)
        bytes[offset++] = (byte)((KeyId >> 8) & 0xFF);
        bytes[offset++] = (byte)(KeyId & 0xFF);

        // Page: 1 byte
        bytes[offset++] = Page;

        // Entity: 2 bytes (big-endian)
        bytes[offset++] = (byte)((Entity >> 8) & 0xFF);
        bytes[offset++] = (byte)(Entity & 0xFF);

        // Key Operation: 4 bytes
        var keyOpBytes = KeyOperation.ToBytes();
        Array.Copy(keyOpBytes, 0, bytes, offset, 4);
        offset += 4;

        // Key Config: 26 bytes
        var keyConfigBytes = KeyConfig.ToBytes();
        Array.Copy(keyConfigBytes, 0, bytes, offset, 26);

        return bytes;
    }
}
