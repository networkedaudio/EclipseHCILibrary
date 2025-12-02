using HCILibrary.Enums;

namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents an IFB element value entry.
/// </summary>
public class IfbElementValue
{
    /// <summary>
    /// Entry action (Deleted, Added, AddedPending, Present, Edited).
    /// </summary>
    public IfbEntryAction Action { get; set; }

    /// <summary>
    /// Raw value bytes.
    /// </summary>
    public byte[] RawValue { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// For IFB_INT_LEVEL: Whether the IFB has been triggered (non-zero means being dimmed).
    /// </summary>
    public bool IsTriggered => RawValue.Length >= 1 && RawValue[0] != 0;

    /// <summary>
    /// For IFB_DIM_LEVEL: The dim level value.
    /// </summary>
    public IfbDimLevel DimLevel => RawValue.Length >= 1 ? (IfbDimLevel)RawValue[0] : IfbDimLevel.Db0;

    /// <summary>
    /// For dial code types (4 bytes): The dial code value.
    /// </summary>
    public uint DialCode
    {
        get
        {
            if (RawValue.Length >= 4)
            {
                return (uint)((RawValue[0] << 24) | (RawValue[1] << 16) | (RawValue[2] << 8) | RawValue[3]);
            }
            return 0;
        }
    }

    /// <summary>
    /// For IFB_PRIORITY (5 bytes): The priority value (5th byte).
    /// </summary>
    public byte Priority => RawValue.Length >= 5 ? RawValue[4] : (byte)0;
}

/// <summary>
/// Represents a single IFB attribute entry in the reply.
/// </summary>
public class IfbAttributeEntry
{
    /// <summary>
    /// The IFB attribute type.
    /// </summary>
    public IfbAttributeType AttributeType { get; set; }

    /// <summary>
    /// The matrix identifier that the IFB belongs to.
    /// </summary>
    public byte MatrixIdentifier { get; set; }

    /// <summary>
    /// Unique IFB identifier within the context of this frame.
    /// </summary>
    public ushort IfbIdentifier { get; set; }

    /// <summary>
    /// Indicates whether the values for this attribute represent the complete known state of the IFB.
    /// </summary>
    public bool IsAbsoluteState { get; set; }

    /// <summary>
    /// Number of elements in this property type list.
    /// </summary>
    public ushort ElementCount { get; set; }

    /// <summary>
    /// List of element values.
    /// </summary>
    public List<IfbElementValue> Elements { get; set; } = new();
}

/// <summary>
/// Reply IFB Status (0x003E).
/// Response to Request IFB Status. Contains IFB attribute information.
/// Also sent as unsolicited message when attributes are edited.
/// </summary>
public class ReplyIfbStatus
{
    /// <summary>
    /// Protocol schema version from the response.
    /// </summary>
    public byte ProtocolSchema { get; set; }

    /// <summary>
    /// Number of attribute entries.
    /// </summary>
    public ushort Count { get; set; }

    /// <summary>
    /// List of IFB attribute entries.
    /// </summary>
    public List<IfbAttributeEntry> Attributes { get; set; } = new();

    /// <summary>
    /// Gets the value size for a given attribute type.
    /// </summary>
    /// <param name="attributeType">The attribute type.</param>
    /// <returns>The size in bytes of the value field.</returns>
    public static int GetValueSize(IfbAttributeType attributeType)
    {
        return attributeType switch
        {
            IfbAttributeType.IntLevel => 1,
            IfbAttributeType.DimLevel => 1,
            IfbAttributeType.Priority => 5,  // Dial code (4) + Priority (1)
            IfbAttributeType.ActiveCallers => 4,  // Dial code
            IfbAttributeType.Sources => 4,  // Dial code
            IfbAttributeType.Destination => 4,  // Dial code
            IfbAttributeType.Returns => 4,  // Dial code
            IfbAttributeType.PotentialCallers => 4,  // Dial code
            IfbAttributeType.ReturnListens => 4,  // Dial code (assumed)
            IfbAttributeType.DestinationListens => 4,  // Dial code (assumed)
            _ => 4  // Default to 4 bytes
        };
    }

    /// <summary>
    /// Parses a Reply IFB Status response from the payload bytes.
    /// </summary>
    /// <param name="payload">The payload bytes (after flags, starting at protocol schema).</param>
    /// <returns>The parsed response, or null if parsing fails.</returns>
    public static ReplyIfbStatus? Parse(byte[] payload)
    {
        // Minimum payload: ProtocolSchema(1) + Count(2) = 3 bytes
        if (payload == null || payload.Length < 3)
        {
            return null;
        }

        int offset = 0;
        var result = new ReplyIfbStatus();

        // Protocol Schema (1 byte)
        result.ProtocolSchema = payload[offset++];

        // Count (2 bytes, big-endian)
        result.Count = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Parse each attribute entry
        for (int i = 0; i < result.Count; i++)
        {
            var entry = ParseAttributeEntry(payload, ref offset);
            if (entry == null)
            {
                break;
            }
            result.Attributes.Add(entry);
        }

        return result;
    }

    private static IfbAttributeEntry? ParseAttributeEntry(byte[] payload, ref int offset)
    {
        // Minimum entry header:
        // AttributeType(1) + MatrixIdentifier(1) + IfbIdentifier(2) + IsAbsoluteState(1) + ElementCount(2) = 7 bytes
        const int minEntryHeaderSize = 7;

        if (payload.Length < offset + minEntryHeaderSize)
        {
            return null;
        }

        var entry = new IfbAttributeEntry();

        // IFB Attribute Type (1 byte)
        entry.AttributeType = (IfbAttributeType)payload[offset++];

        // Matrix Identifier (1 byte)
        entry.MatrixIdentifier = payload[offset++];

        // IFB Identifier (2 bytes, big-endian)
        entry.IfbIdentifier = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Is Absolute State (1 byte)
        entry.IsAbsoluteState = payload[offset++] != 0;

        // Element Count (2 bytes, big-endian)
        entry.ElementCount = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        // Get value size for this attribute type
        int valueSize = GetValueSize(entry.AttributeType);

        // Parse each element
        // Each element: Action(1) + Value(valueSize)
        int elementSize = 1 + valueSize;

        for (int e = 0; e < entry.ElementCount; e++)
        {
            if (payload.Length < offset + elementSize)
            {
                break;
            }

            var element = new IfbElementValue();

            // Entry Action (1 byte)
            element.Action = (IfbEntryAction)payload[offset++];

            // Value (variable size)
            element.RawValue = new byte[valueSize];
            Array.Copy(payload, offset, element.RawValue, 0, valueSize);
            offset += valueSize;

            entry.Elements.Add(element);
        }

        return entry;
    }
}
