using EclipseHXSNMP.Models;
using Lextm.SharpSnmpLib;

namespace EclipseHXSNMP;

/// <summary>
/// In-memory SNMP object store that exposes Eclipse HX matrix status as MIB variables.
/// Reads from <see cref="EclipseHxMatrixStatus"/> and builds a sorted OID→value map.
/// Supports GET (exact match) and GETNEXT (lexicographic next) operations.
/// </summary>
public class EclipseHxSnmpObjectStore
{
    private readonly EclipseHxMatrixStatus _status;
    private readonly object _lock = new();
    private SortedList<ObjectIdentifier, Variable> _variables = new();

    public EclipseHxSnmpObjectStore(EclipseHxMatrixStatus status)
    {
        _status = status ?? throw new ArgumentNullException(nameof(status));
    }

    /// <summary>
    /// Rebuilds the object store from the current matrix status.
    /// Call this after updating the status model to refresh SNMP values.
    /// </summary>
    public void Refresh()
    {
        var vars = new SortedList<ObjectIdentifier, Variable>();

        var psu = _status.GetPsuSnapshot();
        var cards = _status.GetCardsSnapshot();
        var ports = _status.GetPortsSnapshot();

        // -- Card scalars
        AddVar(vars, EclipseHxOids.CardCount, new Integer32(cards.Count));

        // -- Card table rows
        foreach (var card in cards)
        {
            string row = $".{card.Index}";
            AddVar(vars, EclipseHxOids.CardIndex + row, new Integer32(card.Index));
            AddVar(vars, EclipseHxOids.CardType + row, new OctetString(card.CardType.ToString()));
            AddVar(vars, EclipseHxOids.CardCondition + row, new OctetString(card.Condition.ToString()));
            AddVar(vars, EclipseHxOids.CardIsSlotZero + row, BoolToSnmp(card.IsSlotZero));
            AddVar(vars, EclipseHxOids.CardRawStatus + row, new Integer32(card.RawStatus));
        }

        // -- Port scalars
        AddVar(vars, EclipseHxOids.PortCount, new Integer32(ports.Count));

        // -- Port table rows
        foreach (var port in ports)
        {
            string row = $".{port.Index}";
            AddVar(vars, EclipseHxOids.PortIndex + row, new Integer32(port.Index));
            AddVar(vars, EclipseHxOids.PortNumber + row, new Integer32(port.PortNumber));
            AddVar(vars, EclipseHxOids.PortPanelType + row, new OctetString(port.PanelType.ToString()));
            AddVar(vars, EclipseHxOids.PortState + row, new OctetString(port.State.ToString()));
            AddVar(vars, EclipseHxOids.PortIsAoip + row, BoolToSnmp(port.IsAoipDevice));
        }

        // -- PSU scalars
        AddVar(vars, EclipseHxOids.PsuCpuTemperature, new Integer32(psu.CpuTemperature));
        AddVar(vars, EclipseHxOids.PsuExtPsu1Failed, BoolToSnmp(psu.ExtPsu1Failed));
        AddVar(vars, EclipseHxOids.PsuExtPsu2Failed, BoolToSnmp(psu.ExtPsu2Failed));
        AddVar(vars, EclipseHxOids.PsuIntPsu1Failed, BoolToSnmp(psu.IntPsu1Failed));
        AddVar(vars, EclipseHxOids.PsuIntPsu2Failed, BoolToSnmp(psu.IntPsu2Failed));
        AddVar(vars, EclipseHxOids.PsuFan1Failed, BoolToSnmp(psu.Fan1Failed));
        AddVar(vars, EclipseHxOids.PsuFan2Failed, BoolToSnmp(psu.Fan2Failed));
        AddVar(vars, EclipseHxOids.PsuConfigFailed, BoolToSnmp(psu.ConfigFailed));
        AddVar(vars, EclipseHxOids.PsuExtAlarmActive, BoolToSnmp(psu.ExtAlarmActive));
        AddVar(vars, EclipseHxOids.PsuOvertemp, BoolToSnmp(psu.Overtemp));
        AddVar(vars, EclipseHxOids.PsuHasAnyAlarm, BoolToSnmp(psu.HasAnyAlarm));

        lock (_lock)
        {
            _variables = vars;
        }
    }

    /// <summary>
    /// Gets the variable for an exact OID match (SNMP GET).
    /// </summary>
    /// <param name="oid">The OID to look up.</param>
    /// <returns>The variable, or null if not found.</returns>
    public Variable? GetVariable(ObjectIdentifier oid)
    {
        lock (_lock)
        {
            return _variables.TryGetValue(oid, out var variable) ? variable : null;
        }
    }

    /// <summary>
    /// Gets the next variable after the given OID (SNMP GETNEXT).
    /// </summary>
    /// <param name="oid">The OID to search after.</param>
    /// <returns>The next variable, or null if at end of MIB.</returns>
    public Variable? GetNextVariable(ObjectIdentifier oid)
    {
        lock (_lock)
        {
            foreach (var kvp in _variables)
            {
                if (kvp.Key.CompareTo(oid) > 0)
                {
                    return kvp.Value;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Gets all variables in the store. Used for SNMP WALK.
    /// </summary>
    public IReadOnlyList<Variable> GetAllVariables()
    {
        lock (_lock)
        {
            return _variables.Values.ToList().AsReadOnly();
        }
    }

    private static void AddVar(SortedList<ObjectIdentifier, Variable> vars, string oid, ISnmpData value)
    {
        var id = new ObjectIdentifier(oid);
        vars[id] = new Variable(id, value);
    }

    /// <summary>
    /// Converts a boolean to SNMPv2 TruthValue (1 = true, 2 = false).
    /// </summary>
    private static Integer32 BoolToSnmp(bool value) => new(value ? 1 : 2);
}
