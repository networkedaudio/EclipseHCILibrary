using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Represents an IFB element value for Request IFB Set.
/// </summary>
public class IfbSetElementValue
{
    /// <summary>
    /// Entry action (Deleted, Added, AddedPending, Present, Edited).
    /// </summary>
    public IfbEntryAction Action { get; set; }

    /// <summary>
    /// Raw value bytes. Size depends on attribute type.
    /// </summary>
    public byte[] RawValue { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Creates an element for IFB_INT_LEVEL.
    /// </summary>
    /// <param name="action">The entry action.</param>
    /// <param name="triggered">True if IFB is triggered (being dimmed).</param>
    public static IfbSetElementValue CreateIntLevel(IfbEntryAction action, bool triggered)
    {
        return new IfbSetElementValue
        {
            Action = action,
            RawValue = new byte[] { (byte)(triggered ? 1 : 0) }
        };
    }

    /// <summary>
    /// Creates an element for IFB_DIM_LEVEL.
    /// </summary>
    /// <param name="action">The entry action.</param>
    /// <param name="dimLevel">The dim level.</param>
    public static IfbSetElementValue CreateDimLevel(IfbEntryAction action, IfbDimLevel dimLevel)
    {
        return new IfbSetElementValue
        {
            Action = action,
            RawValue = new byte[] { (byte)dimLevel }
        };
    }

    /// <summary>
    /// Creates an element for dial code types (ACTIVE_CALLERS, SOURCES, DESTINATION, RETURNS, POTENTIAL_CALLERS).
    /// </summary>
    /// <param name="action">The entry action.</param>
    /// <param name="dialCode">The 4-byte dial code.</param>
    public static IfbSetElementValue CreateDialCode(IfbEntryAction action, uint dialCode)
    {
        return new IfbSetElementValue
        {
            Action = action,
            RawValue = new byte[]
            {
                (byte)((dialCode >> 24) & 0xFF),
                (byte)((dialCode >> 16) & 0xFF),
                (byte)((dialCode >> 8) & 0xFF),
                (byte)(dialCode & 0xFF)
            }
        };
    }

    /// <summary>
    /// Creates an element for IFB_PRIORITY.
    /// </summary>
    /// <param name="action">The entry action.</param>
    /// <param name="dialCode">The 4-byte dial code.</param>
    /// <param name="priority">The priority value.</param>
    public static IfbSetElementValue CreatePriority(IfbEntryAction action, uint dialCode, byte priority)
    {
        return new IfbSetElementValue
        {
            Action = action,
            RawValue = new byte[]
            {
                (byte)((dialCode >> 24) & 0xFF),
                (byte)((dialCode >> 16) & 0xFF),
                (byte)((dialCode >> 8) & 0xFF),
                (byte)(dialCode & 0xFF),
                priority
            }
        };
    }
}

/// <summary>
/// Represents a single IFB attribute entry for Request IFB Set.
/// </summary>
public class IfbSetAttributeEntry
{
    /// <summary>
    /// The IFB attribute type.
    /// </summary>
    public IfbAttributeType AttributeType { get; set; }

    /// <summary>
    /// Unique IFB identifier within the context of this frame.
    /// </summary>
    public ushort IfbInstance { get; set; }

    /// <summary>
    /// List of element values.
    /// </summary>
    public List<IfbSetElementValue> Elements { get; set; } = new();

    /// <summary>
    /// Gets the value size for this attribute type.
    /// </summary>
    private int ValueSize => AttributeType switch
    {
        IfbAttributeType.IntLevel => 1,
        IfbAttributeType.DimLevel => 1,
        IfbAttributeType.Priority => 5,
        _ => 4  // Default dial code size
    };

    /// <summary>
    /// Converts this entry to bytes.
    /// </summary>
    public byte[] ToBytes()
    {
        var bytes = new List<byte>();

        // IFB Attribute Type (1 byte)
        bytes.Add((byte)AttributeType);

        // IFB Instance (2 bytes, big-endian)
        bytes.Add((byte)((IfbInstance >> 8) & 0xFF));
        bytes.Add((byte)(IfbInstance & 0xFF));

        // Element Count (2 bytes, big-endian)
        ushort elementCount = (ushort)Elements.Count;
        bytes.Add((byte)((elementCount >> 8) & 0xFF));
        bytes.Add((byte)(elementCount & 0xFF));

        // Elements
        foreach (var element in Elements)
        {
            // Entry Action (1 byte)
            bytes.Add((byte)element.Action);

            // Value (variable size)
            bytes.AddRange(element.RawValue);
        }

        return bytes.ToArray();
    }
}

/// <summary>
/// Request IFB Set (0x003F).
/// Requests an IFB property edit, add, or delete.
/// The matrix will respond with Reply IFB Status containing only the attributes
/// that have been deleted, added, or edited.
/// HCIv2 only.
/// </summary>
public class RequestIfbSetRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Gets or sets the protocol schema version. Currently set to 1.
    /// </summary>
    public byte ProtocolSchema { get; set; } = 1;

    /// <summary>
    /// List of IFB attribute entries to set.
    /// </summary>
    public List<IfbSetAttributeEntry> Attributes { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestIfbSetRequest"/> class.
    /// </summary>
    public RequestIfbSetRequest()
        : base(HCIMessageID.RequestIfbSet)
    {
    }

    /// <summary>
    /// Adds an attribute entry to the request.
    /// </summary>
    /// <param name="entry">The attribute entry to add.</param>
    /// <returns>This request for chaining.</returns>
    public RequestIfbSetRequest AddAttribute(IfbSetAttributeEntry entry)
    {
        Attributes.Add(entry);
        return this;
    }

    /// <summary>
    /// Creates a request to set the dim level for an IFB.
    /// </summary>
    /// <param name="ifbInstance">The IFB identifier.</param>
    /// <param name="dimLevel">The dim level to set.</param>
    /// <param name="action">The entry action (default: Edited).</param>
    public static RequestIfbSetRequest CreateSetDimLevel(ushort ifbInstance, IfbDimLevel dimLevel, 
        IfbEntryAction action = IfbEntryAction.Edited)
    {
        var request = new RequestIfbSetRequest();
        var entry = new IfbSetAttributeEntry
        {
            AttributeType = IfbAttributeType.DimLevel,
            IfbInstance = ifbInstance
        };
        entry.Elements.Add(IfbSetElementValue.CreateDimLevel(action, dimLevel));
        request.Attributes.Add(entry);
        return request;
    }

    /// <summary>
    /// Creates a request to trigger or untrigger an IFB.
    /// </summary>
    /// <param name="ifbInstance">The IFB identifier.</param>
    /// <param name="triggered">True to trigger, false to untrigger.</param>
    /// <param name="action">The entry action (default: Edited).</param>
    public static RequestIfbSetRequest CreateSetIntLevel(ushort ifbInstance, bool triggered, 
        IfbEntryAction action = IfbEntryAction.Edited)
    {
        var request = new RequestIfbSetRequest();
        var entry = new IfbSetAttributeEntry
        {
            AttributeType = IfbAttributeType.IntLevel,
            IfbInstance = ifbInstance
        };
        entry.Elements.Add(IfbSetElementValue.CreateIntLevel(action, triggered));
        request.Attributes.Add(entry);
        return request;
    }

    /// <summary>
    /// Creates a request to add a source to an IFB.
    /// </summary>
    /// <param name="ifbInstance">The IFB identifier.</param>
    /// <param name="dialCode">The dial code of the source.</param>
    public static RequestIfbSetRequest CreateAddSource(ushort ifbInstance, uint dialCode)
    {
        var request = new RequestIfbSetRequest();
        var entry = new IfbSetAttributeEntry
        {
            AttributeType = IfbAttributeType.Sources,
            IfbInstance = ifbInstance
        };
        entry.Elements.Add(IfbSetElementValue.CreateDialCode(IfbEntryAction.Added, dialCode));
        request.Attributes.Add(entry);
        return request;
    }

    /// <summary>
    /// Creates a request to delete a source from an IFB.
    /// </summary>
    /// <param name="ifbInstance">The IFB identifier.</param>
    /// <param name="dialCode">The dial code of the source to delete.</param>
    public static RequestIfbSetRequest CreateDeleteSource(ushort ifbInstance, uint dialCode)
    {
        var request = new RequestIfbSetRequest();
        var entry = new IfbSetAttributeEntry
        {
            AttributeType = IfbAttributeType.Sources,
            IfbInstance = ifbInstance
        };
        entry.Elements.Add(IfbSetElementValue.CreateDialCode(IfbEntryAction.Deleted, dialCode));
        request.Attributes.Add(entry);
        return request;
    }

    /// <summary>
    /// Generates the payload for the Request IFB Set message.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        var payload = new List<byte>();

        // Protocol Tag (4 bytes)
        payload.AddRange(ProtocolTag);

        // Protocol Schema (1 byte)
        payload.Add(ProtocolSchema);

        // Count (2 bytes, big-endian)
        ushort count = (ushort)Attributes.Count;
        payload.Add((byte)((count >> 8) & 0xFF));
        payload.Add((byte)(count & 0xFF));

        // Attribute entries
        foreach (var attribute in Attributes)
        {
            payload.AddRange(attribute.ToBytes());
        }

        return payload.ToArray();
    }
}
