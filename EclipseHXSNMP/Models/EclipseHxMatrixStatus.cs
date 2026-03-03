namespace EclipseHXSNMP.Models;

/// <summary>
/// Holds the current state of an Eclipse HX matrix for SNMP exposure.
/// This is the root data model that the SNMP agent reads from.
/// </summary>
public class EclipseHxMatrixStatus
{
    private readonly object _lock = new();

    /// <summary>
    /// The current card status list.
    /// </summary>
    public List<SnmpCardEntry> Cards { get; private set; } = new();

    /// <summary>
    /// The current port/panel status list.
    /// </summary>
    public List<SnmpPortEntry> Ports { get; private set; } = new();

    /// <summary>
    /// The current PSU and frame status.
    /// </summary>
    public SnmpPsuStatus PsuStatus { get; private set; } = new();

    /// <summary>
    /// Timestamp of the last card status update.
    /// </summary>
    public DateTime? LastCardUpdate { get; private set; }

    /// <summary>
    /// Timestamp of the last port status update.
    /// </summary>
    public DateTime? LastPortUpdate { get; private set; }

    /// <summary>
    /// Timestamp of the last PSU status update.
    /// </summary>
    public DateTime? LastPsuUpdate { get; private set; }

    /// <summary>
    /// Updates the card status list from HCI reply data.
    /// Thread-safe.
    /// </summary>
    /// <param name="cards">The new card entries.</param>
    public void UpdateCards(IEnumerable<SnmpCardEntry> cards)
    {
        lock (_lock)
        {
            Cards = new List<SnmpCardEntry>(cards);
            LastCardUpdate = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Updates the port/panel status list from HCI reply data.
    /// Thread-safe.
    /// </summary>
    /// <param name="ports">The new port entries.</param>
    public void UpdatePorts(IEnumerable<SnmpPortEntry> ports)
    {
        lock (_lock)
        {
            Ports = new List<SnmpPortEntry>(ports);
            LastPortUpdate = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Updates the PSU status from HCI reply data.
    /// Thread-safe.
    /// </summary>
    /// <param name="psuStatus">The new PSU status.</param>
    public void UpdatePsuStatus(SnmpPsuStatus psuStatus)
    {
        lock (_lock)
        {
            PsuStatus = psuStatus;
            LastPsuUpdate = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Gets a snapshot of the current card list.
    /// Thread-safe.
    /// </summary>
    public List<SnmpCardEntry> GetCardsSnapshot()
    {
        lock (_lock)
        {
            return new List<SnmpCardEntry>(Cards);
        }
    }

    /// <summary>
    /// Gets a snapshot of the current port list.
    /// Thread-safe.
    /// </summary>
    public List<SnmpPortEntry> GetPortsSnapshot()
    {
        lock (_lock)
        {
            return new List<SnmpPortEntry>(Ports);
        }
    }

    /// <summary>
    /// Gets a snapshot of the current PSU status.
    /// Thread-safe.
    /// </summary>
    public SnmpPsuStatus GetPsuSnapshot()
    {
        lock (_lock)
        {
            return PsuStatus;
        }
    }
}
