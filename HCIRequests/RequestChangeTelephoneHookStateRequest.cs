using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Change Telephone Hook State (0x010D).
/// Toggles the on/off (line release) hook of a Clear-Com telephone hybrid (TEL-14, LQ-SIP).
/// This includes killing all talks to the telephone hybrid and putting the telephone hybrid
/// either on hook or off hook.
/// HCIv2 only.
/// </summary>
public class RequestChangeTelephoneHookStateRequest : HCIRequest
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
    /// Gets or sets the telephone port number (0-495).
    /// </summary>
    public ushort TelephonePortNumber { get; set; }

    /// <summary>
    /// Gets or sets the hook state (OnHook or OffHook).
    /// </summary>
    public HookState HookState { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestChangeTelephoneHookStateRequest"/> class.
    /// </summary>
    public RequestChangeTelephoneHookStateRequest()
        : base(HCIMessageID.RequestChangeTelephoneHookState)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestChangeTelephoneHookStateRequest"/> class
    /// with the specified telephone port and hook state.
    /// </summary>
    /// <param name="telephonePortNumber">The telephone port number (0-495).</param>
    /// <param name="hookState">The desired hook state.</param>
    public RequestChangeTelephoneHookStateRequest(ushort telephonePortNumber, HookState hookState)
        : base(HCIMessageID.RequestChangeTelephoneHookState)
    {
        TelephonePortNumber = telephonePortNumber;
        HookState = hookState;
    }

    /// <summary>
    /// Generates the payload for the Request Change Telephone Hook State message.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // Protocol Tag: 4 bytes (0xABBACEDE)
        // Protocol Schema: 1 byte
        // Telephone Port Number: 2 bytes (big-endian)
        // Hook State: 1 byte

        var payload = new byte[8];
        int offset = 0;

        // Protocol Tag (4 bytes)
        Array.Copy(ProtocolTag, 0, payload, offset, 4);
        offset += 4;

        // Protocol Schema (1 byte)
        payload[offset++] = ProtocolSchema;

        // Telephone Port Number (2 bytes, big-endian)
        payload[offset++] = (byte)((TelephonePortNumber >> 8) & 0xFF);
        payload[offset++] = (byte)(TelephonePortNumber & 0xFF);

        // Hook State (1 byte)
        payload[offset++] = (byte)HookState;

        return payload;
    }
}
