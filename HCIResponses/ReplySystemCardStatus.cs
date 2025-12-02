using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents the status of a single card in the matrix.
/// </summary>
public class CardStatus
{
    /// <summary>
    /// The raw 16-bit status word.
    /// </summary>
    public ushort RawStatus { get; set; }

    /// <summary>
    /// The condition code of the card (bits 0-7).
    /// </summary>
    public CardCondition Condition { get; set; }

    /// <summary>
    /// The card type (bits 8-14).
    /// </summary>
    public CardType CardType { get; set; }

    /// <summary>
    /// Whether this is slot 0 of the rack (bit 15).
    /// </summary>
    public bool IsSlotZero { get; set; }

    /// <summary>
    /// Parses a 16-bit status word into a CardStatus object.
    /// </summary>
    /// <param name="status">The raw 16-bit status word.</param>
    /// <returns>The parsed CardStatus.</returns>
    public static CardStatus Parse(ushort status)
    {
        byte conditionValue = (byte)(status & 0xFF);
        byte cardTypeValue = (byte)((status >> 8) & 0x7F);
        bool isSlotZero = (status & 0x8000) != 0;

        return new CardStatus
        {
            RawStatus = status,
            Condition = Enum.IsDefined(typeof(CardCondition), conditionValue)
                ? (CardCondition)conditionValue
                : CardCondition.Unknown,
            CardType = Enum.IsDefined(typeof(CardType), cardTypeValue)
                ? (CardType)cardTypeValue
                : CardType.Unknown,
            IsSlotZero = isSlotZero
        };
    }
}

/// <summary>
/// Reply System Card Status (0x0004).
/// Contains the current status of all cards in the matrix.
/// Sent when a card changes state or in response to Request System Status.
/// </summary>
public class ReplySystemCardStatus
{
    /// <summary>
    /// The number of card status entries in this reply.
    /// </summary>
    public ushort Count { get; set; }

    /// <summary>
    /// The list of card status entries.
    /// </summary>
    public List<CardStatus> Cards { get; set; } = new();

    /// <summary>
    /// Parses the payload of a Reply System Card Status message.
    /// </summary>
    /// <param name="payload">The message payload (after protocol schema byte).</param>
    /// <returns>The parsed ReplySystemCardStatus, or null if parsing fails.</returns>
    public static ReplySystemCardStatus? Parse(byte[] payload)
    {
        // Minimum payload: Count(2) = 2 bytes
        if (payload == null || payload.Length < 2)
        {
            return null;
        }

        var result = new ReplySystemCardStatus();

        // Count: 16 bit word (big-endian)
        result.Count = (ushort)((payload[0] << 8) | payload[1]);

        // Each status entry is 2 bytes
        int offset = 2;
        for (int i = 0; i < result.Count && offset + 1 < payload.Length; i++)
        {
            ushort status = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            result.Cards.Add(CardStatus.Parse(status));
            offset += 2;
        }

        return result;
    }
}
