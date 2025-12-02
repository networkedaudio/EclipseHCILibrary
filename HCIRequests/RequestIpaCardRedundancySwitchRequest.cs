using HCILibrary.Enums;
using HCILibrary.Models;

namespace HCILibrary.HCIRequests;

/// <summary>
/// Request To Enable/Disable IPA Card Redundancy Switch (HCIv2) - Message ID 0x0182 (386).
/// This message is used to enable or disable IPA card redundancy switch.
/// </summary>
public class RequestIpaCardRedundancySwitchRequest : HCIRequest
{
    /// <summary>
    /// HCIv2 protocol marker.
    /// </summary>
    private static readonly byte[] ProtocolTag = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Value to target all IPA cards.
    /// </summary>
    public const byte AllIpaCards = 0xFF;

    /// <summary>
    /// IPA card slot number. Set to 0xFF for all IPA cards.
    /// </summary>
    public byte IpaCardSlotNumber { get; set; } = AllIpaCards;

    /// <summary>
    /// Switch state. True to disable switching, false to enable switching.
    /// </summary>
    public bool DisableSwitching { get; set; }

    /// <summary>
    /// Creates a new Request IPA Card Redundancy Switch with default values (all cards, enable switching).
    /// </summary>
    public RequestIpaCardRedundancySwitchRequest()
        : base(HCIMessageID.RequestIpaCardRedundancySwitch)
    {
    }

    /// <summary>
    /// Creates a new Request IPA Card Redundancy Switch for all IPA cards.
    /// </summary>
    /// <param name="disableSwitching">True to disable switching, false to enable switching.</param>
    public RequestIpaCardRedundancySwitchRequest(bool disableSwitching)
        : base(HCIMessageID.RequestIpaCardRedundancySwitch)
    {
        IpaCardSlotNumber = AllIpaCards;
        DisableSwitching = disableSwitching;
    }

    /// <summary>
    /// Creates a new Request IPA Card Redundancy Switch for a specific IPA card.
    /// </summary>
    /// <param name="ipaCardSlotNumber">The IPA card slot number, or 0xFF for all cards.</param>
    /// <param name="disableSwitching">True to disable switching, false to enable switching.</param>
    public RequestIpaCardRedundancySwitchRequest(byte ipaCardSlotNumber, bool disableSwitching)
        : base(HCIMessageID.RequestIpaCardRedundancySwitch)
    {
        IpaCardSlotNumber = ipaCardSlotNumber;
        DisableSwitching = disableSwitching;
    }

    /// <summary>
    /// Generates the payload for the request.
    /// </summary>
    /// <returns>The payload bytes.</returns>
    protected override byte[] GeneratePayload()
    {
        // Payload structure:
        // Protocol Tag: 4 bytes (0xABBACEDE)
        // Protocol Schema: 1 byte (set to 1)
        // IPA Card Slot Number: 1 byte (0xFF for all IPA cards)
        // Switch State: 1 byte (1 for disable, 0 for enable)

        using var ms = new MemoryStream();

        // Protocol Tag: 4 bytes
        ms.Write(ProtocolTag, 0, ProtocolTag.Length);

        // Protocol Schema: 1 byte
        ms.WriteByte(0x01);

        // IPA Card Slot Number: 1 byte
        ms.WriteByte(IpaCardSlotNumber);

        // Switch State: 1 byte (1 = disable, 0 = enable)
        ms.WriteByte((byte)(DisableSwitching ? 1 : 0));

        return ms.ToArray();
    }
}
