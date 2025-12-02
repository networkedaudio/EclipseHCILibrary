namespace HCILibrary.Models;

/// <summary>
/// Represents a macro key state entry in the Reply Macro Panel Keys Public State message.
/// </summary>
public class MacroPanelKeyStateEntry
{
    /// <summary>
    /// Panel port number.
    /// </summary>
    public ushort PortNumber { get; set; }

    /// <summary>
    /// Panel region of the key.
    /// </summary>
    public byte Region { get; set; }

    /// <summary>
    /// Page number of the key.
    /// </summary>
    public byte Page { get; set; }

    /// <summary>
    /// Key number.
    /// </summary>
    public byte Key { get; set; }

    /// <summary>
    /// System number of the Macro Key group.
    /// </summary>
    public byte MacroKeyGroupSystem { get; set; }

    /// <summary>
    /// Macro group identifier.
    /// </summary>
    public ushort MacroKeyGroupId { get; set; }

    /// <summary>
    /// Pressed state: true if key is on, false if key is off.
    /// </summary>
    public bool PressedState { get; set; }

    /// <summary>
    /// Gets whether the key is currently on (pressed/latched).
    /// </summary>
    public bool IsKeyOn => PressedState;

    /// <summary>
    /// Gets whether the key is currently off.
    /// </summary>
    public bool IsKeyOff => !PressedState;

    /// <summary>
    /// Creates a new MacroPanelKeyStateEntry.
    /// </summary>
    public MacroPanelKeyStateEntry()
    {
    }

    /// <summary>
    /// Creates a new MacroPanelKeyStateEntry with the specified values.
    /// </summary>
    public MacroPanelKeyStateEntry(ushort portNumber, byte region, byte page, byte key,
        byte macroKeyGroupSystem, ushort macroKeyGroupId, bool pressedState)
    {
        PortNumber = portNumber;
        Region = region;
        Page = page;
        Key = key;
        MacroKeyGroupSystem = macroKeyGroupSystem;
        MacroKeyGroupId = macroKeyGroupId;
        PressedState = pressedState;
    }

    public override string ToString()
    {
        return $"Port={PortNumber}, Region={Region}, Page={Page}, Key={Key}, " +
               $"MacroGroup={MacroKeyGroupSystem}:{MacroKeyGroupId}, State={(PressedState ? "On" : "Off")}";
    }
}
