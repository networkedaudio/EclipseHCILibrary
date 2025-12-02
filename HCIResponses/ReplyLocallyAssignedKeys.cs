using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents the key status byte.
/// </summary>
public class LocalKeyStatus
{
    /// <summary>
    /// Raw status byte.
    /// </summary>
    public byte RawStatus { get; set; }

    /// <summary>
    /// Key state (bits 6-7).
    /// </summary>
    public LocalKeyState KeyState { get; set; }

    /// <summary>
    /// Listen mode flag (bit 5).
    /// </summary>
    public bool ListenMode { get; set; }

    /// <summary>
    /// Pot assigned flag (bit 4).
    /// </summary>
    public bool PotAssigned { get; set; }

    /// <summary>
    /// Pot state (bits 0-3).
    /// </summary>
    public byte PotState { get; set; }

    /// <summary>
    /// Pot number.
    /// </summary>
    public ushort PotNumber { get; set; }

    /// <summary>
    /// Parses key status from payload.
    /// </summary>
    public static LocalKeyStatus Parse(byte[] payload, int offset)
    {
        byte status = payload[offset];
        return new LocalKeyStatus
        {
            RawStatus = status,
            KeyState = (LocalKeyState)((status >> 6) & 0x03),
            ListenMode = (status & 0x20) != 0,
            PotAssigned = (status & 0x10) != 0,
            PotState = (byte)(status & 0x0F),
            PotNumber = (ushort)((payload[offset + 1] << 8) | payload[offset + 2])
        };
    }
}

/// <summary>
/// Represents the key operation structure (4 bytes).
/// </summary>
public class KeyOperation
{
    /// <summary>
    /// Raw operation bytes.
    /// </summary>
    public byte[] RawBytes { get; set; } = new byte[4];

    // Byte 0
    /// <summary>
    /// Latch mode (bits 0-3 of byte 0).
    /// </summary>
    public LatchMode LatchMode { get; set; }

    /// <summary>
    /// Dial mode (bit 5 of byte 0).
    /// </summary>
    public bool Dial { get; set; }

    /// <summary>
    /// Dual/double width (bit 6 of byte 0).
    /// </summary>
    public bool Dual { get; set; }

    /// <summary>
    /// Text mode displayed (bit 7 of byte 0).
    /// </summary>
    public bool TextMode { get; set; }

    // Byte 1
    /// <summary>
    /// Unpaged flag (bit 0 of byte 1).
    /// </summary>
    public bool Unpaged { get; set; }

    /// <summary>
    /// Interlock group number (bits 4-7 of byte 0).
    /// </summary>
    public byte Group { get; set; }

    /// <summary>
    /// Deactivating - all interlock group keys may be off (bit 3 of byte 1).
    /// </summary>
    public bool Deactivating { get; set; }

    /// <summary>
    /// Make/Break - interlock group make before break (bit 2 of byte 1).
    /// </summary>
    public bool MakeBreak { get; set; }

    /// <summary>
    /// Cross page - keys interlocked across pages in same region (bit 1 of byte 1).
    /// </summary>
    public bool CrossPage { get; set; }

    /// <summary>
    /// CMAPSi flag special 1 (bit 0 of byte 1).
    /// </summary>
    public bool CmapsiSp1 { get; set; }

    // Byte 2
    /// <summary>
    /// CMAPSi flag special 2 (bit 7 of byte 2).
    /// </summary>
    public bool CmapsiSp2 { get; set; }

    /// <summary>
    /// Key region number (bits 4-6 of byte 2).
    /// </summary>
    public byte RegionValue { get; set; }

    /// <summary>
    /// Stacked group flag (bit 3 of byte 2).
    /// </summary>
    public bool StackedGroup { get; set; }

    // Byte 3
    /// <summary>
    /// Page value - the page this key switches to when pressed.
    /// </summary>
    public byte PageValue { get; set; }

    /// <summary>
    /// Parses key operation from payload.
    /// </summary>
    public static KeyOperation Parse(byte[] payload, int offset)
    {
        var op = new KeyOperation();
        Array.Copy(payload, offset, op.RawBytes, 0, 4);

        byte byte0 = payload[offset];
        byte byte1 = payload[offset + 1];
        byte byte2 = payload[offset + 2];
        byte byte3 = payload[offset + 3];

        // Byte 0
        op.LatchMode = (LatchMode)(byte0 & 0x0F);
        op.Group = (byte)((byte0 >> 4) & 0x0F);
        op.Dial = (byte0 & 0x20) != 0;
        op.Dual = (byte0 & 0x40) != 0;
        op.TextMode = (byte0 & 0x80) != 0;

        // Byte 1
        op.Unpaged = (byte1 & 0x01) != 0;
        op.CrossPage = (byte1 & 0x02) != 0;
        op.MakeBreak = (byte1 & 0x04) != 0;
        op.Deactivating = (byte1 & 0x08) != 0;
        op.CmapsiSp1 = (byte1 & 0x01) != 0;

        // Byte 2
        op.StackedGroup = (byte2 & 0x08) != 0;
        op.RegionValue = (byte)((byte2 >> 4) & 0x07);
        op.CmapsiSp2 = (byte2 & 0x80) != 0;

        // Byte 3
        op.PageValue = byte3;

        return op;
    }
}

/// <summary>
/// Represents the key action flags.
/// </summary>
public class KeyAction
{
    /// <summary>
    /// Raw action value.
    /// </summary>
    public ushort RawValue { get; set; }

    /// <summary>
    /// Force listen enabled (bit 15).
    /// </summary>
    public bool ForceListen { get; set; }

    /// <summary>
    /// Talk enabled (bit 14).
    /// </summary>
    public bool Talk { get; set; }

    /// <summary>
    /// Listen enabled (bit 13).
    /// </summary>
    public bool Listen { get; set; }

    /// <summary>
    /// Hold to talk enabled (bit 12).
    /// </summary>
    public bool HoldToTalk { get; set; }

    /// <summary>
    /// Initial state selected (bit 11).
    /// </summary>
    public bool InitialState { get; set; }

    /// <summary>
    /// Assign locally enabled (bit 10).
    /// </summary>
    public bool AssignLocally { get; set; }

    /// <summary>
    /// Assign remotely enabled (bit 9).
    /// </summary>
    public bool AssignRemotely { get; set; }

    /// <summary>
    /// Locally assigned flag (bit 8).
    /// </summary>
    public bool LocallyAssigned { get; set; }

    /// <summary>
    /// Parses key action from a 16-bit word.
    /// </summary>
    public static KeyAction Parse(ushort value)
    {
        return new KeyAction
        {
            RawValue = value,
            ForceListen = (value & 0x8000) != 0,
            Talk = (value & 0x4000) != 0,
            Listen = (value & 0x2000) != 0,
            HoldToTalk = (value & 0x1000) != 0,
            InitialState = (value & 0x0800) != 0,
            AssignLocally = (value & 0x0400) != 0,
            AssignRemotely = (value & 0x0200) != 0,
            LocallyAssigned = (value & 0x0100) != 0
        };
    }
}

/// <summary>
/// Represents the key configuration structure (26 bytes).
/// </summary>
public class KeyConfig
{
    /// <summary>
    /// System number.
    /// </summary>
    public byte SystemNumber { get; set; }

    /// <summary>
    /// Spare/unused byte.
    /// </summary>
    public byte Unused { get; set; }

    /// <summary>
    /// Specific use depends on entity.
    /// </summary>
    public ushort Specific { get; set; }

    /// <summary>
    /// Secondary DCC/DialCode (speed dial).
    /// </summary>
    public ushort Secondary { get; set; }

    /// <summary>
    /// Key action flags.
    /// </summary>
    public KeyAction? KeyAction { get; set; }

    /// <summary>
    /// Unique ECS GUID number (16 bytes).
    /// </summary>
    public byte[] Guid { get; set; } = new byte[16];

    /// <summary>
    /// GUID as a formatted string.
    /// </summary>
    public string GuidString => BitConverter.ToString(Guid).Replace("-", "");

    /// <summary>
    /// Parses key config from payload.
    /// </summary>
    public static KeyConfig Parse(byte[] payload, int offset)
    {
        var config = new KeyConfig
        {
            SystemNumber = payload[offset],
            Unused = payload[offset + 1],
            Specific = (ushort)((payload[offset + 2] << 8) | payload[offset + 3]),
            Secondary = (ushort)((payload[offset + 4] << 8) | payload[offset + 5]),
            KeyAction = KeyAction.Parse((ushort)((payload[offset + 6] << 8) | payload[offset + 7]))
        };

        // GUID: 16 bytes starting at offset + 8
        Array.Copy(payload, offset + 8, config.Guid, 0, 16);

        return config;
    }
}

/// <summary>
/// Represents a single locally assigned key entry.
/// </summary>
public class LocallyAssignedKey
{
    /// <summary>
    /// Region this key is on.
    /// </summary>
    public byte Region { get; set; }

    /// <summary>
    /// Key ID.
    /// </summary>
    public byte Key { get; set; }

    /// <summary>
    /// Page this key is on.
    /// </summary>
    public byte Page { get; set; }

    /// <summary>
    /// Entity type (e.g., EN_PORT, EN_GROUP).
    /// </summary>
    public ushort Entity { get; set; }

    /// <summary>
    /// Key status information.
    /// </summary>
    public LocalKeyStatus? KeyStatus { get; set; }

    /// <summary>
    /// Key operation settings.
    /// </summary>
    public KeyOperation? KeyOperation { get; set; }

    /// <summary>
    /// Key configuration.
    /// </summary>
    public KeyConfig? KeyConfig { get; set; }
}

/// <summary>
/// Reply Locally Assigned Keys (0x00BA).
/// Returns the configuration of all locally assigned keys for a selected panel.
/// </summary>
public class ReplyLocallyAssignedKeys
{
    /// <summary>
    /// Card slot number.
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    /// Port offset from first port of the card.
    /// </summary>
    public byte Port { get; set; }

    /// <summary>
    /// Number of locally assigned keys on this panel.
    /// </summary>
    public byte Count { get; set; }

    /// <summary>
    /// List of locally assigned key entries.
    /// </summary>
    public List<LocallyAssignedKey> Keys { get; set; } = new();

    /// <summary>
    /// Parses the payload of a Reply Locally Assigned Keys message.
    /// </summary>
    /// <param name="payload">The message payload (after protocol schema byte).</param>
    /// <returns>The parsed ReplyLocallyAssignedKeys, or null if parsing fails.</returns>
    public static ReplyLocallyAssignedKeys? Parse(byte[] payload)
    {
        // Minimum payload: Slot(1) + Port(1) + Count(1) = 3 bytes
        if (payload == null || payload.Length < 3)
        {
            return null;
        }

        var result = new ReplyLocallyAssignedKeys
        {
            Slot = payload[0],
            Port = payload[1],
            Count = payload[2]
        };

        // Each key entry:
        // Region(1) + Key(1) + Page(1) + Entity(2) + KeyStatus(1) + PotNumber(2) + 
        // KeyOperation(4) + KeyConfig(26) = 38 bytes
        const int keyEntrySize = 38;
        int offset = 3;

        for (int i = 0; i < result.Count; i++)
        {
            if (payload.Length < offset + keyEntrySize)
            {
                break;
            }

            var key = new LocallyAssignedKey
            {
                Region = payload[offset],
                Key = payload[offset + 1],
                Page = payload[offset + 2],
                Entity = (ushort)((payload[offset + 3] << 8) | payload[offset + 4]),
                KeyStatus = LocalKeyStatus.Parse(payload, offset + 5),
                KeyOperation = KeyOperation.Parse(payload, offset + 8),
                KeyConfig = KeyConfig.Parse(payload, offset + 12)
            };

            result.Keys.Add(key);
            offset += keyEntrySize;
        }

        return result;
    }
}
