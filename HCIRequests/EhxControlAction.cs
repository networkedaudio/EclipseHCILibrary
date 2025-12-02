using HCILibrary.Enums;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Represents an EHX Control action for Request EHX Control Actions.
/// Used to change the state of general-purpose outputs (GPOs) and EHX controls.
/// </summary>
public class EhxControlAction
{
    /// <summary>
    /// Action type for EHX Control (always 0x0004).
    /// </summary>
    public const ushort ActionTypeValue = 0x0004;

    /// <summary>
    /// Direction: false = delete, true = add.
    /// </summary>
    public bool Add { get; set; } = true;

    /// <summary>
    /// Pin number (0-31).
    /// </summary>
    public byte PinNumber { get; set; }

    /// <summary>
    /// Card number (0-31).
    /// </summary>
    public byte CardNumber { get; set; }

    /// <summary>
    /// Map type: Enable or Inhibit.
    /// </summary>
    public EhxMapType MapType { get; set; } = EhxMapType.Enable;

    /// <summary>
    /// Creates a new EHX Control action.
    /// </summary>
    public EhxControlAction()
    {
    }

    /// <summary>
    /// Creates a new EHX Control action with the specified parameters.
    /// </summary>
    /// <param name="cardNumber">Card number (0-31).</param>
    /// <param name="pinNumber">Pin number (0-31).</param>
    /// <param name="add">True to add, false to delete.</param>
    /// <param name="mapType">Map type (Enable or Inhibit).</param>
    public EhxControlAction(byte cardNumber, byte pinNumber, bool add = true, EhxMapType mapType = EhxMapType.Enable)
    {
        if (pinNumber > 31)
            throw new ArgumentOutOfRangeException(nameof(pinNumber), "Pin number must be 0-31.");
        if (cardNumber > 31)
            throw new ArgumentOutOfRangeException(nameof(cardNumber), "Card number must be 0-31.");

        CardNumber = cardNumber;
        PinNumber = pinNumber;
        Add = add;
        MapType = mapType;
    }

    /// <summary>
    /// Converts the action to bytes for the message payload.
    /// Returns 10 bytes: Action Type (2) + Action Words (4 x 2 = 8).
    /// </summary>
    /// <returns>The action as a byte array.</returns>
    public byte[] ToBytes()
    {
        var bytes = new byte[10];
        int offset = 0;

        // Action Type: 16 bit word (0x0004), big-endian
        bytes[offset++] = (byte)((ActionTypeValue >> 8) & 0xFF);
        bytes[offset++] = (byte)(ActionTypeValue & 0xFF);

        // Word 0: Direction and fixed bits
        // bit 0: direction (0 = delete, 1 = add)
        // bits 1-3: 0
        // bit 4: 1
        // bits 5-9: 0
        // bit 10: 1
        // bits 11-12: 0
        // bit 13: 1
        // bits 14-15: 0
        // Pattern: 0b0010010000010000 with bit 0 set based on Add
        //          = 0x2410 (bits 4, 10, 13 set)
        ushort word0 = 0x2410;
        if (Add)
            word0 |= 0x0001; // Set bit 0

        bytes[offset++] = (byte)((word0 >> 8) & 0xFF);
        bytes[offset++] = (byte)(word0 & 0xFF);

        // Word 1: Pin and Card numbers
        // bits 0-4: pin number (0-31)
        // bits 5-7: 0
        // bits 8-12: card number (0-31)
        // bits 13-15: 0
        ushort word1 = (ushort)((PinNumber & 0x1F) | ((CardNumber & 0x1F) << 8));
        bytes[offset++] = (byte)((word1 >> 8) & 0xFF);
        bytes[offset++] = (byte)(word1 & 0xFF);

        // Word 2: All zeros
        bytes[offset++] = 0x00;
        bytes[offset++] = 0x00;

        // Word 3: Fixed pattern with map type
        // bit 0: 0
        // bit 1: 1
        // bit 2: 0
        // bits 3-9: 1 (0x7F << 3 = 0x03F8)
        // bit 10: 0
        // bit 11: map type (0 = enable, 1 = inhibit)
        // bit 12: 0
        // bits 13-14: 1
        // bit 15: 0
        // Base pattern: 0b0110000001111010 = 0x607A (bits 1, 3-9, 13-14 set)
        ushort word3 = 0x63FA; // bits 1, 3-9, 13, 14 set
        if (MapType == EhxMapType.Inhibit)
            word3 |= 0x0800; // Set bit 11

        bytes[offset++] = (byte)((word3 >> 8) & 0xFF);
        bytes[offset++] = (byte)(word3 & 0xFF);

        return bytes;
    }

    /// <summary>
    /// Creates an EHX Control action to enable a GPO.
    /// </summary>
    /// <param name="cardNumber">Card number (0-31).</param>
    /// <param name="pinNumber">Pin number (0-31).</param>
    /// <returns>The EHX Control action.</returns>
    public static EhxControlAction Enable(byte cardNumber, byte pinNumber)
        => new(cardNumber, pinNumber, add: true, mapType: EhxMapType.Enable);

    /// <summary>
    /// Creates an EHX Control action to inhibit a GPO.
    /// </summary>
    /// <param name="cardNumber">Card number (0-31).</param>
    /// <param name="pinNumber">Pin number (0-31).</param>
    /// <returns>The EHX Control action.</returns>
    public static EhxControlAction Inhibit(byte cardNumber, byte pinNumber)
        => new(cardNumber, pinNumber, add: true, mapType: EhxMapType.Inhibit);

    /// <summary>
    /// Creates an EHX Control action to delete/remove a control.
    /// </summary>
    /// <param name="cardNumber">Card number (0-31).</param>
    /// <param name="pinNumber">Pin number (0-31).</param>
    /// <returns>The EHX Control action.</returns>
    public static EhxControlAction Delete(byte cardNumber, byte pinNumber)
        => new(cardNumber, pinNumber, add: false, mapType: EhxMapType.Enable);
}
