using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request Telephony Key Status Enable (HCIv2) - Message ID 0x015A (346).
/// This message enables/disables the transmission of key status messages
/// for telephony related keys.
/// </summary>
public class RequestTelephonyKeyStatusEnableRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Whether telephony key status messages are enabled.
    /// When true, the matrix will send unsolicited Reply Telephony Key Status messages
    /// when telephony key state transitions occur.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Creates a new Request Telephony Key Status Enable with status disabled.
    /// </summary>
    public RequestTelephonyKeyStatusEnableRequest()
        : base(HCIMessageID.RequestTelephonyKeyStatusEnable)
    {
    }

    /// <summary>
    /// Creates a new Request Telephony Key Status Enable with the specified enabled state.
    /// </summary>
    /// <param name="enabled">True to enable telephony key status messages, false to disable.</param>
    public RequestTelephonyKeyStatusEnableRequest(bool enabled)
        : base(HCIMessageID.RequestTelephonyKeyStatusEnable)
    {
        Enabled = enabled;
    }

    /// <summary>
    /// Creates a request to enable telephony key status messages.
    /// </summary>
    /// <returns>A request configured to enable telephony key status.</returns>
    public static RequestTelephonyKeyStatusEnableRequest Enable()
    {
        return new RequestTelephonyKeyStatusEnableRequest(true);
    }

    /// <summary>
    /// Creates a request to disable telephony key status messages.
    /// </summary>
    /// <returns>A request configured to disable telephony key status.</returns>
    public static RequestTelephonyKeyStatusEnableRequest Disable()
    {
        return new RequestTelephonyKeyStatusEnableRequest(false);
    }

    /// <summary>
    /// Generates the payload for the request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // Protocol Tag: 4 bytes (0xABBACEDE)
        // Protocol Schema: 1 byte (set to 1)
        // Enabled: 1 byte (1 = enabled, 0 = disabled)

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // Enabled: 1 byte
        ms.WriteByte((byte)(Enabled ? 1 : 0));

        return ms.ToArray();
    }
}
