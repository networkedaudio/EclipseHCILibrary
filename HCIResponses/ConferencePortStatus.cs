namespace HCILibrary.HCIResponses;

/// <summary>
/// Represents a port's connection status within a conference.
/// </summary>
public class ConferencePortStatus
{
    /// <summary>
    /// The port number (0-1023).
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Whether this port is a listener in the conference.
    /// </summary>
    public bool IsListener { get; set; }

    /// <summary>
    /// Whether this port is a talker in the conference.
    /// </summary>
    public bool IsTalker { get; set; }

    public override string ToString()
    {
        var role = (IsListener, IsTalker) switch
        {
            (true, true) => "Listener+Talker",
            (true, false) => "Listener",
            (false, true) => "Talker",
            _ => "None"
        };
        return $"Port {PortNumber}: {role}";
    }
}
