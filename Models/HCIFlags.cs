namespace HCILibrary.Models;

/// <summary>
/// Represents the flag byte from an HCI message.
/// Bit 0: E (More fragments), Bit 1: M (Status change/reply), Bit 2: U (User/Alias specific), 
/// Bit 3: G (Global), Bit 4: S (Start of sequence), Bit 5: N (Always 1).
/// 
/// Flag meanings:
/// - E (Bit 0): Set to 1 if another packet is expected to follow. Last packet has this set to 0.
/// - M (Bit 1): Set to 1 in status change messages. Set to 0 in status reply messages.
/// - S (Bit 4): Set to 1 in the first packet sent in a sequence.
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
    /// Flag E (Bit 0). Set to 1 if another packet is expected to follow (more fragments coming).
    /// The last packet in a sequence has this set to 0.
    /// </summary>
    public bool E => (_flagByte & 0x01) != 0;

    /// <summary>
    /// Flag M (Bit 1). Set to 1 in a status change message. Set to 0 in a status reply message.
    /// Note: May be ignored if set to 1 according to the protocol.
    /// </summary>
    public bool M => (_flagByte & 0x02) != 0;

    /// <summary>
    /// Flag U (Bit 2). User flag - used in some contexts for request/response differentiation.
    /// </summary>
    public bool U => (_flagByte & 0x04) != 0;

    /// <summary>
    /// Flag G (Bit 3). Global bit - can be ignored by the host.
    /// </summary>
    public bool G => (_flagByte & 0x08) != 0;

    /// <summary>
    /// Flag S (Bit 4). Start bit - set to 1 in the first packet sent in a sequence.
    /// </summary>
    public bool S => (_flagByte & 0x10) != 0;

    /// <summary>
    /// Flag N (Bit 5). Always set to 1.
    /// </summary>
    public bool N => (_flagByte & 0x20) != 0;

    public override string ToString() => $"E={E}, M={M}, U={U}, G={G}, S={S}, N={N}";


}
