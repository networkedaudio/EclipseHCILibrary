namespace HCILibrary.Models;

/// <summary>
/// Represents the flag byte from an HCI message.
/// Bit 0: E, Bit 1: M, Bit 2: U, Bit 3: G, Bit 4: S, Bit 5: N.
/// </summary>
public class HCIFlags
{
    private readonly byte _flagByte;

    /// <summary>
    /// Creates a new HCIFlags instance from a flag byte.
    /// </summary>
    /// <param name="flagByte">The raw flag byte from the message.</param>
    public HCIFlags(byte flagByte)
    {
        _flagByte = flagByte;
    }

    /// <summary>
    /// Gets the raw flag byte value.
    /// </summary>
    public byte RawValue => _flagByte;

    /// <summary>
    /// Flag E (Bit 0).
    /// </summary>
    public bool E => (_flagByte & 0x01) != 0;

    /// <summary>
    /// Flag M (Bit 1).
    /// </summary>
    public bool M => (_flagByte & 0x02) != 0;

    /// <summary>
    /// Flag U (Bit 2).
    /// </summary>
    public bool U => (_flagByte & 0x04) != 0;

    /// <summary>
    /// Flag G (Bit 3).
    /// </summary>
    public bool G => (_flagByte & 0x08) != 0;

    /// <summary>
    /// Flag S (Bit 4).
    /// </summary>
    public bool S => (_flagByte & 0x10) != 0;

    /// <summary>
    /// Flag N (Bit 5).
    /// </summary>
    public bool N => (_flagByte & 0x20) != 0;

    public override string ToString()
    {
        return $"E={E}, M={M}, U={U}, G={G}, S={S}, N={N}";
    }
}
