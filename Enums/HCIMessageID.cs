namespace HCILibrary.Enums;

/// <summary>
/// Message ID enumeration for HCI requests and responses.
/// Values will be added as message types are defined.
/// </summary>
public enum HCIMessageID : ushort
{
    /// <summary>
    /// Unknown or undefined message type.
    /// </summary>
    Unknown = 0x0000,

    /// <summary>
    /// Broadcast System Message (HCIv1).
    /// </summary>
    BroadcastSystemMessage = 0x0001,

    /// <summary>
    /// Request System Messages (HCIv2).
    /// </summary>
    RequestSystemMessages = 0x0002,

    /// <summary>
    /// Request System Status (HCIv2).
    /// </summary>
    RequestSystemStatus = 0x0003,

    /// <summary>
    /// Reply System Card Status (HCIv2).
    /// Contains the current status of all cards in the matrix.
    /// </summary>
    ReplySystemCardStatus = 0x0004,

    /// <summary>
    /// Request Panel Status (HCIv2).
    /// </summary>
    RequestPanelStatus = 0x0005,

    /// <summary>
    /// Reply Panel Status (HCIv2).
    /// </summary>
    ReplyPanelStatus = 0x001E,

    /// <summary>
    /// Request Crosspoint Status (HCIv2).
    /// </summary>
    RequestCrosspointStatus = 0x000D,

    /// <summary>
    /// Reply Crosspoint Status (HCIv2).
    /// </summary>
    ReplyCrosspointStatus = 0x000E,

    /// <summary>
    /// Request Actions Status (HCIv2).
    /// </summary>
    RequestActionsStatus = 0x000F,

    /// <summary>
    /// Request Conference Actions (HCIv2).
    /// </summary>
    RequestConferenceActions = 0x0011,

    /// <summary>
    /// Request EHX Control Actions (HCIv2).
    /// Same message ID as RequestConferenceActions (0x0011), distinguished by Action Type in payload.
    /// </summary>
    RequestEhxControlActions = 0x0011,

    /// <summary>
    /// Request Conference Status (HCIv2).
    /// </summary>
    RequestConferenceStatus = 0x0013,

    /// <summary>
    /// Reply Conference Status (HCIv2).
    /// </summary>
    ReplyConferenceStatus = 0x0014,

    /// <summary>
    /// Request EHX Control Card Status (HCIv2).
    /// </summary>
    RequestEhxControlCardStatus = 0x0015,

    /// <summary>
    /// Reply EHX Control Card Status (HCIv2).
    /// </summary>
    ReplyEhxControlCardStatus = 0x0016,

    /// <summary>
    /// Request GPIO/SFO Status (HCIv2).
    /// </summary>
    RequestGpioSfoStatus = 0x0017,

    /// <summary>
    /// Reply GPIO/SFO Status (HCIv2).
    /// </summary>
    ReplyGpioSfoStatus = 0x0018,

    /// <summary>
    /// Request Input Level Actions (HCIv2).
    /// </summary>
    RequestInputLevelActions = 0x0020,

    /// <summary>
    /// Request Input Level Status (HCIv2).
    /// </summary>
    RequestInputLevelStatus = 0x0021,

    /// <summary>
    /// Reply Input Level Status (HCIv2).
    /// </summary>
    ReplyInputLevelStatus = 0x0022,

    /// <summary>
    /// Request Output Level Actions (HCIv2).
    /// </summary>
    RequestOutputLevelActions = 0x0023,

    /// <summary>
    /// Request Output Level Status (HCIv2).
    /// </summary>
    RequestOutputLevelStatus = 0x0024,

    /// <summary>
    /// Reply Output Level Status (HCIv2).
    /// </summary>
    ReplyOutputLevelStatus = 0x0025,

    /// <summary>
    /// Request Crosspoint Level Actions (HCIv2).
    /// </summary>
    RequestCrosspointLevelActions = 0x0026,

    /// <summary>
    /// Request Crosspoint Level Status (HCIv2).
    /// </summary>
    RequestCrosspointLevelStatus = 0x0027,

    /// <summary>
    /// Reply Crosspoint Level Status (HCIv2).
    /// </summary>
    ReplyCrosspointLevelStatus = 0x0028,

    /// <summary>
    /// Request CPU Reset (HCIv2).
    /// </summary>
    RequestCpuReset = 0x0029,

    /// <summary>
    /// Request Xpt and Level Status (HCIv2).
    /// Requests the broadcast of all crosspoints currently in the matrix along with their current level.
    /// Message ID 0x0096 (150).
    /// </summary>
    RequestXptAndLevelStatus = 0x0096,

    /// <summary>
    /// Reply Xpt and Level Status (HCIv2).
    /// Reply containing crosspoints and their current levels.
    /// Message ID 0x0097 (151).
    /// </summary>
    ReplyXptAndLevelStatus = 0x0097,

    /// <summary>
    /// Request IFB Status (HCIv2).
    /// Requests attributes of the specified IFB. Response is Reply IFB Status.
    /// </summary>
    RequestIfbStatus = 0x003D,

    /// <summary>
    /// Reply IFB Status (HCIv2).
    /// Response to Request IFB Status. Contains IFB attribute information.
    /// </summary>
    ReplyIfbStatus = 0x003E,

    /// <summary>
    /// Request IFB Set (HCIv2).
    /// Requests an IFB property edit, add, or delete. Response is Reply IFB Status.
    /// </summary>
    RequestIfbSet = 0x003F,

    /// <summary>
    /// Request Set System Time (HCIv2).
    /// Sets the system time on the config card. Only one request accepted per config card boot.
    /// No reply is sent for this message.
    /// </summary>
    RequestSetSystemTime = 0x0044,

    /// <summary>
    /// Request Frame Status (HCIv2).
    /// Requests the core matrix frame status (not interface cards).
    /// Message ID 0x0061 (97).
    /// </summary>
    RequestFrameStatus = 0x0061,

    /// <summary>
    /// Reply Frame Status (HCIv2).
    /// Reply to Request Frame Status containing core matrix frame status.
    /// Message ID 0x0062 (98).
    /// </summary>
    ReplyFrameStatus = 0x0062,

    /// <summary>
    /// Request Unicode Alias Add (HCIv2).
    /// </summary>
    RequestUnicodeAliasAdd = 0x00F4,

    /// <summary>
    /// Reply Unicode Alias Status (HCIv2).
    /// </summary>
    ReplyUnicodeAliasStatus = 0x00F5,

    /// <summary>
    /// Request Unicode Alias List (HCIv2).
    /// </summary>
    RequestUnicodeAliasList = 0x00F6,

    /// <summary>
    /// Request Alias Delete (HCIv2).
    /// </summary>
    RequestAliasDelete = 0x0084,

    /// <summary>
    /// Reply Alias Delete (HCIv2).
    /// </summary>
    ReplyAliasDelete = 0x0085,

    /// <summary>
    /// Request Entity Info (HCIv2).
    /// Requests entity instance information (Conferences, Groups, IFBs).
    /// </summary>
    RequestEntityInfo = 0x00AF,

    /// <summary>
    /// Reply Entity Info (HCIv2).
    /// Response to Request Entity Info. May span multiple messages (check continuation flag).
    /// </summary>
    ReplyEntityInfo = 0x00B0,

    /// <summary>
    /// Request Panel Keys Status (HCIv2).
    /// Requests the latch status of keys on a specific panel or role.
    /// </summary>
    RequestPanelKeysStatus = 0x00B1,

    /// <summary>
    /// Reply Panel Keys Status (HCIv2).
    /// Returns the status of all keys available on the requested panel or role.
    /// </summary>
    ReplyPanelKeysStatus = 0x00B2,

    /// <summary>
    /// Request Panel Keys Action (HCIv2).
    /// Sets the state of one or more keys on a specified panel.
    /// </summary>
    RequestPanelKeysAction = 0x00B3,

    /// <summary>
    /// Reply Panel Keys Action Status (HCIv2).
    /// Returns the status of keys toggled on/off by the Request Panel Keys Action.
    /// </summary>
    ReplyPanelKeysActionStatus = 0x00B4,

    /// <summary>
    /// Request Port Info (HCIv2).
    /// Requests the connected port type and additional port information for a card.
    /// </summary>
    RequestPortInfo = 0x00B7,

    /// <summary>
    /// Reply Port Info (HCIv2).
    /// Returns the connected port type and additional port information for a card.
    /// </summary>
    ReplyPortInfo = 0x00B8,

    /// <summary>
    /// Request Locally Assigned Keys (HCIv2).
    /// Requests all locally assigned key configuration for a selected panel.
    /// </summary>
    RequestLocallyAssignedKeys = 0x00B9,

    /// <summary>
    /// Reply Locally Assigned Keys (HCIv2).
    /// Returns the configuration of all locally assigned keys for a selected panel.
    /// </summary>
    ReplyLocallyAssignedKeys = 0x00BA,

    /// <summary>
    /// Request Assigned Keys (HCIv2).
    /// Requests all assigned key configuration for a selected panel.
    /// </summary>
    RequestAssignedKeys = 0x00E7,

    /// <summary>
    /// Reply Assigned Keys (HCIv2).
    /// Returns the configuration of all assigned keys for a selected panel.
    /// </summary>
    ReplyAssignedKeys = 0x00E8,

    /// <summary>
    /// Request Card Info (HCIv2).
    /// Retrieves the card information at a specified slot together with its health status.
    /// </summary>
    RequestCardInfo = 0x00C3,

    /// <summary>
    /// Reply Card Info (HCIv2).
    /// Returns the card information at a specified slot together with its health status.
    /// </summary>
    ReplyCardInfo = 0x00C4,

    /// <summary>
    /// Request Conference / Fixed Group Members Edits (HCIv2).
    /// Requests all locally edited members of all Fixed groups or Conferences (partylines).
    /// </summary>
    RequestConferenceFixedGroupMembersEdits = 0x00C5,

    /// <summary>
    /// Reply Conference Assignments (HCIv2).
    /// Returns members of all conferences specifying talk, listen and local edit status.
    /// </summary>
    ReplyConferenceAssignments = 0x00C6,

    /// <summary>
    /// Set Config for Multiple Keys (HCIv2).
    /// Sets the config for multiple keys on a panel with advanced key property control.
    /// </summary>
    RequestSetConfigMultipleKeys = 0x00CD,

    /// <summary>
    /// Reply Set Config for Multiple Keys (HCIv2).
    /// Replies to the Set Config for Multiple Keys message for selected panel.
    /// </summary>
    ReplySetConfigMultipleKeys = 0x00CE,

    /// <summary>
    /// Reply System Crosspoint (HCIv2).
    /// Replies to the System Crosspoint Request with all remote routes, or sent
    /// unsolicited when a route is made or broken.
    /// Message ID 0x00C8 (200).
    /// </summary>
    ReplySystemCrosspoint = 0x00C8,

    /// <summary>
    /// Request Forced Listen Edits (HCIv2).
    /// Requests all forced listen edits.
    /// Note: Shares Message ID 0x00C9 with RequestSystemCrosspoint.
    /// </summary>
    RequestForcedListenEdits = 0x00C9,

    /// <summary>
    /// Request System Crosspoint (HCIv2).
    /// Requests the state of linked matrices point to point routes (cross-points).
    /// Note: Shares Message ID 0x00C9 (201) with RequestForcedListenEdits.
    /// </summary>
    RequestSystemCrosspoint = 0x00C9,

    /// <summary>
    /// Reply Forced Listen Edits (HCIv2).
    /// Returns all forced listen edits.
    /// </summary>
    ReplyForcedListenEdits = 0x00CA,

    /// <summary>
    /// Request Forced Listen Actions (HCIv2).
    /// Allows the host to set a forced listen crosspoint in the matrix.
    /// </summary>
    RequestForcedListenActions = 0x00E1,

    /// <summary>
    /// Request Remote Key Actions (HCIv2).
    /// Allows the host to remotely assign keys on a panel.
    /// </summary>
    RequestRemoteKeyActions = 0x00EB,

    /// <summary>
    /// Reply Remote Key Actions (HCIv2).
    /// Response to Request Remote Key Actions.
    /// </summary>
    ReplyRemoteKeyActions = 0x00EC,

    /// <summary>
    /// Request Remote Key Action Status (HCIv2).
    /// Requests all key assignment actions for all panels or a specific panel.
    /// </summary>
    RequestRemoteKeyActionStatus = 0x00ED,

    /// <summary>
    /// Reply Remote Key Action Status (HCIv2).
    /// Sent when a new key assignment is made, or in response to Request Remote Key Action Status.
    /// </summary>
    ReplyRemoteKeyActionStatus = 0x00EE,

    /// <summary>
    /// Request VoIP Status / Request Peripheral Info / Request Panel Discovery / Request IP Panel List (HCIv2).
    /// Shared message ID (0x00F7) - differentiated by Sub Message ID in payload.
    /// Sub ID 0x00 = Panel Discovery, Sub ID 0x08 = IP Panel List, Sub ID 0x14 = Peripheral Info, Sub ID 0x17 = VoIP Status.
    /// </summary>
    RequestVoIPStatus = 0x00F7,

    /// <summary>
    /// Request Peripheral Info (HCIv2).
    /// Requests software and hardware version info for FreeSpeak 2 Antenna, Splitters, Beltpacks, etc.
    /// Same message ID as RequestVoIPStatus (0x00F7), distinguished by Sub Message ID 0x14.
    /// </summary>
    RequestPeripheralInfo = 0x00F7,

    /// <summary>
    /// Request Panel Discovery (HCIv2).
    /// Requests discovery of IP panels connected to the current LAN.
    /// Same message ID as RequestVoIPStatus (0x00F7), distinguished by Sub Message ID 0x00.
    /// Message ID 0x00F7 (247).
    /// </summary>
    RequestPanelDiscovery = 0x00F7,

    /// <summary>
    /// Request IP Panel List (HCIv2).
    /// Requests the content of the discovered IP panels cache stored in the matrix.
    /// Same message ID as RequestVoIPStatus (0x00F7), distinguished by Sub Message ID 0x08.
    /// Message ID 0x00F7 (247).
    /// </summary>
    RequestIPPanelList = 0x00F7,

    /// <summary>
    /// Request IP Panel Settings Assign (HCIv2).
    /// Applies IP settings to a specified IP V-Series panel.
    /// Same message ID as RequestVoIPStatus (0x00F7), distinguished by Sub Message ID 0x02.
    /// Message ID 0x00F7 (247).
    /// </summary>
    RequestIPPanelSettingsAssign = 0x00F7,

    /// <summary>
    /// Reply Peripheral Info (HCIv2).
    /// Response to Request Peripheral Info. Never sent unsolicited.
    /// </summary>
    ReplyPeripheralInfo = 0x007B,

    /// <summary>
    /// Reply Panel Discovery (HCIv2).
    /// Reply to the IP panel discovery type request.
    /// Message ID 0x00F8 (248).
    /// Shared message ID - differentiated by Sub Message ID in payload.
    /// Sub ID 1 = Discovery Reply, Sub ID 9 = Panel Cache Reply.
    /// </summary>
    ReplyPanelDiscovery = 0x00F8,

    /// <summary>
    /// Reply IP Panel List (HCIv2).
    /// Reply to Request IP Panel List, contains the discovered IP panel cache.
    /// Same message ID as ReplyPanelDiscovery (0x00F8), distinguished by Sub Message ID 9.
    /// Message ID 0x00F8 (248).
    /// </summary>
    ReplyIPPanelList = 0x00F8,

    /// <summary>
    /// Reply IP Panel Settings Assign (HCIv2).
    /// Reply to Request IP Panel Settings Assign.
    /// Same message ID as ReplyPanelDiscovery (0x00F8), distinguished by Sub Message ID 3.
    /// Message ID 0x00F8 (248).
    /// </summary>
    ReplyIPPanelSettingsAssign = 0x00F8,

    /// <summary>
    /// Reply VoIP Status (HCIv2).
    /// Returns VoIP Status for all IVC32 channels on all IVC32 and E-IPA cards.
    /// NOTE: Verify this message ID (0x00F9) against protocol documentation.
    /// </summary>
    ReplyVoIPStatus = 0x00F9,

    /// <summary>
    /// Request Key Group Action (HCIv2).
    /// Associates a key entity (port, IFB instance, etc.) to a key group.
    /// Message ID 0x00FB (251).
    /// </summary>
    RequestKeyGroupAction = 0x00FB,

    /// <summary>
    /// Reply Key Group Status (HCIv2).
    /// Reply to Request Key Group Action.
    /// Message ID 0x00FC (252).
    /// </summary>
    ReplyKeyGroupStatus = 0x00FC,

    /// <summary>
    /// Request Change Telephone Hook State (HCIv2).
    /// Toggles the on/off hook state of a Clear-Com telephone hybrid (TEL-14, LQ-SIP).
    /// </summary>
    RequestChangeTelephoneHookState = 0x010D,

    /// <summary>
    /// Request VOX Threshold Status (HCIv2).
    /// Requests the VOX threshold levels for inputs on the matrix.
    /// Reply uses the same message ID (0x004A).
    /// </summary>
    RequestVoxThresholdStatus = 0x004A,

    /// <summary>
    /// Reply VOX Threshold Status (HCIv2).
    /// Returns the VOX threshold levels for inputs on the matrix.
    /// Same message ID as Request (0x004A) - distinguished by context.
    /// </summary>
    ReplyVoxThresholdStatus = 0x004A,

    /// <summary>
    /// Request VOX Threshold Set (HCIv2).
    /// Sets the VOX threshold levels for one or more ports.
    /// </summary>
    RequestVoxThresholdSet = 0x0048,

    /// <summary>
    /// Reply Beltpack Status (HCIv2).
    /// Sent when a beltpack status changes (online/offline, frequency, mode, etc.).
    /// </summary>
    ReplyBeltpackStatus = 0x004C,

    /// <summary>
    /// Request Beltpack Information (HCIv2).
    /// Requests all beltpack data records from the matrix including hardware, registration, and configuration.
    /// Message ID 0x0101 (257).
    /// </summary>
    RequestBeltpackInformation = 0x0101,

    /// <summary>
    /// Reply Beltpack Information (HCIv2).
    /// Returns beltpack data records from the matrix.
    /// Message ID 0x0102 (258).
    /// </summary>
    ReplyBeltpackInformation = 0x0102,

    /// <summary>
    /// Request SRecord Transmission Initiation (HCIv2).
    /// Initiates an SRecord transfer to the matrix (configuration or firmware).
    /// </summary>
    RequestSRecordTransmissionInitiation = 0x0045,

    /// <summary>
    /// Request Rack Properties (HCIv2).
    /// Requests rack property information such as the active configuration bank.
    /// Reply uses the same message ID (0x002C), distinguished by Sub Message ID.
    /// </summary>
    RequestRackProperties = 0x002C,

    /// <summary>
    /// Reply Rack Properties (HCIv2).
    /// Response to Request Rack Properties containing rack property information.
    /// Same message ID as Request (0x002C) - distinguished by Sub Message ID.
    /// </summary>
    ReplyRackProperties = 0x002C,

    /// <summary>
    /// Reply Audio Monitor Actions (HCIv2).
    /// Response to Request Audio Monitor Actions, contains the current audio monitor state.
    /// Message ID 0x0010.
    /// </summary>
    ReplyAudioMonitorActions = 0x0010,

    /// <summary>
    /// Request Audio Monitor Actions (HCIv2).
    /// Allows the host to add or remove port monitors in the matrix.
    /// Same message ID as RequestConferenceActions (0x0011), distinguished by Action Type 0x0100.
    /// </summary>
    RequestAudioMonitorActions = 0x0011,

    /// <summary>
    /// Request Telephony Client Get State (HCIv2).
    /// Requests the state of specified or all Telephony Client ports (TEL-14, SIP).
    /// </summary>
    RequestTelephonyClientGetState = 0x011D,

    /// <summary>
    /// Reply Telephony Client State (HCIv2).
    /// Response to Request Telephony Client Get State containing port states.
    /// </summary>
    ReplyTelephonyClientState = 0x011E,

    /// <summary>
    /// Request Telephony Client Disconnect Incoming (HCIv2).
    /// Disconnects a call that is currently connected (incoming or outgoing).
    /// Same message ID as ReplyTelephonyClientDisconnect - distinguished by context.
    /// </summary>
    RequestTelephonyClientDisconnect = 0x0123,

    /// <summary>
    /// Reply Telephony Client Disconnect Incoming (HCIv2).
    /// Acknowledges the disconnect request from the associated Telephony Server.
    /// Same message ID as RequestTelephonyClientDisconnect - distinguished by context.
    /// </summary>
    ReplyTelephonyClientDisconnect = 0x0123,

    /// <summary>
    /// Request Telephony Client Disconnect Outgoing (HCIv2).
    /// Requests disconnection of a Telephony call connected via a third party SIP server switch.
    /// Message ID 0x0125 (293).
    /// </summary>
    RequestTelephonyClientDisconnectOutgoing = 0x0125,

    /// <summary>
    /// Reply Telephony Client Disconnect Outgoing (HCIv2).
    /// Acknowledges the disconnection of the call associated with the specified port.
    /// Message ID 0x0126 (294).
    /// </summary>
    ReplyTelephonyClientDisconnectOutgoing = 0x0126,

    /// <summary>
    /// Request Telephony Client Dial Info Outgoing (HCIv2).
    /// Sends dial information from the Matrix to the specified SIP server application.
    /// Message ID 0x0127 (295). No reply is sent for this message.
    /// </summary>
    RequestTelephonyClientDialInfoOutgoing = 0x0127,

    /// <summary>
    /// Request Telephony Client Dial Info Incoming (HCIv2).
    /// Sends dial information from the SIP call server to the matrix.
    /// Message ID 0x0128 (296).
    /// </summary>
    RequestTelephonyClientDialInfoIncoming = 0x0128,

    /// <summary>
    /// Request Get Proxy Indication State (HCIv2).
    /// Requests the state of proxy key LED indications for specified or all panels.
    /// These are 3rd party system proxy keys driven via HCI.
    /// Message ID 0x0136 (310).
    /// </summary>
    RequestGetProxyIndicationState = 0x0136,

    /// <summary>
    /// Reply Get Proxy Indication State (HCIv2).
    /// Response containing proxy key LED indication states.
    /// Message ID 0x0137 (311).
    /// </summary>
    ReplyGetProxyIndicationState = 0x0137,

    /// <summary>
    /// Request Set Proxy Indication State (HCIv2).
    /// Sets the state of proxy key LED indications for one or more panel keys.
    /// These are 3rd party system proxy keys driven via HCI.
    /// Message ID 0x0138 (312).
    /// </summary>
    RequestSetProxyIndicationState = 0x0138,

    /// <summary>
    /// Reply Set Proxy Indication State (HCIv2).
    /// Response containing resultant proxy key LED indication states after a set request.
    /// Message ID 0x0139 (313).
    /// </summary>
    ReplySetProxyIndicationState = 0x0139,

    /// <summary>
    /// Request Get Proxy Display Data (HCIv2).
    /// Requests the display data currently set for specified third-party proxy displays.
    /// Display data is shown on physical displays (e.g., OLED on V-Series panels).
    /// Message ID 0x013A (314).
    /// </summary>
    RequestGetProxyDisplayData = 0x013A,

    /// <summary>
    /// Reply Get Proxy Display Data (HCIv2).
    /// Response containing proxy display data for panel keys.
    /// Message ID 0x013B (315).
    /// </summary>
    ReplyGetProxyDisplayData = 0x013B,

    /// <summary>
    /// Request Set Proxy Display Data (HCIv2).
    /// Sets the display data for specified third-party proxy displays.
    /// Display data is shown on physical displays (e.g., OLED on V-Series panels).
    /// Message ID 0x013C (316).
    /// </summary>
    RequestSetProxyDisplayData = 0x013C,

    /// <summary>
    /// Reply Set Proxy Display Data (HCIv2).
    /// Response containing resultant proxy display data after a set request.
    /// Message ID 0x013D (317).
    /// </summary>
    ReplySetProxyDisplayData = 0x013D,

    /// <summary>
    /// Request Panel Keys Status Auto Updates (HCIv2).
    /// Requests unsolicited transmission of Panel Key Status Reply messages when key status updates.
    /// Message ID 0x013E (318).
    /// </summary>
    RequestPanelKeysStatusAutoUpdates = 0x013E,

    /// <summary>
    /// Reply Panel Keys Status Auto Updates (HCIv2).
    /// Acknowledges the received Panel Keys Status Auto Updates Request message.
    /// Message ID 0x013F (319).
    /// </summary>
    ReplyPanelKeysStatusAutoUpdates = 0x013F,

    /// <summary>
    /// Request Panel Keys Public Get State (HCIv2).
    /// Requests the current latch state of keys on the specified panel or all panels.
    /// Message ID 0x0140 (320).
    /// </summary>
    RequestPanelKeysPublicGetState = 0x0140,

    /// <summary>
    /// Reply Panel Keys Public State (HCIv2).
    /// Response containing the current latch state of keys on panels.
    /// Message ID 0x0141 (321).
    /// </summary>
    ReplyPanelKeysPublicState = 0x0141,

    /// <summary>
    /// Request Panel Keys Unlatch All (HCIv2).
    /// Requests the unlatching of all keys associated with the specified panel.
    /// Message ID 0x014E (334).
    /// </summary>
    RequestPanelKeysUnlatchAll = 0x014E,

    /// <summary>
    /// Reply Panel Keys Unlatch All (HCIv2).
    /// Reply to the Panel Keys Unlatch All Request.
    /// Message ID 0x014F (335).
    /// </summary>
    ReplyPanelKeysUnlatchAll = 0x014F,

    /// <summary>
    /// Request Panel Shift Page Action (HCIv2).
    /// Gets or sets the current page of a specified panel.
    /// Action 0 = Get Current Page, Action 1 = Set Current Page.
    /// Message ID 0x0153 (339).
    /// </summary>
    RequestPanelShiftPageAction = 0x0153,

    /// <summary>
    /// Reply Panel Shift Page Action (HCIv2).
    /// Reply to the Panel Shift Page Action Request.
    /// Message ID 0x0154 (340).
    /// </summary>
    ReplyPanelShiftPageAction = 0x0154,

    /// <summary>
    /// Request Panel Keys Public Set State (HCIv2).
    /// Requests the change of state of keys for a specific panel or all panels.
    /// Message ID 0x0155 (341).
    /// </summary>
    RequestPanelKeysPublicSetState = 0x0155,

    /// <summary>
    /// Reply Panel Keys Public Set State (HCIv2).
    /// Reply to the Public Set State Request.
    /// Message ID 0x0156 (342).
    /// </summary>
    ReplyPanelKeysPublicSetState = 0x0156,

    /// <summary>
    /// Request Telephony Key Status Enable (HCIv2).
    /// Enables/disables the transmission of key status messages for telephony related keys.
    /// Message ID 0x015A (346).
    /// </summary>
    RequestTelephonyKeyStatusEnable = 0x015A,

    /// <summary>
    /// Reply Telephony Key Status Enable (HCIv2).
    /// Reply to the Telephony Key Status Enable Request.
    /// Message ID 0x015B (347).
    /// </summary>
    ReplyTelephonyKeyStatusEnable = 0x015B,

    /// <summary>
    /// Reply Telephony Key Status (HCIv2).
    /// Unsolicited key pressed state information sent to connected Host applications.
    /// Only transmitted if telephony key state transitions have been enabled.
    /// Message ID 0x015C (348).
    /// </summary>
    ReplyTelephonyKeyStatus = 0x015C,

    /// <summary>
    /// Set Panel Audio Front End State Request (HCIv2).
    /// Sets the HCI requested state of the panel MIC, speaker and sidetone state.
    /// When enabled, acts as though a virtual talk key is pressed.
    /// Message ID 0x0161 (353).
    /// </summary>
    RequestSetPanelAudioFrontEndState = 0x0161,

    /// <summary>
    /// Set Panel Audio Front End State Reply (HCIv2).
    /// Acknowledges successful receipt and processing of the Set Panel Audio Front End State Request.
    /// Message ID 0x0162 (354).
    /// </summary>
    ReplySetPanelAudioFrontEndState = 0x0162,

    /// <summary>
    /// Get Panel Audio Front End State Request (HCIv2).
    /// Gets the HCI requested state of the panel MIC, speaker and sidetone state.
    /// Message ID 0x0163 (355).
    /// </summary>
    RequestGetPanelAudioFrontEndState = 0x0163,

    /// <summary>
    /// Get Panel Audio Front End State Reply (HCIv2).
    /// Response containing the current state of the HCI requested panel audio front end.
    /// Message ID 0x0164 (356).
    /// </summary>
    ReplyGetPanelAudioFrontEndState = 0x0164,

    /// <summary>
    /// Request Trunk Usage Statistics (HCIv2).
    /// Requests a report on the current trunk statistics held by the matrix.
    /// Message ID 0x0170 (368).
    /// </summary>
    RequestTrunkUsageStatistics = 0x0170,

    /// <summary>
    /// Reply Trunk Usage Statistics (HCIv2).
    /// Contains the current trunk statistics held by the matrix.
    /// Message ID 0x0171 (369).
    /// </summary>
    ReplyTrunkUsageStatistics = 0x0171,

    /// <summary>
    /// Request Macro Panel Keys Public State (HCIv2).
    /// Gets the current latched state of Dynam-EC macro keys on the specified panel.
    /// Message ID 0x0172 (370).
    /// </summary>
    RequestMacroPanelKeysPublicState = 0x0172,

    /// <summary>
    /// Reply Macro Panel Keys Public State (HCIv2).
    /// Contains the current latched state of Dynam-EC macro keys on the specified panel.
    /// Message ID 0x0173 (371).
    /// </summary>
    ReplyMacroPanelKeysPublicState = 0x0173,

    /// <summary>
    /// Request Alt Text State (HCIv2).
    /// Requests the 'Alt Text' display state of a specified panel or all panels.
    /// Message ID 0x0178 (376).
    /// </summary>
    RequestAltTextState = 0x0178,

    /// <summary>
    /// Reply Alt Text State (HCIv2).
    /// Contains the 'Alt Text' display state for panels.
    /// Message ID 0x0179 (377).
    /// </summary>
    ReplyAltTextState = 0x0179,

    /// <summary>
    /// Request Alt Text Set (HCIv2).
    /// Sets the state of the Alt Text feature of a panel/panels in the local matrix.
    /// Message ID 0x0180 (378).
    /// </summary>
    RequestAltTextSet = 0x0180,

    /// <summary>
    /// Reply Alt Text Set (HCIv2).
    /// Acknowledges the Alt Text Set request.
    /// Message ID 0x0181 (379).
    /// </summary>
    ReplyAltTextSet = 0x0181,

    /// <summary>
    /// Request Assigned Keys (With Labels) (HCIv2).
    /// Requests all assigned key configuration with labels for selected panel.
    /// Message ID 0x017C (380).
    /// </summary>
    RequestAssignedKeysWithLabels = 0x017C,

    /// <summary>
    /// Reply Assigned Keys (With Labels) (HCIv2).
    /// Contains assigned key configuration with labels for the panel.
    /// Message ID 0x017D (381).
    /// </summary>
    ReplyAssignedKeysWithLabels = 0x017D,

    /// <summary>
    /// Request To Enable/Disable IPA Card Redundancy Switch (HCIv2).
    /// Used to enable or disable IPA card redundancy switch.
    /// Message ID 0x0182 (386).
    /// </summary>
    RequestIpaCardRedundancySwitch = 0x0182,

    /// <summary>
    /// Reply IPA Card Redundancy Switch (HCIv2).
    /// Acknowledges the IPA card redundancy switch request.
    /// Message ID 0x0183 (387).
    /// </summary>
    ReplyIpaCardRedundancySwitch = 0x0183,

    /// <summary>
    /// Request Role State (HCIv2).
    /// Requests the current status of a matrix role.
    /// Message ID 0x0184 (388).
    /// </summary>
    RequestRoleState = 0x0184,

    /// <summary>
    /// Reply Role State (HCIv2).
    /// Contains the current status of matrix role(s).
    /// Message ID 0x0185 (389).
    /// </summary>
    ReplyRoleState = 0x0185,

    /// <summary>
    /// Request Role State Set (HCIv2).
    /// Used to request a change to the current role assignment state.
    /// Message ID 0x0186 (390).
    /// </summary>
    RequestRoleStateSet = 0x0186,

    /// <summary>
    /// Reply Role State Set (HCIv2).
    /// Reply to Request Role State Set message with success/failure status.
    /// Message ID 0x0187 (391).
    /// </summary>
    ReplyRoleStateSet = 0x0187,

    /// <summary>
    /// Request Network Redundancy Endpoint Status (HCIv2).
    /// Used to request the network redundancy status for one or all endpoints.
    /// Message ID 0x0188 (392).
    /// </summary>
    RequestNetworkRedundancyEndpointStatus = 0x0188,

    /// <summary>
    /// Reply Network Redundancy Endpoint Status (HCIv2).
    /// Contains the network redundancy status for endpoint(s).
    /// Message ID 0x0189 (393).
    /// </summary>
    ReplyNetworkRedundancyEndpointStatus = 0x0189,

    /// <summary>
    /// Request Network Redundancy Card Status (HCIv2).
    /// Used to request the main/standby state of a card.
    /// Message ID 0x018A (394).
    /// </summary>
    RequestNetworkRedundancyCardStatus = 0x018A,

    /// <summary>
    /// Reply Network Redundancy Card Status (HCIv2).
    /// Contains the main/standby state of card(s).
    /// Message ID 0x018B (395).
    /// </summary>
    ReplyNetworkRedundancyCardStatus = 0x018B,

    /// <summary>
    /// Request Panel Connection Management Action (HCIv2).
    /// Used to request actions against a matrix panel connection.
    /// Message ID 0x018E (398).
    /// </summary>
    RequestPanelConnectionManagementAction = 0x018E,

    /// <summary>
    /// Reply Panel Connection Management Action (HCIv2).
    /// Reply to panel connection management action request.
    /// Message ID 0x018F (399).
    /// </summary>
    ReplyPanelConnectionManagementAction = 0x018F,

    /// <summary>
    /// Request Beltpack Add (HCIv2).
    /// Used to request the addition of a wireless beltpack to the matrix.
    /// Message ID 0x0193 (403).
    /// </summary>
    RequestBeltpackAdd = 0x0193,

    /// <summary>
    /// Reply Beltpack Add (HCIv2).
    /// Reply to beltpack add request.
    /// Message ID 0x0194 (404).
    /// </summary>
    ReplyBeltpackAdd = 0x0194,

    /// <summary>
    /// Request Beltpack Delete (HCIv2).
    /// Used to request the deletion of a wireless beltpack from the matrix.
    /// Message ID 0x0195 (405).
    /// </summary>
    RequestBeltpackDelete = 0x0195,

    /// <summary>
    /// Reply Beltpack Delete (HCIv2).
    /// Reply to beltpack delete request.
    /// Message ID 0x0196 (406).
    /// </summary>
    ReplyBeltpackDelete = 0x0196,

    // Additional message IDs will be added here as they are defined.
}
