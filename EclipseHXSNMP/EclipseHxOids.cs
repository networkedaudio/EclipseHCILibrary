using Lextm.SharpSnmpLib;

namespace EclipseHXSNMP;

/// <summary>
/// OID constants for the Eclipse HX MIB.
/// Base enterprise OID: 1.3.6.1.4.1.99999 (placeholder — replace with assigned PEN).
/// </summary>
public static class EclipseHxOids
{
    /// <summary>
    /// Enterprise base OID for the Eclipse HX MIB.
    /// </summary>
    public const string Base = "1.3.6.1.4.1.99999.1";

    // -- Cards branch: .1
    public const string Cards = Base + ".1";
    public const string CardCount = Cards + ".1.0";
    public const string CardTable = Cards + ".2";
    public const string CardEntry = CardTable + ".1";

    // Card entry columns (appended with .{column}.{rowIndex})
    public const string CardIndex = CardEntry + ".1";
    public const string CardType = CardEntry + ".2";
    public const string CardCondition = CardEntry + ".3";
    public const string CardIsSlotZero = CardEntry + ".4";
    public const string CardRawStatus = CardEntry + ".5";

    // -- Ports branch: .2
    public const string Ports = Base + ".2";
    public const string PortCount = Ports + ".1.0";
    public const string PortTable = Ports + ".2";
    public const string PortEntry = PortTable + ".1";

    // Port entry columns
    public const string PortIndex = PortEntry + ".1";
    public const string PortNumber = PortEntry + ".2";
    public const string PortPanelType = PortEntry + ".3";
    public const string PortState = PortEntry + ".4";
    public const string PortIsAoip = PortEntry + ".5";

    // -- PSU branch: .3
    public const string Psu = Base + ".3";
    public const string PsuCpuTemperature = Psu + ".1.0";
    public const string PsuExtPsu1Failed = Psu + ".2.0";
    public const string PsuExtPsu2Failed = Psu + ".3.0";
    public const string PsuIntPsu1Failed = Psu + ".4.0";
    public const string PsuIntPsu2Failed = Psu + ".5.0";
    public const string PsuFan1Failed = Psu + ".6.0";
    public const string PsuFan2Failed = Psu + ".7.0";
    public const string PsuConfigFailed = Psu + ".8.0";
    public const string PsuExtAlarmActive = Psu + ".9.0";
    public const string PsuOvertemp = Psu + ".10.0";
    public const string PsuHasAnyAlarm = Psu + ".11.0";

    // -- Transceivers branch: .4
    public const string Transceivers = Base + ".4";
    public const string TransceiverCount = Transceivers + ".1.0";
    public const string TransceiverTable = Transceivers + ".2";
    public const string TransceiverEntry = TransceiverTable + ".1";

    // Transceiver entry columns
    public const string TransceiverIndex = TransceiverEntry + ".1";
    public const string TransceiverSlot = TransceiverEntry + ".2";
    public const string TransceiverPort = TransceiverEntry + ".3";
    public const string TransceiverPanelType = TransceiverEntry + ".4";
    public const string TransceiverIsOnline = TransceiverEntry + ".5";
    public const string TransceiverLabel = TransceiverEntry + ".6";
    public const string TransceiverKeys = TransceiverEntry + ".7";
    public const string TransceiverExpansionPanels = TransceiverEntry + ".8";
    public const string TransceiverFirmware = TransceiverEntry + ".9";

    // -- Beltpacks branch: .5
    public const string Beltpacks = Base + ".5";
    public const string BeltpackCount = Beltpacks + ".1.0";
    public const string BeltpackTable = Beltpacks + ".2";
    public const string BeltpackEntry = BeltpackTable + ".1";

    // Beltpack entry columns
    public const string BeltpackIndex = BeltpackEntry + ".1";
    public const string BeltpackPmid = BeltpackEntry + ".2";
    public const string BeltpackPort = BeltpackEntry + ".3";
    public const string BeltpackPanelType = BeltpackEntry + ".4";
    public const string BeltpackIsOnline = BeltpackEntry + ".5";
    public const string BeltpackLabel = BeltpackEntry + ".6";
    public const string BeltpackAlias = BeltpackEntry + ".7";
    public const string BeltpackKeys = BeltpackEntry + ".8";
    public const string BeltpackFirmware = BeltpackEntry + ".9";
    public const string BeltpackFrequency = BeltpackEntry + ".10";
    public const string BeltpackWirelessMode = BeltpackEntry + ".11";
    public const string BeltpackRoleNumber = BeltpackEntry + ".12";
    public const string BeltpackAntennaPort = BeltpackEntry + ".13";

    /// <summary>
    /// Creates an ObjectIdentifier from a dotted-string OID.
    /// </summary>
    public static ObjectIdentifier ToOid(string oid) => new(oid);
}
