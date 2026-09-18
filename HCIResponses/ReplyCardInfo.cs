using HCILibrary.Enums;
using System.Reflection.Metadata.Ecma335;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents information about a single card.
/// </summary>
public class CardInfo
{
    /// <summary>
    /// Slot type.
    /// </summary>
    public SlotType SlotType { get; set; }

    /// <summary>
    /// Expected card type.
    /// </summary>
    public CardInfoType ExpectedCardType { get; set; }

    /// <summary>
    /// Current card type.
    /// </summary>
    public CardInfoType CurrentCardType { get; set; }

    /// <summary>
    /// Health status of the card.
    /// </summary>
    public CardHealth Health { get; set; }

    /// <summary>
    /// Whether this is a rear connector unit (bit 0).
    /// </summary>
    public bool RearConnectorUnit { get; set; }

    /// <summary>
    /// Rack number (starting from 1).
    /// </summary>
    public int RackNumber { get; set; }

    /// <summary>
    /// Slot number.
    /// </summary>
    public int SlotNumber { get; set; }

    public int AudioCardNumber
    {
        // CPU cards are P1 and P2 so audio cards numbering is off by 2
        // But the frame is zero indexed
        get => SlotNumber - 1;
    }
    /// <summary>
    /// First port on card.
    /// </summary>
    public int FirstPort { get; set; }

    /// <summary>
    /// Last port on card.
    /// </summary>
    public int LastPort { get; set; }

    /// <summary>
    /// Number of channels (typically number of ports).
    /// </summary>
    public int Channels { get; set; }

    /// <summary>
    /// DTMF board present (MVX card only).
    /// Bitmap of DTMF cards. 0xFFFF indicates all 16 ports have a DTMF encoder/decoder.
    /// </summary>
    public ushort DtmfBoardPresent { get; set; }

    /// <summary>
    /// Application version string (up to 64 bytes).
    /// </summary>
    public string AppVersionStr { get; set; } = string.Empty;

    /// <summary>
    /// Boot version string (up to 64 bytes).
    /// </summary>
    public string BootVersionStr { get; set; } = string.Empty;

    /// <summary>
    /// FPGA version string (up to 64 bytes).
    /// </summary>
    public string FpgaVersionStr { get; set; } = string.Empty;
}

/// <summary>
/// Reply Card Info (0x00C4).
/// Returns the card information at a specified slot together with its health status.
/// </summary>
public class ReplyCardInfo
{
    /// <summary>
    /// Number of card infos in this reply.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// List of card info entries.
    /// </summary>
    public List<CardInfo> Cards { get; set; } = new();

    /// <summary>
    /// Parses the payload of a Reply Card Info message.
    /// </summary>
    /// <param name="payload">The message payload (after protocol schema byte).</param>
    /// <returns>The parsed ReplyCardInfo, or null if parsing fails.</returns>
    public static ReplyCardInfo? Parse(byte[] payload)
    {
        // Minimum payload: Count(1) = 1 byte
        if (payload == null || payload.Length < 1)
        {
            return null;
        }

        var result = new ReplyCardInfo();

        // Each card info entry (matches firmware ecs_hci_card_info_t, packed):
        // SlotType(1) + ExpectedCardType(1) + CurrentCardType(1) + Health(1) +
        // RearConnector(1) + RackNumber(1) + SlotNumber(1) + FirstPort(2) + LastPort(2) +
        // Channels(1) + DtmfBoardPresent(2) + AppVersionStr(64) + BootVersionStr(64) + FpgaVersionStr(64)
        // = 206 bytes per entry
        const int cardInfoSize = 206;

        // The matrix only prefixes the Count byte on the FIRST message of a burst;
        // continuation messages carry raw entries with no count. Detect which case
        // this payload is by whether the length leaves a single leading byte.
        int offset;
        int entryCount;
        if (payload.Length % cardInfoSize == 1)
        {
            // Leading Count byte present.
            result.Count = payload[0];
            entryCount = payload[0];
            offset = 1;
        }
        else
        {
            // No count byte: entries start at the beginning of the payload.
            entryCount = payload.Length / cardInfoSize;
            result.Count = (byte)entryCount;
            offset = 0;
        }

        for (int i = 0; i < entryCount; i++)
        {
            if (payload.Length < offset + cardInfoSize)
            {
                break;
            }

            var cardInfo = new CardInfo();

            // Slot type: 1 byte
            byte slotTypeValue = payload[offset];
            cardInfo.SlotType = Enum.IsDefined(typeof(SlotType), slotTypeValue)
                ? (SlotType)slotTypeValue
                : SlotType.NoSlot;
            offset++;

            // Expected card type: 1 byte
            byte expectedCardTypeValue = payload[offset];
            cardInfo.ExpectedCardType = Enum.IsDefined(typeof(CardInfoType), expectedCardTypeValue)
                ? (CardInfoType)expectedCardTypeValue
                : CardInfoType.Unknown;
            offset++;

            // Current card type: 1 byte
            byte currentCardTypeValue = payload[offset];
            cardInfo.CurrentCardType = Enum.IsDefined(typeof(CardInfoType), currentCardTypeValue)
                ? (CardInfoType)currentCardTypeValue
                : CardInfoType.Unknown;
            offset++;

            // Health: 1 byte
            byte healthValue = payload[offset];
            cardInfo.Health = Enum.IsDefined(typeof(CardHealth), healthValue)
                ? (CardHealth)healthValue
                : CardHealth.Absent;
            offset++;

            // Rear connector unit: bit 0 of 1 byte
            cardInfo.RearConnectorUnit = (payload[offset] & 0x01) != 0;
            offset++;

            // Rack number: 1 byte
            cardInfo.RackNumber = payload[offset];
            offset++;

            // Slot number: 1 byte
            cardInfo.SlotNumber = payload[offset];
            // CPU takes up first two slots, so the first audio card is slot 2.
            offset++;

            // First port: 2 bytes (big-endian)
            cardInfo.FirstPort = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Last port: 2 bytes (big-endian)
            cardInfo.LastPort = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // Channels: 1 byte
            cardInfo.Channels = payload[offset];
            offset++;

            // DTMF board present: 2 bytes (big-endian)
            cardInfo.DtmfBoardPresent = (ushort)((payload[offset] << 8) | payload[offset + 1]);
            offset += 2;

            // App version string: 64 bytes
            cardInfo.AppVersionStr = ParseNullTerminatedString(payload, offset, 64);
            offset += 64;

            // Boot version string: 64 bytes
            cardInfo.BootVersionStr = ParseNullTerminatedString(payload, offset, 64);
            offset += 64;

            // FPGA version string: 64 bytes
            cardInfo.FpgaVersionStr = ParseNullTerminatedString(payload, offset, 64);
            offset += 64;

            result.Cards.Add(cardInfo);
        }

        return result;
    }

    /// <summary>
    /// Parses a null-terminated string from the payload.
    /// </summary>
    private static string ParseNullTerminatedString(byte[] payload, int offset, int maxLength)
    {
        int length = 0;
        for (int i = 0; i < maxLength && offset + i < payload.Length; i++)
        {
            if (payload[offset + i] == 0)
            {
                break;
            }
            length++;
        }
        return length > 0 ? System.Text.Encoding.ASCII.GetString(payload, offset, length) : string.Empty;
    }
}
