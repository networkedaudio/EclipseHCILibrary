using HCILibrary.Enums;
using HCILibrary.HCIResponses;

namespace HCILibrary.Models;

/// <summary>
/// Represents a decoded HCI reply message.
/// </summary>
public class HCIReply
{
    /// <summary>
    /// The total message length as specified in the first two bytes after the start marker.
    /// </summary>
    public ushort MessageLength { get; set; }

    /// <summary>
    /// The message ID identifying the type of message.
    /// </summary>
    public HCIMessageID MessageID { get; set; }

    /// <summary>
    /// The flags byte parsed into individual flag values.
    /// </summary>
    public HCIFlags Flags { get; set; } = new HCIFlags(0);

    /// <summary>
    /// The HCI protocol version (v1 or v2).
    /// </summary>
    public HCIVersion Version { get; set; }

    /// <summary>
    /// Additional protocol indicator for HCIv2 messages.
    /// 0 if this is an HCIv1 message, otherwise the value from the packet.
    /// </summary>
    public byte AdditionalProtocolIndicator { get; set; }

    /// <summary>
    /// Protocol schema version for HCIv2 messages.
    /// Used by some messages to determine payload structure.
    /// </summary>
    public byte Schema { get; set; } = 1;

    /// <summary>
    /// The payload data that varies based on message ID.
    /// </summary>
    public byte[] Payload { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// The raw message bytes for reference.
    /// </summary>
    public byte[] RawMessage { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Parsed event data for Event messages (Message ID 0x0001).
    /// Null if this is not an Event message.
    /// </summary>
    public HCIEvent? Event { get; set; }

    /// <summary>
    /// Parsed conference status for Reply Conference Status messages (Message ID 0x0014).
    /// Null if this is not a Reply Conference Status message.
    /// </summary>
    public ReplyConferenceStatus? ConferenceStatus { get; set; }

    /// <summary>
    /// Parsed crosspoint status for Reply Crosspoint Status messages (Message ID 0x000E).
    /// Null if this is not a Reply Crosspoint Status message.
    /// </summary>
    public ReplyCrosspointStatus? CrosspointStatus { get; set; }

    /// <summary>
    /// Parsed crosspoint level status for Reply Crosspoint Level Status messages (Message ID 0x0028).
    /// Null if this is not a Reply Crosspoint Level Status message.
    /// </summary>
    public ReplyCrosspointLevelStatus? CrosspointLevelStatus { get; set; }

    /// <summary>
    /// Parsed unicode alias status for Reply Unicode Alias Status messages (Message ID 0x00F5).
    /// Null if this is not a Reply Unicode Alias Status message.
    /// </summary>
    public ReplyUnicodeAliasStatus? UnicodeAliasStatus { get; set; }

    /// <summary>
    /// Parsed alias delete status for Reply Alias Delete messages (Message ID 0x0085).
    /// Null if this is not a Reply Alias Delete message.
    /// </summary>
    public ReplyAliasDelete? AliasDeleteStatus { get; set; }

    /// <summary>
    /// Parsed EHX control card status for Reply EHX Control Card Status messages (Message ID 0x0016).
    /// Null if this is not a Reply EHX Control Card Status message.
    /// </summary>
    public ReplyEhxControlCardStatus? EhxControlCardStatus { get; set; }

    /// <summary>
    /// Parsed GPIO/SFO status for Reply GPIO/SFO Status messages (Message ID 0x0018).
    /// Null if this is not a Reply GPIO/SFO Status message.
    /// </summary>
    public ReplyGpioSfoStatus? GpioSfoStatus { get; set; }

    /// <summary>
    /// Parsed input level status for Reply Input Level Status messages (Message ID 0x0022).
    /// Null if this is not a Reply Input Level Status message.
    /// </summary>
    public ReplyInputLevelStatus? InputLevelStatus { get; set; }

    /// <summary>
    /// Parsed output level status for Reply Output Level Status messages (Message ID 0x0025).
    /// Null if this is not a Reply Output Level Status message.
    /// </summary>
    public ReplyOutputLevelStatus? OutputLevelStatus { get; set; }

    /// <summary>
    /// Parsed panel status for Reply Panel Status messages (Message ID 0x001E).
    /// Null if this is not a Reply Panel Status message.
    /// </summary>
    public ReplyPanelStatus? PanelStatus { get; set; }

    /// <summary>
    /// Parsed system card status for Reply System Card Status messages (Message ID 0x0004).
    /// Null if this is not a Reply System Card Status message.
    /// </summary>
    public ReplySystemCardStatus? SystemCardStatus { get; set; }

    /// <summary>
    /// Parsed panel keys status for Reply Panel Keys Status messages (Message ID 0x00B2).
    /// Null if this is not a Reply Panel Keys Status message.
    /// </summary>
    public ReplyPanelKeysStatus? PanelKeysStatus { get; set; }

    /// <summary>
    /// Parsed panel keys action status for Reply Panel Keys Action Status messages (Message ID 0x00B4).
    /// Null if this is not a Reply Panel Keys Action Status message.
    /// </summary>
    public ReplyPanelKeysActionStatus? PanelKeysActionStatus { get; set; }

    /// <summary>
    /// Parsed port info for Reply Port Info messages (Message ID 0x00B8).
    /// Null if this is not a Reply Port Info message.
    /// </summary>
    public ReplyPortInfo? PortInfo { get; set; }

    /// <summary>
    /// Parsed locally assigned keys for Reply Locally Assigned Keys messages (Message ID 0x00BA).
    /// Null if this is not a Reply Locally Assigned Keys message.
    /// </summary>
    public ReplyLocallyAssignedKeys? LocallyAssignedKeys { get; set; }

    /// <summary>
    /// Parsed assigned keys for Reply Assigned Keys messages (Message ID 0x00E8).
    /// Null if this is not a Reply Assigned Keys message.
    /// </summary>
    public ReplyAssignedKeys? AssignedKeys { get; set; }

    /// <summary>
    /// Parsed card info for Reply Card Info messages (Message ID 0x00C4).
    /// Null if this is not a Reply Card Info message.
    /// </summary>
    public ReplyCardInfo? CardInfo { get; set; }

    /// <summary>
    /// Parsed conference assignments for Reply Conference Assignments messages (Message ID 0x00C6).
    /// Null if this is not a Reply Conference Assignments message.
    /// </summary>
    public ReplyConferenceAssignments? ConferenceAssignments { get; set; }

    /// <summary>
    /// Parsed set config multiple keys reply for Reply Set Config Multiple Keys messages (Message ID 0x00CE).
    /// Null if this is not a Reply Set Config Multiple Keys message.
    /// </summary>
    public ReplySetConfigMultipleKeys? SetConfigMultipleKeys { get; set; }

    /// <summary>
    /// Parsed forced listen edits for Reply Forced Listen Edits messages (Message ID 0x00CA).
    /// Null if this is not a Reply Forced Listen Edits message.
    /// </summary>
    public ReplyForcedListenEdits? ForcedListenEdits { get; set; }

    /// <summary>
    /// Parsed remote key actions for Reply Remote Key Actions messages (Message ID 0x00EC).
    /// Null if this is not a Reply Remote Key Actions message.
    /// </summary>
    public ReplyRemoteKeyActions? RemoteKeyActions { get; set; }

    /// <summary>
    /// Parsed remote key action status for Reply Remote Key Action Status messages (Message ID 0x00EE).
    /// Null if this is not a Reply Remote Key Action Status message.
    /// </summary>
    public ReplyRemoteKeyActionStatus? RemoteKeyActionStatus { get; set; }

    /// <summary>
    /// Parsed VOX threshold status for Reply VOX Threshold Status messages (Message ID 0x004A).
    /// Null if this is not a Reply VOX Threshold Status message.
    /// </summary>
    public ReplyVoxThresholdStatus? VoxThresholdStatus { get; set; }

    /// <summary>
    /// Parsed beltpack status for Reply Beltpack Status messages (Message ID 0x004C).
    /// Null if this is not a Reply Beltpack Status message.
    /// </summary>
    public ReplyBeltpackStatus? BeltpackStatus { get; set; }

    /// <summary>
    /// Parsed rack properties config bank for Reply Rack Properties messages (Message ID 0x002C).
    /// Null if this is not a Reply Rack Properties: Config Bank message.
    /// </summary>
    public ReplyRackPropertiesConfigBank? RackPropertiesConfigBank { get; set; }

    /// <summary>
    /// Parsed rack properties rack state for Reply Rack Properties messages (Message ID 0x002C).
    /// Null if this is not a Reply Rack Properties: Rack State Get message.
    /// </summary>
    public ReplyRackPropertiesRackState? RackPropertiesRackState { get; set; }

    /// <summary>
    /// Parsed rack configuration status for Reply Rack Configuration Status messages (Message ID 0x002C, Sub ID 13).
    /// Contains configuration status from the matrix including download timestamps, licenses, etc.
    /// Null if this is not a Reply Rack Configuration Status message.
    /// </summary>
    public ReplyRackConfigurationStatus? RackConfigurationStatus { get; set; }

    /// <summary>
    /// Parsed audio monitor actions for Reply Audio Monitor Actions messages (Message ID 0x0010).
    /// Null if this is not a Reply Audio Monitor Actions message.
    /// </summary>
    public ReplyAudioMonitorActions? AudioMonitorActions { get; set; }

    /// <summary>
    /// Parsed telephony client state for Reply Telephony Client State messages (Message ID 0x011E).
    /// Null if this is not a Reply Telephony Client State message.
    /// </summary>
    public ReplyTelephonyClientState? TelephonyClientState { get; set; }

    /// <summary>
    /// Parsed telephony client disconnect acknowledgement for Reply Telephony Client Disconnect messages (Message ID 0x0123).
    /// Null if this is not a Reply Telephony Client Disconnect message.
    /// </summary>
    public ReplyTelephonyClientDisconnect? TelephonyClientDisconnect { get; set; }

    /// <summary>
    /// Parsed telephony client disconnect outgoing acknowledgement for Reply Telephony Client Disconnect Outgoing messages (Message ID 0x0126).
    /// Null if this is not a Reply Telephony Client Disconnect Outgoing message.
    /// </summary>
    public ReplyTelephonyClientDisconnectOutgoing? TelephonyClientDisconnectOutgoing { get; set; }

    /// <summary>
    /// Parsed proxy indication state for Reply Get Proxy Indication State messages (Message ID 0x0137).
    /// Null if this is not a Reply Get Proxy Indication State message.
    /// </summary>
    public ReplyGetProxyIndicationState? ProxyIndicationState { get; set; }

    /// <summary>
    /// Parsed set proxy indication state for Reply Set Proxy Indication State messages (Message ID 0x0139).
    /// Null if this is not a Reply Set Proxy Indication State message.
    /// </summary>
    public ReplySetProxyIndicationState? SetProxyIndicationState { get; set; }

    /// <summary>
    /// Parsed proxy display data for Reply Get Proxy Display Data messages (Message ID 0x013B).
    /// Null if this is not a Reply Get Proxy Display Data message.
    /// </summary>
    public ReplyGetProxyDisplayData? ProxyDisplayData { get; set; }

    /// <summary>
    /// Parsed set proxy display data for Reply Set Proxy Display Data messages (Message ID 0x013D).
    /// Null if this is not a Reply Set Proxy Display Data message.
    /// </summary>
    public ReplySetProxyDisplayData? SetProxyDisplayData { get; set; }

    /// <summary>
    /// Parsed panel keys public state for Reply Panel Keys Public State messages (Message ID 0x0141).
    /// Null if this is not a Reply Panel Keys Public State message.
    /// </summary>
    public ReplyPanelKeysPublicState? PanelKeysPublicState { get; set; }

    /// <summary>
    /// Parsed panel keys status auto updates acknowledgement for Reply Panel Keys Status Auto Updates messages (Message ID 0x013F).
    /// Null if this is not a Reply Panel Keys Status Auto Updates message.
    /// </summary>
    public ReplyPanelKeysStatusAutoUpdates? PanelKeysStatusAutoUpdates { get; set; }

    /// <summary>
    /// Parsed panel keys public set state for Reply Panel Keys Public Set State messages (Message ID 0x0156).
    /// Null if this is not a Reply Panel Keys Public Set State message.
    /// </summary>
    public ReplyPanelKeysPublicSetState? PanelKeysPublicSetState { get; set; }

    /// <summary>
    /// Parsed telephony key status for Reply Telephony Key Status messages (Message ID 0x015C).
    /// Unsolicited key pressed state information. Null if this is not a Reply Telephony Key Status message.
    /// </summary>
    public ReplyTelephonyKeyStatus? TelephonyKeyStatus { get; set; }

    /// <summary>
    /// Parsed set panel audio front end state for Reply Set Panel Audio Front End State messages (Message ID 0x0162).
    /// Acknowledges the successful receipt and processing of the request. Null if this is not a Reply Set Panel Audio Front End State message.
    /// </summary>
    public ReplySetPanelAudioFrontEndState? SetPanelAudioFrontEndState { get; set; }

    /// <summary>
    /// Parsed get panel audio front end state for Reply Get Panel Audio Front End State messages (Message ID 0x0164).
    /// Contains the current state of the HCI requested panel audio front end. Null if this is not a Reply Get Panel Audio Front End State message.
    /// </summary>
    public ReplyGetPanelAudioFrontEndState? GetPanelAudioFrontEndState { get; set; }

    /// <summary>
    /// Parsed telephony key status enable for Reply Telephony Key Status Enable messages (Message ID 0x015B).
    /// Confirms whether telephony key status messages are enabled or disabled. Null if this is not a Reply Telephony Key Status Enable message.
    /// </summary>
    public ReplyTelephonyKeyStatusEnable? TelephonyKeyStatusEnable { get; set; }

    /// <summary>
    /// Parsed system crosspoint for Reply System Crosspoint messages (Message ID 0x00C8).
    /// Contains remote routes to or from this matrix, or unsolicited route transitions.
    /// Null if this is not a Reply System Crosspoint message.
    /// </summary>
    public ReplySystemCrosspoint? SystemCrosspoint { get; set; }

    /// <summary>
    /// Parsed panel keys unlatch all for Reply Panel Keys Unlatch All messages (Message ID 0x014F).
    /// Confirms the unlatching of all keys on the specified panel. Null if this is not a Reply Panel Keys Unlatch All message.
    /// </summary>
    public ReplyPanelKeysUnlatchAll? PanelKeysUnlatchAll { get; set; }

    /// <summary>
    /// Parsed panel discovery for Reply Panel Discovery messages (Message ID 0x00F8).
    /// Reply to the IP panel discovery request. Null if this is not a Reply Panel Discovery message.
    /// </summary>
    public ReplyPanelDiscovery? PanelDiscovery { get; set; }

    /// <summary>
    /// Parsed IP panel list for Reply IP Panel List messages (Message ID 0x00F8, Sub ID 9).
    /// Contains the discovered IP panel cache. Null if this is not a Reply IP Panel List message.
    /// </summary>
    public ReplyIPPanelList? IPPanelList { get; set; }

    /// <summary>
    /// Parsed IP panel settings assign reply for Reply IP Panel Settings Assign messages (Message ID 0x00F8, Sub ID 3).
    /// Contains the status and MAC address of the panel. Null if this is not a Reply IP Panel Settings Assign message.
    /// </summary>
    public ReplyIPPanelSettingsAssign? IPPanelSettingsAssign { get; set; }

    /// <summary>
    /// Parsed panel shift page action for Reply Panel Shift Page Action messages (Message ID 0x0154).
    /// Contains the current active page of the specified panel. Null if this is not a Reply Panel Shift Page Action message.
    /// </summary>
    public ReplyPanelShiftPageAction? PanelShiftPageAction { get; set; }

    /// <summary>
    /// Parsed key group status for Reply Key Group Status messages (Message ID 0x00FC).
    /// Contains the current assignment state of key group(s). Null if this is not a Reply Key Group Status message.
    /// </summary>
    public ReplyKeyGroupStatus? KeyGroupStatus { get; set; }

    /// <summary>
    /// Parsed frame status for Reply Frame Status messages (Message ID 0x0062).
    /// Contains PSU status and CPU temperature. Can be requested or sent automatically on PSU alarm transitions.
    /// Null if this is not a Reply Frame Status message.
    /// </summary>
    public ReplyFrameStatus? FrameStatus { get; set; }

    /// <summary>
    /// Parsed xpt and level status for Reply Xpt and Level Status messages (Message ID 0x0097).
    /// Contains source, destination, and level information for crosspoints.
    /// Can be requested or sent unsolicited when a crosspoint is made, broken, or level adjusted.
    /// Null if this is not a Reply Xpt and Level Status message.
    /// </summary>
    public ReplyXptAndLevelStatus? XptAndLevelStatus { get; set; }

    /// <summary>
    /// Parsed trunk usage statistics for Reply Trunk Usage Statistics messages (Message ID 0x0171).
    /// Contains trunk statistics equivalent to those displayed every 10 minutes in the EHX event log.
    /// Null if this is not a Reply Trunk Usage Statistics message.
    /// </summary>
    public ReplyTrunkUsageStatistics? TrunkUsageStatistics { get; set; }

    /// <summary>
    /// Parsed IPA card redundancy switch reply for Reply IPA Card Redundancy Switch messages (Message ID 0x0183).
    /// Contains switch states for IPA cards indicating whether redundancy switching is enabled or disabled.
    /// Null if this is not a Reply IPA Card Redundancy Switch message.
    /// </summary>
    public ReplyIpaCardRedundancySwitch? IpaCardRedundancySwitch { get; set; }

    /// <summary>
    /// Parsed macro panel keys public state for Reply Macro Panel Keys Public State messages (Message ID 0x0173).
    /// Contains the current latched state of Dynam-EC macro keys on panels.
    /// Can be a response to a request or sent unsolicited when panels come online.
    /// Null if this is not a Reply Macro Panel Keys Public State message.
    /// </summary>
    public ReplyMacroPanelKeysPublicState? MacroPanelKeysPublicState { get; set; }

    /// <summary>
    /// Parsed alt text set reply for Reply Alt Text Set messages (Message ID 0x0181).
    /// Confirms the Alt Text state for the specified panel.
    /// Null if this is not a Reply Alt Text Set message.
    /// </summary>
    public ReplyAltTextSet? AltTextSet { get; set; }

    /// <summary>
    /// Parsed assigned keys with labels for Reply Assigned Keys (With Labels) messages (Message ID 0x017D).
    /// Contains all assigned key configurations with labels for the selected panel.
    /// Null if this is not a Reply Assigned Keys (With Labels) message.
    /// </summary>
    public ReplyAssignedKeysWithLabels? AssignedKeysWithLabels { get; set; }

    /// <summary>
    /// Parsed role state set result for Reply Role State Set messages (Message ID 0x0187).
    /// Contains the result of a role state change request.
    /// Null if this is not a Reply Role State Set message.
    /// </summary>
    public ReplyRoleStateSet? RoleStateSet { get; set; }

    /// <summary>
    /// Parsed network redundancy endpoint status for Reply Network Redundancy Endpoint Status messages (Message ID 0x0189).
    /// Contains the network redundancy status for one or more endpoints.
    /// Null if this is not a Reply Network Redundancy Endpoint Status message.
    /// </summary>
    public ReplyNetworkRedundancyEndpointStatus? NetworkRedundancyEndpointStatus { get; set; }

    /// <summary>
    /// Parsed network redundancy card status for Reply Network Redundancy Card Status messages (Message ID 0x018B).
    /// Contains the main/standby state for one or more cards.
    /// Null if this is not a Reply Network Redundancy Card Status message.
    /// </summary>
    public ReplyNetworkRedundancyCardStatus? NetworkRedundancyCardStatus { get; set; }

    /// <summary>
    /// Parsed panel connection management action result for Reply Panel Connection Management Action messages (Message ID 0x018F).
    /// Contains the result of a panel connection management action request.
    /// Null if this is not a Reply Panel Connection Management Action message.
    /// </summary>
    public ReplyPanelConnectionManagementAction? PanelConnectionManagementAction { get; set; }

    /// <summary>
    /// Parsed beltpack information for Reply Beltpack Information messages (Message ID 0x0102).
    /// Contains beltpack hardware, registration, and configuration data.
    /// Null if this is not a Reply Beltpack Information message.
    /// </summary>
    public ReplyBeltpackInformation? BeltpackInformation { get; set; }

    /// <summary>
    /// Parsed beltpack delete result for Reply Beltpack Delete messages (Message ID 0x0196).
    /// Contains success/failure status of the delete operation.
    /// Null if this is not a Reply Beltpack Delete message.
    /// </summary>
    public ReplyBeltpackDelete? BeltpackDelete { get; set; }
}
