using HCILibrary.Enums;

namespace HCILibrary.Models;

/// <summary>
/// Represents a card's network redundancy status entry.
/// </summary>
public class NetworkRedundancyCardEntry
{
    /// <summary>
    /// Slot number of the card.
    /// </summary>
    public byte PhysicalSlot { get; set; }

    /// <summary>
    /// IVC (Intercom Voice Channel) redundancy state.
    /// </summary>
    public CardRedundancyState IvcState { get; set; }

    /// <summary>
    /// Number of times the card has switched active IVC LAN port since boot up.
    /// </summary>
    public ushort IvcSwitchoverCount { get; set; }

    /// <summary>
    /// AoIP (Audio over IP) redundancy state.
    /// </summary>
    public CardRedundancyState AoipState { get; set; }

    /// <summary>
    /// Number of times the card has switched active AoIP LAN port since boot up.
    /// </summary>
    public ushort AoipSwitchoverCount { get; set; }

    /// <summary>
    /// Gets whether IVC is in Main state.
    /// </summary>
    public bool IsIvcMain => IvcState == CardRedundancyState.Main;

    /// <summary>
    /// Gets whether IVC is in Standby state.
    /// </summary>
    public bool IsIvcStandby => IvcState == CardRedundancyState.Standby;

    /// <summary>
    /// Gets whether AoIP is in Main state.
    /// </summary>
    public bool IsAoipMain => AoipState == CardRedundancyState.Main;

    /// <summary>
    /// Gets whether AoIP is in Standby state.
    /// </summary>
    public bool IsAoipStandby => AoipState == CardRedundancyState.Standby;

    /// <summary>
    /// Creates a new NetworkRedundancyCardEntry.
    /// </summary>
    public NetworkRedundancyCardEntry()
    {
    }

    /// <summary>
    /// Creates a new NetworkRedundancyCardEntry with specified values.
    /// </summary>
    /// <param name="physicalSlot">The slot number.</param>
    /// <param name="ivcState">The IVC state.</param>
    /// <param name="ivcSwitchoverCount">The IVC switchover count.</param>
    /// <param name="aoipState">The AoIP state.</param>
    /// <param name="aoipSwitchoverCount">The AoIP switchover count.</param>
    public NetworkRedundancyCardEntry(byte physicalSlot, CardRedundancyState ivcState, ushort ivcSwitchoverCount,
        CardRedundancyState aoipState, ushort aoipSwitchoverCount)
    {
        PhysicalSlot = physicalSlot;
        IvcState = ivcState;
        IvcSwitchoverCount = ivcSwitchoverCount;
        AoipState = aoipState;
        AoipSwitchoverCount = aoipSwitchoverCount;
    }
}
