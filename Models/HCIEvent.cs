using HCILibrary.Enums;

namespace HCILibrary.Models;

/// <summary>
/// Represents a decoded HCI Event message (Message ID 0x0001).
/// </summary>
public class HCIEvent
{
    /// <summary>
    /// The event class (Fatal Error, Non-Fatal Error, Warning, Information, Debug, Log to disk).
    /// </summary>
    public HCIEventClass EventClass { get; set; }

    /// <summary>
    /// Unique or component allocated event ID.
    /// </summary>
    public ushort Code { get; set; }

    /// <summary>
    /// Reserved bytes (4 bytes).
    /// </summary>
    public byte[] Reserved { get; set; } = new byte[4];

    /// <summary>
    /// Null terminated event text (max 180 bytes).
    /// </summary>
    public string Text { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"[{EventClass}] Code={Code}: {Text}";
    }
}
