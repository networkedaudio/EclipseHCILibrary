using HCILibrary.Enums;
using HCILibrary.Models;
using HCILibrary.Helpers;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Input Level Actions (0x0020).
/// Instructs the matrix to change the input gain for the specified input port(s).
/// HCIv2 only.
/// </summary>
public class RequestInputLevelActionsRequest : HCIRequest
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
    /// The list of input level actions to perform.
    /// </summary>
    public List<LevelAction> Actions { get; } = new();

    /// <summary>
    /// Creates a new Request Input Level Actions request.
    /// </summary>
    public RequestInputLevelActionsRequest()
        : base(HCIMessageID.RequestInputLevelActions)
    {
    }

    /// <summary>
    /// Adds an input level action.
    /// </summary>
    /// <param name="action">The action to add.</param>
    public void AddAction(LevelAction action)
    {
        Actions.Add(action);
    }

    /// <summary>
    /// Adds an input level action with the specified port and level.
    /// </summary>
    /// <param name="portNumber">Port number (0-1023).</param>
    /// <param name="levelValue">Level value (0-255).</param>
    public void AddAction(ushort portNumber, byte levelValue)
    {
        Actions.Add(new LevelAction(portNumber, levelValue));
    }

    /// <summary>
    /// Sets the input level for a port using a dB value.
    /// </summary>
    /// <param name="portNumber">Port number (0-1023).</param>
    /// <param name="dB">Gain in dB (-72 to +29).</param>
    public void SetLevelDb(ushort portNumber, double dB)
    {
        var level = GainLevel.DbToLevel(dB);
        // Level value in this message is 0-255, so clamp the result
        Actions.Add(new LevelAction(portNumber, (byte)Math.Min((int)level, 255)));
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request Input Level Actions.
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
