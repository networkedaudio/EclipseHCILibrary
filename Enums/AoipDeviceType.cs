namespace HCILibrary.Enums;

/// <summary>
/// Device type reported within AoIP Device Status sub-messages.
/// </summary>
public enum AoipDeviceType : byte
{
    /// <summary>Unknown or unrecognised device.</summary>
    Unknown = 0,

    /// <summary>E-IPA card.</summary>
    Eipa = 1,

    /// <summary>IVC-32 card.</summary>
    Ivc32 = 2,

    /// <summary>LQ device.</summary>
    Lq = 3,

    /// <summary>FreeSpeak Edge transceiver.</summary>
    FsEdge = 4,

    /// <summary>Agent-IC endpoint.</summary>
    AgentIc = 5,

    /// <summary>Station-IC endpoint.</summary>
    StationIc = 6,

    /// <summary>V-Series IP panel.</summary>
    VSeries = 7,
}
