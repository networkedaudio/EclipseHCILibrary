using HCILibrary.Enums;
using HCILibrary.Models;
using HCILibrary.Helpers;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Crosspoint Level Actions (HCIv2) - Message ID 0x0026.
/// This message asks the CSU to change the gain level(s) of the specified crosspoint(s).
/// </summary>
public class RequestCrosspointLevelActionsRequest : HCIRequest
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
    /// The list of crosspoint level actions.
    /// </summary>
    public List<CrosspointLevelAction> Actions { get; } = new();

    /// <summary>
    /// Creates a new Request Crosspoint Level Actions request.
    /// </summary>
    public RequestCrosspointLevelActionsRequest()
        : base(HCIMessageID.RequestCrosspointLevelActions)
    {
    }

    /// <summary>
    /// Adds a crosspoint level action using a level value.
    /// </summary>
    /// <param name="destinationPort">The destination port number (0-1023).</param>
    /// <param name="sourcePort">The source port number (0-1023).</param>
    /// <param name="levelValue">The level value (0-287). Use GainLevel constants.</param>
    public void AddAction(ushort destinationPort, ushort sourcePort, ushort levelValue)
    {
        Actions.Add(new CrosspointLevelAction(destinationPort, sourcePort, levelValue));
    }

    /// <summary>
    /// Adds a crosspoint level action using a gain in dB.
    /// </summary>
    /// <param name="destinationPort">The destination port number (0-1023).</param>
    /// <param name="sourcePort">The source port number (0-1023).</param>
    /// <param name="gainDb">The gain in dB (-72 to +29).</param>
    public void AddActionDb(ushort destinationPort, ushort sourcePort, double gainDb)
    {
        ushort level = GainLevel.DbToLevel(gainDb);
        Actions.Add(new CrosspointLevelAction(destinationPort, sourcePort, level));
    }

    /// <summary>
    /// Adds a crosspoint mute (cut) action.
    /// </summary>
    /// <param name="destinationPort">The destination port number (0-1023).</param>
    /// <param name="sourcePort">The source port number (0-1023).</param>
    public void AddMute(ushort destinationPort, ushort sourcePort)
    {
        Actions.Add(new CrosspointLevelAction(destinationPort, sourcePort, GainLevel.Cut));
    }

    /// <summary>
    /// Adds a crosspoint unity gain (0 dB) action.
    /// </summary>
    /// <param name="destinationPort">The destination port number (0-1023).</param>
    /// <param name="sourcePort">The source port number (0-1023).</param>
    public void AddUnityGain(ushort destinationPort, ushort sourcePort)
    {
        Actions.Add(new CrosspointLevelAction(destinationPort, sourcePort, GainLevel.Unity0dB));
    }

    /// <summary>
    /// Generates the payload for this request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload: ProtocolTag(4) + ProtocolSchema(1) + Count(2) + 
        //          [DestPort(2) + SrcPort(2) + Level(2)] * Count
        int actionDataSize = Actions.Count * 6; // Each action: 3 x 16-bit words
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
            // Destination Port: 16 bit word (big-endian)
            payload[offset++] = (byte)(action.DestinationPort >> 8);
            payload[offset++] = (byte)(action.DestinationPort & 0xFF);

            // Source Port: 16 bit word (big-endian)
            payload[offset++] = (byte)(action.SourcePort >> 8);
            payload[offset++] = (byte)(action.SourcePort & 0xFF);

            // Level Value: 16 bit word (big-endian)
            payload[offset++] = (byte)(action.LevelValue >> 8);
            payload[offset++] = (byte)(action.LevelValue & 0xFF);
        }

        return payload;
    }
}
