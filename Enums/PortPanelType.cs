namespace HCILibrary.Enums;

/// <summary>
/// Panel type enumeration for port info.
/// </summary>
public enum PortPanelType : ushort
{
    /// <summary>
    /// Unknown panel type.
    /// </summary>
    Unknown = 0x0000,

    /// <summary>
    /// iStation panel.
    /// </summary>
    IStation = 0x8000,

    /// <summary>
    /// ICS 1016 panel.
    /// </summary>
    Ics1016 = 0x8008,

    /// <summary>
    /// ICS 1008 panel.
    /// </summary>
    Ics1008 = 0x800E,

    /// <summary>
    /// V-Series 1U Lever panel.
    /// </summary>
    VSeries1ULever = 0x8010,

    /// <summary>
    /// V-Series 1U Push panel.
    /// </summary>
    VSeries1UPush = 0x8011,

    /// <summary>
    /// V-Series 2U Lever panel.
    /// </summary>
    VSeries2ULever = 0x8012,

    /// <summary>
    /// V-Series 2U Push panel.
    /// </summary>
    VSeries2UPush = 0x8013,

    /// <summary>
    /// V-Series Desk Lever panel.
    /// </summary>
    VSeriesDeskLever = 0x8014,

    /// <summary>
    /// V-Series Desk Push panel.
    /// </summary>
    VSeriesDeskPush = 0x8015,

    /// <summary>
    /// V-Series 1U Rotary panel.
    /// </summary>
    VSeries1URotary = 0x8016,

    /// <summary>
    /// V-Series 2U Rotary panel.
    /// </summary>
    VSeries2URotary = 0x8019,

    /// <summary>
    /// V-Series Desk Rotary panel.
    /// </summary>
    VSeriesDeskRotary = 0x801A,

    /// <summary>
    /// V-Series 2U (32 Key) panel.
    /// </summary>
    VSeries2U32Key = 0x8020,

    /// <summary>
    /// CCI-22 panel.
    /// </summary>
    Cci22 = 0x8100,

    /// <summary>
    /// FOR-22 panel.
    /// </summary>
    For22 = 0x8102,

    /// <summary>
    /// Tel-14/SIP panel.
    /// </summary>
    Tel14Sip = 0x8106,

    /// <summary>
    /// LQ panel.
    /// </summary>
    Lq = 0x8110,

    /// <summary>
    /// Edge Beltpacks.
    /// </summary>
    EdgeBeltpacks = 0x8204
}
