namespace HCILibrary.Models;

/// <summary>
/// Represents a crosspoint and level entry in the Reply Xpt and Level Status message.
/// </summary>
public class XptAndLevelEntry
{
    /// <summary>
    /// If set, this is the header port entry for subsequent entries that are in relation to this port.
    /// </summary>
    public bool Monitored { get; set; }

    /// <summary>
    /// If set, the monitor port is the crosspoint source.
    /// </summary>
    public bool TalkXpt { get; set; }

    /// <summary>
    /// If set, the monitor port is the crosspoint destination.
    /// </summary>
    public bool ListenXpt { get; set; }

    /// <summary>
    /// Port number (10 bits).
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    /// Crosspoint level.
    /// </summary>
    public ushort Level { get; set; }

    /// <summary>
    /// Creates a new XptAndLevelEntry.
    /// </summary>
    public XptAndLevelEntry()
    {
    }

    /// <summary>
    /// Creates a new XptAndLevelEntry with the specified values.
    /// </summary>
    /// <param name="monitored">If set, this is the header port entry.</param>
    /// <param name="talkXpt">If set, monitor port is the xpt source.</param>
    /// <param name="listenXpt">If set, monitor port is the xpt destination.</param>
    /// <param name="port">Port number.</param>
    /// <param name="level">Crosspoint level.</param>
    public XptAndLevelEntry(bool monitored, bool talkXpt, bool listenXpt, ushort port, ushort level)
    {
        Monitored = monitored;
        TalkXpt = talkXpt;
        ListenXpt = listenXpt;
        Port = port;
        Level = level;
    }

    public override string ToString()
    {
        return $"Port={Port}, Level={Level}, Monitored={Monitored}, TalkXpt={TalkXpt}, ListenXpt={ListenXpt}";
    }
}
