using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents the status of a single GPIO/SFO pin.
/// </summary>
public class GpioSfoPinStatus
{
    /// <summary>
    /// Pin number (0-31).
    /// </summary>
    public byte PinNumber { get; set; }

    /// <summary>
    /// State of the pin: true = on, false = off.
    /// </summary>
    public bool IsOn { get; set; }

    /// <summary>
    /// Type of pin: Input or Output.
    /// </summary>
    public GpioPinType PinType { get; set; }

    /// <summary>
    /// Parses pin data from a 16-bit word.
    /// </summary>
    /// <param name="data">The 16-bit pin data.</param>
    /// <returns>The parsed pin status.</returns>
    public static GpioSfoPinStatus FromWord(ushort data)
    {
        return new GpioSfoPinStatus
        {
            PinNumber = (byte)(data & 0x1F),              // bits 0-4 (0-31)
            IsOn = (data & 0x2000) != 0,                  // bit 13
            PinType = (data & 0x4000) != 0 ? GpioPinType.Input : GpioPinType.Output  // bit 14
        };
    }
}

/// <summary>
/// Reply GPIO/SFO Status (0x0018).
/// Contains the current state of the input and output pins on a GPIO/SFO card.
/// Generated when a pin changes state or in response to Request GPIO/SFO Status.
/// </summary>
public class ReplyGpioSfoStatus
{
    /// <summary>
    /// Protocol schema version from the response.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The card number (0-128).
    /// </summary>
    public ushort CardNumber { get; set; }

    /// <summary>
    /// The list of pin statuses.
    /// </summary>
    public List<GpioSfoPinStatus> Pins { get; } = new();

    /// <summary>
    /// Parses a Reply GPIO/SFO Status response from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags, starting at protocol tag).</param>
    /// <returns>The parsed response.</returns>
    public static ReplyGpioSfoStatus Parse(byte[] payload)
    {
        var result = new ReplyGpioSfoStatus();
        int offset = 0;

        // Skip protocol tag (4 bytes) - already validated by caller
        offset += 4;

        // Protocol schema (1 byte)
        result.ProtocolSchema = payload[offset++];

        // Pin count (16-bit, big-endian)
        ushort pinCount = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Card data (16-bit, big-endian)
        // bits 0-12: card number
        // bits 13-14: 0
        // bit 15: 1
        ushort cardData = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        result.CardNumber = (ushort)(cardData & 0x1FFF);
        offset += 2;

        // Pin data (2 bytes each)
        for (int i = 0; i < pinCount; i++)
        {
            if (offset + 2 <= payload.Length)
            {
                ushort pinData = (ushort)((payload[offset] << 8) | payload[offset + 1]);
                result.Pins.Add(GpioSfoPinStatus.FromWord(pinData));
                offset += 2;
            }
        }

        return result;
    }
}
