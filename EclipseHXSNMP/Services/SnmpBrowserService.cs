using System.Net;
using EclipseHXSNMP.Models;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace EclipseHXSNMP.Services;

/// <summary>
/// Service for browsing SNMP agents on remote matrices.
/// Performs SNMP WALK operations and builds a MIB tree for display.
/// </summary>
public class SnmpBrowserService
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Walks the MIB tree on a remote matrix and returns a hierarchical tree.
    /// </summary>
    /// <param name="connection">The matrix connection to query.</param>
    /// <param name="baseOid">The base OID to walk from (default: Eclipse HX base).</param>
    /// <returns>The root node of the MIB tree.</returns>
    public async Task<MibTreeNode> WalkAsync(MatrixConnection connection, string? baseOid = null)
    {
        var oid = baseOid ?? EclipseHxOids.Base;
        var variables = await Task.Run(() => PerformWalk(connection, oid));
        return BuildTree(variables, oid);
    }

    /// <summary>
    /// Gets a single OID value from a remote matrix.
    /// </summary>
    public async Task<string?> GetValueAsync(MatrixConnection connection, string oid)
    {
        try
        {
            var result = await Task.Run(() =>
            {
                var endpoint = new IPEndPoint(IPAddress.Parse(connection.IpAddress), connection.Port);
                var variables = Messenger.Get(
                    VersionCode.V2,
                    endpoint,
                    new OctetString(connection.Community),
                    new List<Variable> { new(new ObjectIdentifier(oid)) },
                    (int)DefaultTimeout.TotalMilliseconds);
                return variables;
            });

            return result.Count > 0 ? result[0].Data.ToString() : null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Tests connectivity to a remote matrix by attempting an SNMP GET on sysDescr.
    /// </summary>
    public async Task<bool> TestConnectionAsync(MatrixConnection connection)
    {
        try
        {
            // Try to GET sysDescr.0 (1.3.6.1.2.1.1.1.0)
            var result = await GetValueAsync(connection, "1.3.6.1.2.1.1.1.0");
            return result != null;
        }
        catch
        {
            return false;
        }
    }

    private List<Variable> PerformWalk(MatrixConnection connection, string baseOid)
    {
        var results = new List<Variable>();

        try
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(connection.IpAddress), connection.Port);
            Messenger.Walk(
                VersionCode.V2,
                endpoint,
                new OctetString(connection.Community),
                new ObjectIdentifier(baseOid),
                results,
                (int)DefaultTimeout.TotalMilliseconds,
                WalkMode.WithinSubtree);
        }
        catch
        {
            // Return whatever we collected before the error
        }

        return results;
    }

    private static MibTreeNode BuildTree(List<Variable> variables, string baseOid)
    {
        var root = new MibTreeNode
        {
            Name = GetOidName(baseOid),
            Oid = baseOid,
            IsExpanded = true
        };

        foreach (var variable in variables)
        {
            var oidStr = variable.Id.ToString();
            var value = FormatValue(variable.Data);
            InsertIntoTree(root, oidStr, value);
        }

        return root;
    }

    private static void InsertIntoTree(MibTreeNode root, string oid, string value)
    {
        // Strip the base OID prefix to get the relative path
        var basePrefix = root.Oid + ".";
        if (!oid.StartsWith(basePrefix))
        {
            // Direct child or doesn't match — add as leaf
            root.Children.Add(new MibTreeNode
            {
                Name = GetOidName(oid),
                Oid = oid,
                Value = value
            });
            return;
        }

        var relativeParts = oid[basePrefix.Length..].Split('.');
        var current = root;

        // Navigate/create intermediate branch nodes
        var runningOid = root.Oid;
        for (int i = 0; i < relativeParts.Length - 1; i++)
        {
            runningOid += "." + relativeParts[i];
            var child = current.Children.Find(c => c.Oid == runningOid);
            if (child == null)
            {
                child = new MibTreeNode
                {
                    Name = GetOidName(runningOid),
                    Oid = runningOid
                };
                current.Children.Add(child);
            }
            current = child;
        }

        // Add the leaf
        runningOid += "." + relativeParts[^1];
        var leaf = current.Children.Find(c => c.Oid == runningOid);
        if (leaf != null)
        {
            leaf.Value = value;
        }
        else
        {
            current.Children.Add(new MibTreeNode
            {
                Name = GetOidName(runningOid),
                Oid = runningOid,
                Value = value
            });
        }
    }

    /// <summary>
    /// Maps known OIDs to human-readable names from the MIB.
    /// </summary>
    private static string GetOidName(string oid)
    {
        return oid switch
        {
            EclipseHxOids.Base => "eclipseHx",
            EclipseHxOids.Cards => "eclipseCards",
            EclipseHxOids.CardCount => "cardCount",
            EclipseHxOids.CardTable => "cardTable",
            EclipseHxOids.CardEntry => "cardEntry",
            EclipseHxOids.Ports => "eclipsePorts",
            EclipseHxOids.PortCount => "portCount",
            EclipseHxOids.PortTable => "portTable",
            EclipseHxOids.PortEntry => "portEntry",
            EclipseHxOids.Psu => "eclipsePsu",
            EclipseHxOids.PsuCpuTemperature => "psuCpuTemperature",
            EclipseHxOids.PsuExtPsu1Failed => "psuExtPsu1Failed",
            EclipseHxOids.PsuExtPsu2Failed => "psuExtPsu2Failed",
            EclipseHxOids.PsuIntPsu1Failed => "psuIntPsu1Failed",
            EclipseHxOids.PsuIntPsu2Failed => "psuIntPsu2Failed",
            EclipseHxOids.PsuFan1Failed => "psuFan1Failed",
            EclipseHxOids.PsuFan2Failed => "psuFan2Failed",
            EclipseHxOids.PsuConfigFailed => "psuConfigFailed",
            EclipseHxOids.PsuExtAlarmActive => "psuExtAlarmActive",
            EclipseHxOids.PsuOvertemp => "psuOvertemp",
            EclipseHxOids.PsuHasAnyAlarm => "psuHasAnyAlarm",
            _ => oid[(oid.LastIndexOf('.') + 1)..]
        };
    }

    private static string FormatValue(ISnmpData data)
    {
        return data switch
        {
            Integer32 i => i.ToInt32().ToString(),
            OctetString s => s.ToString(),
            ObjectIdentifier o => o.ToString(),
            Null => "(null)",
            NoSuchInstance => "(no such instance)",
            NoSuchObject => "(no such object)",
            EndOfMibView => "(end of MIB)",
            _ => data.ToString() ?? "(unknown)"
        };
    }
}
