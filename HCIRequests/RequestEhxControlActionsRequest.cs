using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request EHX Control Actions (0x0011).
/// Used to change the state of general-purpose outputs (GPOs) and EHX controls.
/// EHX controls can be used within the CSU configuration map to trigger matrix
/// GPO-Relays, routes, logic. They can also be attached to matrix GPI-Inputs
/// and triggered by an external source.
/// HCIv2 only.
/// </summary>
public class RequestEhxControlActionsRequest : HCIRequest
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
    /// The list of EHX control actions to perform.
    /// </summary>
    public List<EhxControlAction> Actions { get; } = new();

    /// <summary>
    /// Creates a new Request EHX Control Actions request.
    /// </summary>
    public RequestEhxControlActionsRequest()
        : base(HCIMessageID.RequestEhxControlActions)
    {
    }

    /// <summary>
    /// Adds an EHX control action.
    /// </summary>
    /// <param name="action">The action to add.</param>
    public void AddAction(EhxControlAction action)
    {
        Actions.Add(action);
    }

    /// <summary>
    /// Adds an EHX control action to enable a GPO.
    /// </summary>
    /// <param name="cardNumber">Card number (0-31).</param>
    /// <param name="pinNumber">Pin number (0-31).</param>
    public void EnableGpo(byte cardNumber, byte pinNumber)
    {
        Actions.Add(EhxControlAction.Enable(cardNumber, pinNumber));
    }

    /// <summary>
    /// Adds an EHX control action to inhibit a GPO.
    /// </summary>
    /// <param name="cardNumber">Card number (0-31).</param>
    /// <param name="pinNumber">Pin number (0-31).</param>
    public void InhibitGpo(byte cardNumber, byte pinNumber)
    {
        Actions.Add(EhxControlAction.Inhibit(cardNumber, pinNumber));
    }

    /// <summary>
    /// Adds an EHX control action to delete/remove a control.
    /// </summary>
    /// <param name="cardNumber">Card number (0-31).</param>
    /// <param name="pinNumber">Pin number (0-31).</param>
    public void DeleteGpo(byte cardNumber, byte pinNumber)
    {
        Actions.Add(EhxControlAction.Delete(cardNumber, pinNumber));
    }

    /// <summary>
    /// Generates the HCIv2 payload for Request EHX Control Actions.
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

        // Action data (10 bytes each: 2 for type + 8 for action words)
        foreach (var action in Actions)
        {
            var actionBytes = action.ToBytes();
            ms.Write(actionBytes, 0, actionBytes.Length);
        }

        return ms.ToArray();
    }
}
