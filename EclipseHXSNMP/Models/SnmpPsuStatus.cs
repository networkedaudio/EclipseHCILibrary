namespace EclipseHXSNMP.Models;

/// <summary>
/// Represents the PSU and frame status for SNMP exposure.
/// </summary>
public class SnmpPsuStatus
{
    /// <summary>
    /// CPU card temperature in degrees Celsius.
    /// </summary>
    public short CpuTemperature { get; set; }

    /// <summary>
    /// External PSU 1 status: true = failed.
    /// </summary>
    public bool ExtPsu1Failed { get; set; }

    /// <summary>
    /// External PSU 2 status: true = failed.
    /// </summary>
    public bool ExtPsu2Failed { get; set; }

    /// <summary>
    /// Internal PSU 1 status: true = failed.
    /// </summary>
    public bool IntPsu1Failed { get; set; }

    /// <summary>
    /// Internal PSU 2 status: true = failed.
    /// </summary>
    public bool IntPsu2Failed { get; set; }

    /// <summary>
    /// Fan 1 status: true = failed.
    /// </summary>
    public bool Fan1Failed { get; set; }

    /// <summary>
    /// Fan 2 status: true = failed.
    /// </summary>
    public bool Fan2Failed { get; set; }

    /// <summary>
    /// Configuration failure: true = failed.
    /// </summary>
    public bool ConfigFailed { get; set; }

    /// <summary>
    /// External alarm: true = active.
    /// </summary>
    public bool ExtAlarmActive { get; set; }

    /// <summary>
    /// Over-temperature condition: true = alarm.
    /// </summary>
    public bool Overtemp { get; set; }

    /// <summary>
    /// Whether any alarm condition is active.
    /// </summary>
    public bool HasAnyAlarm =>
        ExtPsu1Failed || ExtPsu2Failed ||
        IntPsu1Failed || IntPsu2Failed ||
        Fan1Failed || Fan2Failed ||
        ConfigFailed || ExtAlarmActive || Overtemp;
}
