using HCILibrary.Enums;
using HCILibrary.Models;
using HCILibrary.Helpers;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Output Level Actions (0x0023).
/// Instructs the matrix to change the output signal level(s) leaving the CSU
/// for the specified output port(s) to the new gain value(s) (-72dB to +18dB).
/// A gain level of 0 indicates full cut.
/// HCIv2 only.
/// </summary>
public class RequestOutputLevelActionsRequest : HCIRequest
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
    /// The list of output level actions to perform.
    /// Uses the same structure as input level actions.
    /// </summary>
    public List<LevelAction> Actions { get; } = new();

    /// <summary>
    /// Creates a new Request Output Level Actions request.
    /// </summary>
    public RequestOutputLevelActionsRequest()
        : base(HCIMessageID.RequestOutputLevelActions)
    {
    }

    /// <summary>
    /// Adds an output level action.
    /// </summary>
    /// <param name="action">The action to add.</param>
    public void AddAction(LevelAction action)
    {
        Actions.Add(action);
    }

    /// <summary>
    /// Adds an output level action with the specified port and level.
    /// </summary>
    /// <param name="portNumber">Port number (0-1023).</param>
    /// <param name="levelValue">Level value (0-255).</param>
    public void AddAction(ushort portNumber, byte levelValue)
    {
        Actions.Add(new LevelAction(portNumber, levelValue));
    }

    /// <summary>
    /// Sets the output level for a port using a dB value.
    /// </summary>
    /// <param name="portNumber">Port number (0-1023).</param>
    /// <param name="dB">Gain in dB (-72 to +18).</param>
    public void SetLevelDb(ushort portNumber, double dB)
    {
        var level = GainLevel.DbToLevel(dB);
        // Level value in this message is 0-255, so clamp the result
        Actions.Add(new LevelAction(portNumber, (byte)Math.Min((int)level, 255)));
    }

    /// <summary>
    /// Sets the output level for a port to full cut (mute).
    /// </summary>
    /// <param name="portNumber">Port number (0-1023).</param>
    public void SetCut(ushort portNumber)
    {
        Actions.Add(new LevelAction(portNumber, 0));
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request Output Level Actions.
    /// </summary>
    /// <returns>The payload byte array.</returns>
    protected override byte[] GeneratePayload()
    {
        using var ms = new MemoryStream();

        // HCIv2 protocol tag (4 bytes)
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol schema (1 byte)
        ms.WriteByte(ProtocolSchema);

        // Count (16-bit, big-endian)
        ushort count = (ushort)Actions.Count;
        ms.WriteByte((byte)((count >> 8) & 0xFF));
        ms.WriteByte((byte)(count & 0xFF));

        // Action data (4 bytes each: 2 for port + 2 for level)
        foreach (var action in Actions)
        {
            var actionBytes = action.ToBytes();
            ms.Write(actionBytes, 0, actionBytes.Length);
        }

        return ms.ToArray();
    }
}
