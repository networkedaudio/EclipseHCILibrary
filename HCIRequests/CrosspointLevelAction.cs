using HCILibrary.Helpers;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Represents a single crosspoint level action.
/// </summary>
public class CrosspointLevelAction
{
    /// <summary>
    /// The destination port number (0-1023).
    /// </summary>
    public ushort DestinationPort { get; set; }

    /// <summary>
    /// The source port number (0-1023).
    /// </summary>
    public ushort SourcePort { get; set; }

    /// <summary>
    /// The level value (0-287). Use GainLevel helper for conversions.
    /// </summary>
    public ushort LevelValue { get; set; }

    /// <summary>
    /// Creates a new crosspoint level action.
    /// </summary>
    /// <param name="destinationPort">The destination port number (0-1023).</param>
    /// <param name="sourcePort">The source port number (0-1023).</param>
    /// <param name="levelValue">The level value (0-287).</param>
    public CrosspointLevelAction(ushort destinationPort, ushort sourcePort, ushort levelValue)
    {
        DestinationPort = destinationPort;
        SourcePort = sourcePort;
        LevelValue = levelValue;
    }

    /// <summary>
    /// Gets the gain in dB for this action.
    /// </summary>
    public double GainDb => GainLevel.LevelToDb(LevelValue);

    public override string ToString()
    {
        return $"Src:{SourcePort} -> Dst:{DestinationPort} @ {GainLevel.FormatLevel(LevelValue)}";
    }
}
