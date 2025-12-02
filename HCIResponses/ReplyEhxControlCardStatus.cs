using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents the status of a single EHX control card.
/// </summary>
public class EhxControlCardInfo
{
    /// <summary>
    /// Card number (bits 0-12, range 0-8191).
    /// </summary>
    public ushort CardNumber { get; set; }

    /// <summary>
    /// Card status: true = present, false = absent.
    /// </summary>
    public bool IsPresent { get; set; }

    /// <summary>
    /// Card type: GPIO or SFO.
    /// </summary>
    public EhxCardType CardType { get; set; }

    /// <summary>
    /// Parses card data from a 16-bit word.
    /// </summary>
    /// <param name="data">The 16-bit card data.</param>
    /// <returns>The parsed card info.</returns>
    public static EhxControlCardInfo FromWord(ushort data)
    {
        return new EhxControlCardInfo
        {
            CardNumber = (ushort)(data & 0x1FFF),        // bits 0-12
            IsPresent = (data & 0x4000) != 0,            // bit 14
            CardType = (data & 0x8000) != 0 ? EhxCardType.Sfo : EhxCardType.Gpio  // bit 15
        };
    }
}

/// <summary>
/// Reply EHX Control Card Status (0x0016).
/// Contains the current state of the Matrix GPIOs and EHX Control cards.
/// Generated when pins on a control change state or in response to Request EHX Control Card Status.
/// </summary>
public class ReplyEhxControlCardStatus
{
    /// <summary>
    /// Protocol schema version from the response.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// The list of control card statuses.
    /// </summary>
    public List<EhxControlCardInfo> Cards { get; } = new();

    /// <summary>
    /// Parses a Reply EHX Control Card Status response from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags, starting at protocol tag).</param>
    /// <returns>The parsed response.</returns>
    public static ReplyEhxControlCardStatus Parse(byte[] payload)
    {
        var result = new ReplyEhxControlCardStatus();
        int offset = 0;

        // Skip protocol tag (4 bytes) - already validated by caller
        offset += 4;

        // Protocol schema (1 byte)
        result.ProtocolSchema = payload[offset++];

        // Card count (16-bit, big-endian)
        ushort cardCount = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Card data (2 bytes each)
        for (int i = 0; i < cardCount; i++)
        {
            if (offset + 2 <= payload.Length)
            {
                ushort cardData = (ushort)((payload[offset] << 8) | payload[offset + 1]);
                result.Cards.Add(EhxControlCardInfo.FromWord(cardData));
                offset += 2;
            }
        }

        return result;
    }
}
