namespace HCILibrary.Enums;

/// <summary>
/// Represents the type of telephony client port.
/// </summary>
public enum TelephonyClientType : byte
{
    /// <summary>
    /// Unknown or unspecified telephony client type.
    /// </summary>
    Unknown = 0x00,

    /// <summary>
    /// TEL-14 telephone hybrid interface.
    /// </summary>
    Tel14 = 0x01,

    /// <summary>
    /// SIP (Session Initiation Protocol) client.
    /// </summary>
    Sip = 0x02,

    /// <summary>
    /// LQ-SIP telephone interface.
    /// </summary>
    LqSip = 0x03
}
