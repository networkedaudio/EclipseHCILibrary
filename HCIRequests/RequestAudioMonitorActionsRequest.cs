using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Audio Monitor Actions (HCIv2) - Message ID 0x0011.
/// This message is used by the host to add or remove port monitors in the matrix.
/// A port monitor is another port that receives the same audio mix as the monitored port.
/// This feature would typically be used by a recording or supervision feature.
/// </summary>
public class RequestAudioMonitorActionsRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol tag marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Protocol schema version.
    /// </summary>
    private const byte ProtocolSchema = 0x01;

    /// <summary>
    /// Action type for audio monitor actions (0x0100).
    /// </summary>
    private const ushort ActionTypeAudioMonitor = 0x0100;

    /// <summary>
    /// The list of audio monitor actions to request.
    /// </summary>
    public List<AudioMonitorAction> Actions { get; } = new();

    /// <summary>
    /// Creates a new Request Audio Monitor Actions request.
    /// </summary>
    public RequestAudioMonitorActionsRequest()
        : base(HCIMessageID.RequestAudioMonitorActions)
    {
    }

    /// <summary>
    /// Adds an audio monitor action to the request.
    /// </summary>
    /// <param name="isAdd">true to add monitor, false to delete monitor.</param>
    /// <param name="monitorPort">The monitor port number (0-2047) - receives the mirrored audio.</param>
    /// <param name="targetPort">The target port number (0-2047) - the port being monitored.</param>
    public void AddAction(bool isAdd, ushort monitorPort, ushort targetPort)
    {
        Actions.Add(new AudioMonitorAction(isAdd, monitorPort, targetPort));
    }

    /// <summary>
    /// Adds an action to create a port monitor (add).
    /// </summary>
    /// <param name="monitorPort">The monitor port number (0-2047) - receives the mirrored audio.</param>
    /// <param name="targetPort">The target port number (0-2047) - the port being monitored.</param>
    public void AddMonitor(ushort monitorPort, ushort targetPort)
    {
        AddAction(true, monitorPort, targetPort);
    }

    /// <summary>
    /// Adds an action to remove a port monitor (delete).
    /// </summary>
    /// <param name="monitorPort">The monitor port number (0-2047) - the port receiving mirrored audio.</param>
    /// <param name="targetPort">The target port number (0-2047) - the port being monitored.</param>
    public void RemoveMonitor(ushort monitorPort, ushort targetPort)
    {
        AddAction(false, monitorPort, targetPort);
    }

    /// <summary>
    /// Generates the payload for this request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // ProtocolTag(4) + ProtocolSchema(1) + Count(2) + [ActionType(2) + ActionData(8)] * Count
        int actionDataSize = Actions.Count * 10; // Each action: ActionType(2) + ActionData(8)
        var payload = new byte[4 + 1 + 2 + actionDataSize];

        int offset = 0;

        // Protocol Tag: 4 bytes (0xABBACEDE)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema: 1 byte
        payload[offset++] = ProtocolSchema;

        // Count: 16 bit word (big-endian)
        ushort count = (ushort)Actions.Count;
        payload[offset++] = (byte)(count >> 8);
        payload[offset++] = (byte)(count & 0xFF);

        // Action Data for each action
        foreach (var action in Actions)
        {
            // Action Type: 16 bit word (0x0100 for audio monitor)
            payload[offset++] = (byte)(ActionTypeAudioMonitor >> 8);
            payload[offset++] = (byte)(ActionTypeAudioMonitor & 0xFF);

            // Action: 4 x 16 bit words (8 bytes)
            var actionBytes = action.ToBytes();
            Array.Copy(actionBytes, 0, payload, offset, 8);
            offset += 8;
        }

        return payload;
    }
}
