namespace HCILibrary.Models;

/// <summary>
/// Represents an IPA card switch state entry in the Reply IPA Card Redundancy Switch message.
/// </summary>
public class IpaCardSwitchStateEntry
{
    /// <summary>
    /// Slot number of the IPA card.
    /// </summary>
    public byte SlotNumber { get; set; }

    /// <summary>
    /// Switch state: true if switching is disabled, false if switching is enabled.
    /// </summary>
    public bool SwitchDisabled { get; set; }

    /// <summary>
    /// Gets whether switching is enabled for this card.
    /// </summary>
    public bool SwitchEnabled => !SwitchDisabled;

    /// <summary>
    /// Creates a new IpaCardSwitchStateEntry.
    /// </summary>
    public IpaCardSwitchStateEntry()
    {
    }

    /// <summary>
    /// Creates a new IpaCardSwitchStateEntry with the specified values.
    /// </summary>
    /// <param name="slotNumber">Slot number of the IPA card.</param>
    /// <param name="switchDisabled">True if switching is disabled, false if enabled.</param>
    public IpaCardSwitchStateEntry(byte slotNumber, bool switchDisabled)
    {
        SlotNumber = slotNumber;
        SwitchDisabled = switchDisabled;
    }

    public override string ToString()
    {
        return $"Slot={SlotNumber}, SwitchState={(SwitchDisabled ? "Disabled" : "Enabled")}";
    }
}
