using HCILibrary.Enums;
using HCILibrary.HCIResponses;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request GPIO/SFO Status (0x0017).
/// Instructs the matrix to send the status of all GPI and GPO pins on the specified GPIO/SFO card.
/// HCIv2 only.
/// </summary>
public class RequestGpioSfoStatusRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version. Set to 1; future payload changes will increment this.
    /// </summary>
    private const byte ProtocolSchema = 0x01;

    /// <summary>
    /// The card number (0-128).
    /// </summary>
    public byte CardNumber { get; set; }

    /// <summary>
    /// The card type: GPIO or SFO.
    /// </summary>
    public EhxCardType CardType { get; set; } = EhxCardType.Gpio;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestGpioSfoStatusRequest"/> class.
    /// </summary>
    public RequestGpioSfoStatusRequest()
        : base(HCIMessageID.RequestGpioSfoStatus)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestGpioSfoStatusRequest"/> class
    /// with the specified card number and type.
    /// </summary>
    /// <param name="cardNumber">The card number (0-128).</param>
    /// <param name="cardType">The card type (GPIO or SFO).</param>
    public RequestGpioSfoStatusRequest(byte cardNumber, EhxCardType cardType = EhxCardType.Gpio)
        : base(HCIMessageID.RequestGpioSfoStatus)
    {
        if (cardNumber > 128)
            throw new ArgumentOutOfRangeException(nameof(cardNumber), "Card number must be 0-128.");

        CardNumber = cardNumber;
        CardType = cardType;
    }

    /// <summary>
    /// Creates a request for GPIO card status.
    /// </summary>
    /// <param name="cardNumber">The card number (0-128).</param>
    /// <returns>The request.</returns>
    public static RequestGpioSfoStatusRequest ForGpio(byte cardNumber)
        => new(cardNumber, EhxCardType.Gpio);

    /// <summary>
    /// Creates a request for SFO card status.
    /// </summary>
    /// <param name="cardNumber">The card number (0-128).</param>
    /// <returns>The request.</returns>
    public static RequestGpioSfoStatusRequest ForSfo(byte cardNumber)
        => new(cardNumber, EhxCardType.Sfo);

    /// <summary>
    /// Generates the HCIv2 payload for Request GPIO/SFO Status.
    /// Payload: Protocol Tag (4 bytes) + Protocol Schema (1 byte) + Card Number (2 bytes).
    /// </summary>
    /// <returns>The payload byte array.</returns>
    protected override byte[] GeneratePayload()
    {
        using var ms = new MemoryStream();

        // HCIv2 protocol tag
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol schema
        ms.WriteByte(ProtocolSchema);

        // GPIO/SFO card number (16-bit, big-endian)
        // bits 0-14: card number
        // bit 15: card type (0 = GPIO, 1 = SFO)
        ushort cardWord = CardNumber;
        if (CardType == EhxCardType.Sfo)
            cardWord |= 0x8000; // Set bit 15

        ms.WriteByte((byte)((cardWord >> 8) & 0xFF));
        ms.WriteByte((byte)(cardWord & 0xFF));

        return ms.ToArray();
    }
}
