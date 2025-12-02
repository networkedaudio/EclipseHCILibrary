using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Represents a single forced listen action entry.
/// </summary>
public class ForcedListenActionEntry
{
    /// <summary>
    /// The system number of the source port.
    /// </summary>
    public byte SourceSystemNumber { get; set; }

    /// <summary>
    /// The source port number.
    /// </summary>
    public ushort SourcePort { get; set; }

    /// <summary>
    /// The destination port number.
    /// </summary>
    public ushort DestinationPort { get; set; }

    /// <summary>
    /// The edit action (Add or Remove forced listen).
    /// </summary>
    public ForcedListenAction EditAction { get; set; }

    /// <summary>
    /// Converts the entry to bytes.
    /// Entry size: Unused(1) + SourceSystemNumber(1) + SourcePort(2) + DestinationPort(2) + EditAction(1) = 7 bytes
    /// </summary>
    public byte[] ToBytes()
    {
        return new byte[]
        {
            0x00, // Unused byte
            SourceSystemNumber,
            (byte)((SourcePort >> 8) & 0xFF),
            (byte)(SourcePort & 0xFF),
            (byte)((DestinationPort >> 8) & 0xFF),
            (byte)(DestinationPort & 0xFF),
            (byte)EditAction
        };
    }
}

/// <summary>
/// Request Forced Listen Actions (0x00E1).
/// Allows the host to set a forced listen crosspoint in the matrix.
/// </summary>
public class RequestForcedListenActionsRequest : HCIRequest
{
    /// <summary>
    /// List of forced listen action entries.
    /// </summary>
    public List<ForcedListenActionEntry> Actions { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestForcedListenActionsRequest"/> class.
    /// </summary>
    public RequestForcedListenActionsRequest()
        : base(HCIMessageID.RequestForcedListenActions)
    {
    }

    /// <summary>
    /// Generates the payload for the Request Forced Listen Actions message.
    /// </summary>
    /// <returns>The message payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload: Count(2) + Actions(7 * count)
        const int entrySize = 7;
        int totalSize = 2 + (Actions.Count * entrySize);

        var payload = new byte[totalSize];
        int offset = 0;

        // Count: 2 bytes (big-endian)
        payload[offset++] = (byte)((Actions.Count >> 8) & 0xFF);
        payload[offset++] = (byte)(Actions.Count & 0xFF);

        // Action entries
        foreach (var action in Actions)
        {
            var actionBytes = action.ToBytes();
            Array.Copy(actionBytes, 0, payload, offset, actionBytes.Length);
            offset += actionBytes.Length;
        }

        return payload;
    }

    /// <summary>
    /// Adds a forced listen action entry.
    /// </summary>
    /// <param name="entry">The forced listen action entry to add.</param>
    public void AddAction(ForcedListenActionEntry entry)
    {
        Actions.Add(entry);
    }

    /// <summary>
    /// Adds a forced listen action with the specified parameters.
    /// </summary>
    /// <param name="sourceSystemNumber">The system number of the source port.</param>
    /// <param name="sourcePort">The source port number.</param>
    /// <param name="destinationPort">The destination port number.</param>
    /// <param name="action">The edit action (Add or Remove).</param>
    public void AddAction(byte sourceSystemNumber, ushort sourcePort, ushort destinationPort, ForcedListenAction action)
    {
        Actions.Add(new ForcedListenActionEntry
        {
            SourceSystemNumber = sourceSystemNumber,
            SourcePort = sourcePort,
            DestinationPort = destinationPort,
            EditAction = action
        });
    }

    /// <summary>
    /// Adds a forced listen (shorthand for Add action).
    /// </summary>
    /// <param name="sourceSystemNumber">The system number of the source port.</param>
    /// <param name="sourcePort">The source port number.</param>
    /// <param name="destinationPort">The destination port number.</param>
    public void AddForcedListen(byte sourceSystemNumber, ushort sourcePort, ushort destinationPort)
    {
        AddAction(sourceSystemNumber, sourcePort, destinationPort, ForcedListenAction.Add);
    }

    /// <summary>
    /// Removes a forced listen (shorthand for Remove action).
    /// </summary>
    /// <param name="sourceSystemNumber">The system number of the source port.</param>
    /// <param name="sourcePort">The source port number.</param>
    /// <param name="destinationPort">The destination port number.</param>
    public void RemoveForcedListen(byte sourceSystemNumber, ushort sourcePort, ushort destinationPort)
    {
        AddAction(sourceSystemNumber, sourcePort, destinationPort, ForcedListenAction.Remove);
    }
}
