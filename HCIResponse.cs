using HCILibrary.Enums;
using HCILibrary.HCIResponses;
using HCILibrary.Models;

namespace HCILibrary;

/// <summary>
/// Handles decoding of HCI response messages.
/// </summary>
public static class HCIResponse
{
    /// <summary>
    /// HCIv2 protocol marker bytes.
    /// </summary>
    private static readonly byte[] HCIv2Marker = { 0xAB, 0xBA, 0xCE, 0xDE };

    /// <summary>
    /// Decodes a complete HCI message into an HCIReply object.
    /// </summary>
    /// <param name="message">The complete message including start and end markers.</param>
    /// <returns>The decoded HCIReply, or null if the message is invalid.</returns>
    public static HCIReply? Decode(byte[] message)
    {
        if (message == null || message.Length < 9) // Minimum: start(2) + length(2) + msgId(2) + flags(1) + end(2)
        {
            return null;
        }

        var reply = new HCIReply
        {
            RawMessage = message
        };

        // Skip start marker (0x5A 0x0F), read length (2 bytes, big-endian)
        reply.MessageLength = (ushort)((message[2] << 8) | message[3]);

        // Validate total length matches
        // Total message = start(2) + length field value
        if (message.Length != reply.MessageLength + 2)
        {
            return null;
        }

        // Read Message ID (2 bytes, big-endian)
        ushort messageIdValue = (ushort)((message[4] << 8) | message[5]);
        reply.MessageID = Enum.IsDefined(typeof(HCIMessageID), messageIdValue) 
            ? (HCIMessageID)messageIdValue 
            : HCIMessageID.Unknown;

        // Read Flags (1 byte)
        reply.Flags = new HCIFlags(message[6]);

        // Check for HCIv2 marker (AB BA CE DE) starting at byte 7
        int payloadStart;
        if (message.Length >= 11 && 
            message[7] == HCIv2Marker[0] && 
            message[8] == HCIv2Marker[1] && 
            message[9] == HCIv2Marker[2] && 
            message[10] == HCIv2Marker[3])
        {
            reply.Version = HCIVersion.HCIv2;
            // Schema byte is the byte after the marker
            reply.Schema = message.Length > 11 ? message[11] : (byte)1;
            reply.AdditionalProtocolIndicator = reply.Schema;
            payloadStart = 12;
        }
        else
        {
            reply.Version = HCIVersion.HCIv1;
            reply.Schema = 1;
            reply.AdditionalProtocolIndicator = 0;
            payloadStart = 7;
        }

        // Extract payload (everything between header and end marker)
        int payloadEnd = message.Length - 2; // Exclude end marker
        int payloadLength = payloadEnd - payloadStart;
        
        if (payloadLength > 0)
        {
            reply.Payload = new byte[payloadLength];
            Array.Copy(message, payloadStart, reply.Payload, 0, payloadLength);
        }

        // Process the decoded message based on version and message ID
        ProcessMessage(reply);

        return reply;
    }

    /// <summary>
    /// Processes a decoded message based on its version and message ID.
    /// </summary>
    /// <param name="reply">The decoded HCIReply to process.</param>
    private static void ProcessMessage(HCIReply reply)
    {
        switch (reply.Version)
        {
            case HCIVersion.HCIv1:
                ProcessHCIv1Message(reply);
                break;

            case HCIVersion.HCIv2:
                ProcessHCIv2Message(reply);
                break;
        }
    }

    /// <summary>
    /// Processes an HCIv1 message based on its message ID.
    /// </summary>
    /// <param name="reply">The decoded HCIReply to process.</param>
    private static void ProcessHCIv1Message(HCIReply reply)
    {
        switch (reply.MessageID)
        {
            case HCIMessageID.Unknown:
                // Unknown message type - no specific processing
                break;

            case HCIMessageID.BroadcastSystemMessage:
                DecodeEvent(reply);
                break;

            // Additional HCIv1 message handlers will be added here
            default:
                // Default handler for unimplemented message types
                break;
        }
    }

    /// <summary>
    /// Decodes an Event message (Message ID 0x0001) payload.
    /// </summary>
    /// <param name="reply">The reply containing the event payload.</param>
    private static void DecodeEvent(HCIReply reply)
    {
        var payload = reply.Payload;
        
        // Minimum payload: Class(2) + Code(2) + Reserved(4) = 8 bytes
        if (payload.Length < 8)
        {
            return;
        }

        var eventData = new HCIEvent();

        // Class: 16 bit word (big-endian)
        ushort eventClassValue = (ushort)((payload[0] << 8) | payload[1]);
        eventData.EventClass = Enum.IsDefined(typeof(HCIEventClass), eventClassValue)
            ? (HCIEventClass)eventClassValue
            : HCIEventClass.FatalError;

        // Code: 16 bit word (big-endian)
        eventData.Code = (ushort)((payload[2] << 8) | payload[3]);

        // Reserved: 4 bytes
        Array.Copy(payload, 4, eventData.Reserved, 0, 4);

        // Text: Null terminated, max 180 bytes (starts at offset 8)
        if (payload.Length > 8)
        {
            int textLength = 0;
            int maxTextLength = Math.Min(payload.Length - 8, 180);
            
            // Find null terminator or end of payload
            for (int i = 8; i < 8 + maxTextLength; i++)
            {
                if (payload[i] == 0)
                {
                    break;
                }
                textLength++;
            }

            if (textLength > 0)
            {
                eventData.Text = System.Text.Encoding.ASCII.GetString(payload, 8, textLength);
            }
        }

        reply.Event = eventData;
    }

    /// <summary>
    /// Processes an HCIv2 message based on its message ID.
    /// </summary>
    /// <param name="reply">The decoded HCIReply to process.</param>
    private static void ProcessHCIv2Message(HCIReply reply)
    {
        switch (reply.MessageID)
        {
            case HCIMessageID.Unknown:
                // Unknown message type - no specific processing
                break;

            case HCIMessageID.ReplyCrosspointStatus:
                DecodeReplyCrosspointStatus(reply);
                break;

            case HCIMessageID.ReplyConferenceStatus:
                DecodeReplyConferenceStatus(reply);
                break;

            case HCIMessageID.ReplyCrosspointLevelStatus:
                DecodeReplyCrosspointLevelStatus(reply);
                break;

            case HCIMessageID.ReplyUnicodeAliasStatus:
                DecodeReplyUnicodeAliasStatus(reply);
                break;

            case HCIMessageID.ReplyAliasDelete:
                DecodeReplyAliasDelete(reply);
                break;

            case HCIMessageID.ReplyEhxControlCardStatus:
                DecodeReplyEhxControlCardStatus(reply);
                break;

            case HCIMessageID.ReplyGpioSfoStatus:
                DecodeReplyGpioSfoStatus(reply);
                break;

            case HCIMessageID.ReplyInputLevelStatus:
                DecodeReplyInputLevelStatus(reply);
                break;

            case HCIMessageID.ReplyOutputLevelStatus:
                DecodeReplyOutputLevelStatus(reply);
                break;

            case HCIMessageID.ReplyPanelStatus:
                DecodeReplyPanelStatus(reply);
                break;

            case HCIMessageID.ReplySystemCardStatus:
                DecodeReplySystemCardStatus(reply);
                break;

            case HCIMessageID.ReplyPanelKeysStatus:
                DecodeReplyPanelKeysStatus(reply);
                break;

            case HCIMessageID.ReplyPanelKeysActionStatus:
                DecodeReplyPanelKeysActionStatus(reply);
                break;

            case HCIMessageID.ReplyPortInfo:
                DecodeReplyPortInfo(reply);
                break;

            case HCIMessageID.ReplyLocallyAssignedKeys:
                DecodeReplyLocallyAssignedKeys(reply);
                break;

            case HCIMessageID.ReplyAssignedKeys:
                DecodeReplyAssignedKeys(reply);
                break;

            case HCIMessageID.ReplyCardInfo:
                DecodeReplyCardInfo(reply);
                break;

            case HCIMessageID.ReplyConferenceAssignments:
                DecodeReplyConferenceAssignments(reply);
                break;

            case HCIMessageID.ReplySetConfigMultipleKeys:
                DecodeReplySetConfigMultipleKeys(reply);
                break;

            case HCIMessageID.ReplyForcedListenEdits:
                DecodeReplyForcedListenEdits(reply);
                break;

            case HCIMessageID.ReplyRemoteKeyActions:
                DecodeReplyRemoteKeyActions(reply);
                break;

            case HCIMessageID.ReplyRemoteKeyActionStatus:
                DecodeReplyRemoteKeyActionStatus(reply);
                break;

            case HCIMessageID.ReplyVoxThresholdStatus:
                // Note: RequestVoxThresholdStatus shares the same ID (0x004A)
                // Incoming messages with this ID are always replies
                DecodeReplyVoxThresholdStatus(reply);
                break;

            case HCIMessageID.ReplyBeltpackStatus:
                // Message ID 0x004C is shared between Beltpack Status and Panel Status
                // Differentiate by entry size: Beltpack = 8 bytes, Panel = 4 bytes
                DecodeReplyBeltpackOrPanelStatus(reply);
                break;

            case HCIMessageID.ReplyRackProperties:
                // Note: RequestRackProperties shares the same ID (0x002C)
                // Incoming messages with this ID are replies
                DecodeReplyRackProperties(reply);
                break;

            case HCIMessageID.ReplyAudioMonitorActions:
                DecodeReplyAudioMonitorActions(reply);
                break;

            case HCIMessageID.ReplyTelephonyClientState:
                DecodeReplyTelephonyClientState(reply);
                break;

            case HCIMessageID.ReplyTelephonyClientDisconnect:
                DecodeReplyTelephonyClientDisconnect(reply);
                break;

            case HCIMessageID.ReplyTelephonyClientDisconnectOutgoing:
                DecodeReplyTelephonyClientDisconnectOutgoing(reply);
                break;

            case HCIMessageID.ReplyGetProxyIndicationState:
                DecodeReplyGetProxyIndicationState(reply);
                break;

            case HCIMessageID.ReplySetProxyIndicationState:
                DecodeReplySetProxyIndicationState(reply);
                break;

            case HCIMessageID.ReplyGetProxyDisplayData:
                DecodeReplyGetProxyDisplayData(reply);
                break;

            case HCIMessageID.ReplySetProxyDisplayData:
                DecodeReplySetProxyDisplayData(reply);
                break;

            case HCIMessageID.ReplyPanelKeysPublicState:
                DecodeReplyPanelKeysPublicState(reply);
                break;

            case HCIMessageID.ReplyPanelKeysStatusAutoUpdates:
                DecodeReplyPanelKeysStatusAutoUpdates(reply);
                break;

            case HCIMessageID.ReplyPanelKeysPublicSetState:
                DecodeReplyPanelKeysPublicSetState(reply);
                break;

            case HCIMessageID.ReplyTelephonyKeyStatusEnable:
                DecodeReplyTelephonyKeyStatusEnable(reply);
                break;

            case HCIMessageID.ReplyTelephonyKeyStatus:
                DecodeReplyTelephonyKeyStatus(reply);
                break;

            case HCIMessageID.ReplySetPanelAudioFrontEndState:
                DecodeReplySetPanelAudioFrontEndState(reply);
                break;

            case HCIMessageID.ReplyGetPanelAudioFrontEndState:
                DecodeReplyGetPanelAudioFrontEndState(reply);
                break;

            case HCIMessageID.ReplySystemCrosspoint:
                DecodeReplySystemCrosspoint(reply);
                break;

            case HCIMessageID.ReplyPanelKeysUnlatchAll:
                DecodeReplyPanelKeysUnlatchAll(reply);
                break;

            case HCIMessageID.ReplyPanelDiscovery:
            // case HCIMessageID.ReplyIPPanelList: // Same message ID (0x00F8), differentiated by Sub ID
                DecodeReplyPanelDiscoveryOrIPPanelList(reply);
                break;

            case HCIMessageID.ReplyPanelShiftPageAction:
                DecodeReplyPanelShiftPageAction(reply);
                break;

            case HCIMessageID.ReplyKeyGroupStatus:
                DecodeReplyKeyGroupStatus(reply);
                break;

            case HCIMessageID.ReplyFrameStatus:
                DecodeReplyFrameStatus(reply);
                break;

            case HCIMessageID.ReplyXptAndLevelStatus:
                DecodeReplyXptAndLevelStatus(reply);
                break;

            case HCIMessageID.ReplyTrunkUsageStatistics:
                DecodeReplyTrunkUsageStatistics(reply);
                break;

            case HCIMessageID.ReplyIpaCardRedundancySwitch:
                DecodeReplyIpaCardRedundancySwitch(reply);
                break;

            case HCIMessageID.ReplyMacroPanelKeysPublicState:
                DecodeReplyMacroPanelKeysPublicState(reply);
                break;

            case HCIMessageID.ReplyAltTextSet:
                DecodeReplyAltTextSet(reply);
                break;

            case HCIMessageID.ReplyAssignedKeysWithLabels:
                DecodeReplyAssignedKeysWithLabels(reply);
                break;

            case HCIMessageID.ReplyRoleStateSet:
                DecodeReplyRoleStateSet(reply);
                break;

            case HCIMessageID.ReplyNetworkRedundancyEndpointStatus:
                DecodeReplyNetworkRedundancyEndpointStatus(reply);
                break;

            case HCIMessageID.ReplyNetworkRedundancyCardStatus:
                DecodeReplyNetworkRedundancyCardStatus(reply);
                break;

            case HCIMessageID.ReplyPanelConnectionManagementAction:
                DecodeReplyPanelConnectionManagementAction(reply);
                break;

            case HCIMessageID.ReplyBeltpackInformation:
                DecodeReplyBeltpackInformation(reply);
                break;

            case HCIMessageID.ReplyBeltpackDelete:
                DecodeReplyBeltpackDelete(reply);
                break;

            // Additional HCIv2 message handlers will be added here
            default:
                // Default handler for unimplemented message types
                break;
        }
    }

    /// <summary>
    /// Decodes a Reply Crosspoint Status message (Message ID 0x000E) payload.
    /// </summary>
    /// <param name="reply">The reply containing the crosspoint status payload.</param>
    private static void DecodeReplyCrosspointStatus(HCIReply reply)
    {
        reply.CrosspointStatus = ReplyCrosspointStatus.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Conference Status message (Message ID 0x0014) payload.
    /// </summary>
    /// <param name="reply">The reply containing the conference status payload.</param>
    private static void DecodeReplyConferenceStatus(HCIReply reply)
    {
        reply.ConferenceStatus = ReplyConferenceStatus.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Crosspoint Level Status message (Message ID 0x0028) payload.
    /// </summary>
    /// <param name="reply">The reply containing the crosspoint level status payload.</param>
    private static void DecodeReplyCrosspointLevelStatus(HCIReply reply)
    {
        reply.CrosspointLevelStatus = ReplyCrosspointLevelStatus.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Unicode Alias Status message (Message ID 0x00F5) payload.
    /// </summary>
    /// <param name="reply">The reply containing the unicode alias status payload.</param>
    private static void DecodeReplyUnicodeAliasStatus(HCIReply reply)
    {
        // The U flag indicates if this is a response to an Alias Add request (true) 
        // or an Alias List request (false)
        reply.UnicodeAliasStatus = ReplyUnicodeAliasStatus.Decode(reply.Payload, reply.Flags.U);
    }

    /// <summary>
    /// Decodes a Reply Alias Delete message (Message ID 0x0085) payload.
    /// </summary>
    /// <param name="reply">The reply containing the alias delete payload.</param>
    private static void DecodeReplyAliasDelete(HCIReply reply)
    {
        reply.AliasDeleteStatus = ReplyAliasDelete.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply EHX Control Card Status message (Message ID 0x0016) payload.
    /// </summary>
    /// <param name="reply">The reply containing the EHX control card status payload.</param>
    private static void DecodeReplyEhxControlCardStatus(HCIReply reply)
    {
        reply.EhxControlCardStatus = ReplyEhxControlCardStatus.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply GPIO/SFO Status message (Message ID 0x0018) payload.
    /// </summary>
    /// <param name="reply">The reply containing the GPIO/SFO status payload.</param>
    private static void DecodeReplyGpioSfoStatus(HCIReply reply)
    {
        reply.GpioSfoStatus = ReplyGpioSfoStatus.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Input Level Status message (Message ID 0x0022) payload.
    /// </summary>
    /// <param name="reply">The reply containing the input level status payload.</param>
    private static void DecodeReplyInputLevelStatus(HCIReply reply)
    {
        reply.InputLevelStatus = ReplyInputLevelStatus.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Output Level Status message (Message ID 0x0025) payload.
    /// </summary>
    /// <param name="reply">The reply containing the output level status payload.</param>
    private static void DecodeReplyOutputLevelStatus(HCIReply reply)
    {
        reply.OutputLevelStatus = ReplyOutputLevelStatus.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Panel Status message (Message ID 0x001E) payload.
    /// </summary>
    /// <param name="reply">The reply containing the panel status payload.</param>
    private static void DecodeReplyPanelStatus(HCIReply reply)
    {
        reply.PanelStatus = ReplyPanelStatus.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply System Card Status message (Message ID 0x0004) payload.
    /// </summary>
    /// <param name="reply">The reply containing the system card status payload.</param>
    private static void DecodeReplySystemCardStatus(HCIReply reply)
    {
        reply.SystemCardStatus = ReplySystemCardStatus.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Panel Keys Status message (Message ID 0x00B2) payload.
    /// </summary>
    /// <param name="reply">The reply containing the panel keys status payload.</param>
    private static void DecodeReplyPanelKeysStatus(HCIReply reply)
    {
        reply.PanelKeysStatus = ReplyPanelKeysStatus.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Panel Keys Action Status message (Message ID 0x00B4) payload.
    /// </summary>
    /// <param name="reply">The reply containing the panel keys action status payload.</param>
    private static void DecodeReplyPanelKeysActionStatus(HCIReply reply)
    {
        reply.PanelKeysActionStatus = ReplyPanelKeysActionStatus.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Port Info message (Message ID 0x00B8) payload.
    /// </summary>
    /// <param name="reply">The reply containing the port info payload.</param>
    private static void DecodeReplyPortInfo(HCIReply reply)
    {
        reply.PortInfo = ReplyPortInfo.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Locally Assigned Keys message (Message ID 0x00BA) payload.
    /// </summary>
    /// <param name="reply">The reply containing the locally assigned keys payload.</param>
    private static void DecodeReplyLocallyAssignedKeys(HCIReply reply)
    {
        reply.LocallyAssignedKeys = ReplyLocallyAssignedKeys.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Assigned Keys message (Message ID 0x00E8) payload.
    /// </summary>
    /// <param name="reply">The reply containing the assigned keys payload.</param>
    private static void DecodeReplyAssignedKeys(HCIReply reply)
    {
        reply.AssignedKeys = ReplyAssignedKeys.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Card Info message (Message ID 0x00C4) payload.
    /// </summary>
    /// <param name="reply">The reply containing the card info payload.</param>
    private static void DecodeReplyCardInfo(HCIReply reply)
    {
        reply.CardInfo = ReplyCardInfo.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Conference Assignments message (Message ID 0x00C6) payload.
    /// </summary>
    /// <param name="reply">The reply containing the conference assignments payload.</param>
    private static void DecodeReplyConferenceAssignments(HCIReply reply)
    {
        reply.ConferenceAssignments = ReplyConferenceAssignments.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Set Config Multiple Keys message (Message ID 0x00CE) payload.
    /// </summary>
    /// <param name="reply">The reply containing the set config multiple keys payload.</param>
    private static void DecodeReplySetConfigMultipleKeys(HCIReply reply)
    {
        // Schema is determined by the HCIv2 schema byte already parsed
        byte schema = reply.Schema;
        reply.SetConfigMultipleKeys = ReplySetConfigMultipleKeys.Parse(reply.Payload, schema);
    }

    /// <summary>
    /// Decodes a Reply Forced Listen Edits message (Message ID 0x00CA) payload.
    /// </summary>
    /// <param name="reply">The reply containing the forced listen edits payload.</param>
    private static void DecodeReplyForcedListenEdits(HCIReply reply)
    {
        reply.ForcedListenEdits = ReplyForcedListenEdits.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Remote Key Actions message (Message ID 0x00EC) payload.
    /// </summary>
    /// <param name="reply">The reply containing the remote key actions payload.</param>
    private static void DecodeReplyRemoteKeyActions(HCIReply reply)
    {
        reply.RemoteKeyActions = ReplyRemoteKeyActions.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Remote Key Action Status message (Message ID 0x00EE) payload.
    /// </summary>
    /// <param name="reply">The reply containing the remote key action status payload.</param>
    private static void DecodeReplyRemoteKeyActionStatus(HCIReply reply)
    {
        reply.RemoteKeyActionStatus = ReplyRemoteKeyActionStatus.Parse(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply VOX Threshold Status message (Message ID 0x004A) payload.
    /// </summary>
    /// <param name="reply">The reply containing the VOX threshold status payload.</param>
    private static void DecodeReplyVoxThresholdStatus(HCIReply reply)
    {
        reply.VoxThresholdStatus = ReplyVoxThresholdStatus.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Beltpack Status or Reply Panel Status message (Message ID 0x004C).
    /// Differentiates between the two based on entry size:
    /// - Beltpack Status: 8 bytes per entry
    /// - Panel Status: 4 bytes per entry
    /// </summary>
    /// <param name="reply">The reply containing the status payload.</param>
    private static void DecodeReplyBeltpackOrPanelStatus(HCIReply reply)
    {
        var payload = reply.Payload;
        if (payload == null || payload.Length < 7)
        {
            return;
        }

        int offset = 0;

        // Check for HCIv2 marker (0xAB 0xBA 0xCE 0xDE)
        if (payload[0] == 0xAB && payload[1] == 0xBA &&
            payload[2] == 0xCE && payload[3] == 0xDE)
        {
            offset = 4;
        }

        // Skip schema byte
        offset++;

        // Read entry count (2 bytes, big-endian)
        if (offset + 2 > payload.Length)
        {
            return;
        }

        ushort entryCount = (ushort)((payload[offset] << 8) | payload[offset + 1]);
        offset += 2;

        if (entryCount == 0)
        {
            // No entries - default to panel status (arbitrary choice)
            reply.PanelStatus = ReplyPanelStatus.Parse(payload);
            return;
        }

        // Calculate remaining bytes after header
        int remainingBytes = payload.Length - offset;

        // Determine entry size based on payload length
        // Beltpack: 8 bytes per entry, Panel: 4 bytes per entry
        int bytesPerEntry = remainingBytes / entryCount;

        if (bytesPerEntry >= 8)
        {
            // Beltpack Status (8 bytes per entry)
            reply.BeltpackStatus = ReplyBeltpackStatus.Decode(payload);
        }
        else
        {
            // Panel Status (4 bytes per entry)
            reply.PanelStatus = ReplyPanelStatus.Parse(payload);
        }
    }

    /// <summary>
    /// Decodes a Reply Beltpack Status message (Message ID 0x004C) payload.
    /// </summary>
    /// <param name="reply">The reply containing the beltpack status payload.</param>
    private static void DecodeReplyBeltpackStatus(HCIReply reply)
    {
        reply.BeltpackStatus = ReplyBeltpackStatus.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Rack Properties message (Message ID 0x002C) payload.
    /// Supports Config Bank Reply (Sub Message ID 5), Rack State Get Reply (Sub Message ID 9),
    /// and Rack Configuration Status Reply (Sub Message ID 13).
    /// </summary>
    /// <param name="reply">The reply containing the rack properties payload.</param>
    private static void DecodeReplyRackProperties(HCIReply reply)
    {
        var payload = reply.Payload;
        if (payload == null || payload.Length < 6)
        {
            return;
        }

        int offset = 0;

        // Check for HCIv2 marker (0xAB 0xBA 0xCE 0xDE)
        if (payload[0] == 0xAB && payload[1] == 0xBA &&
            payload[2] == 0xCE && payload[3] == 0xDE)
        {
            offset = 4;
        }

        // Skip schema byte
        offset++;

        if (offset >= payload.Length)
        {
            return;
        }

        // Read first byte to determine Sub Message ID
        byte firstByte = payload[offset];

        // Check if this is a 1-byte Sub ID (12 or 13 for Rack Configuration Status)
        // or a 2-byte Sub ID (other rack property messages)
        if (firstByte == (byte)RackPropertySubMessageId.RackConfigurationStatusRequest ||
            firstByte == (byte)RackPropertySubMessageId.RackConfigurationStatusReply)
        {
            // 1-byte Sub ID
            switch ((RackPropertySubMessageId)firstByte)
            {
                case RackPropertySubMessageId.RackConfigurationStatusReply:
                    reply.RackConfigurationStatus = ReplyRackConfigurationStatus.Decode(payload);
                    break;
            }
        }
        else
        {
            // 2-byte Sub ID (big-endian)
            if (offset + 2 > payload.Length)
            {
                return;
            }

            ushort subMessageId = (ushort)((payload[offset] << 8) | payload[offset + 1]);

            switch ((RackPropertySubMessageId)subMessageId)
            {
                case RackPropertySubMessageId.ConfigBankReply:
                    reply.RackPropertiesConfigBank = ReplyRackPropertiesConfigBank.Decode(payload);
                    break;

                case RackPropertySubMessageId.RackStateGetReply:
                    reply.RackPropertiesRackState = ReplyRackPropertiesRackState.Decode(payload);
                    break;

                default:
                    // Unknown sub message type - try to decode as config bank for backwards compatibility
                    reply.RackPropertiesConfigBank = ReplyRackPropertiesConfigBank.Decode(payload);
                    break;
            }
        }
    }

    /// <summary>
    /// Decodes a Reply Audio Monitor Actions message (Message ID 0x0010) payload.
    /// </summary>
    /// <param name="reply">The reply containing the audio monitor actions payload.</param>
    private static void DecodeReplyAudioMonitorActions(HCIReply reply)
    {
        reply.AudioMonitorActions = ReplyAudioMonitorActions.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Telephony Client State message (Message ID 0x011E) payload.
    /// </summary>
    /// <param name="reply">The reply containing the telephony client state payload.</param>
    private static void DecodeReplyTelephonyClientState(HCIReply reply)
    {
        reply.TelephonyClientState = ReplyTelephonyClientState.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Telephony Client Disconnect message (Message ID 0x0123) payload.
    /// </summary>
    /// <param name="reply">The reply containing the telephony client disconnect payload.</param>
    private static void DecodeReplyTelephonyClientDisconnect(HCIReply reply)
    {
        reply.TelephonyClientDisconnect = ReplyTelephonyClientDisconnect.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Telephony Client Disconnect Outgoing message (Message ID 0x0125) payload.
    /// </summary>
    /// <param name="reply">The reply containing the telephony client disconnect outgoing payload.</param>
    private static void DecodeReplyTelephonyClientDisconnectOutgoing(HCIReply reply)
    {
        reply.TelephonyClientDisconnectOutgoing = ReplyTelephonyClientDisconnectOutgoing.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Get Proxy Indication State message (Message ID 0x0137) payload.
    /// </summary>
    /// <param name="reply">The reply containing the proxy indication state payload.</param>
    private static void DecodeReplyGetProxyIndicationState(HCIReply reply)
    {
        reply.ProxyIndicationState = ReplyGetProxyIndicationState.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Set Proxy Indication State message (Message ID 0x0139) payload.
    /// </summary>
    /// <param name="reply">The reply containing the set proxy indication state payload.</param>
    private static void DecodeReplySetProxyIndicationState(HCIReply reply)
    {
        reply.SetProxyIndicationState = ReplySetProxyIndicationState.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Get Proxy Display Data message (Message ID 0x013B) payload.
    /// </summary>
    /// <param name="reply">The reply containing the proxy display data payload.</param>
    private static void DecodeReplyGetProxyDisplayData(HCIReply reply)
    {
        reply.ProxyDisplayData = ReplyGetProxyDisplayData.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Set Proxy Display Data message (Message ID 0x013D) payload.
    /// </summary>
    /// <param name="reply">The reply containing the set proxy display data payload.</param>
    private static void DecodeReplySetProxyDisplayData(HCIReply reply)
    {
        reply.SetProxyDisplayData = ReplySetProxyDisplayData.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Panel Keys Public State message (Message ID 0x0141) payload.
    /// </summary>
    /// <param name="reply">The reply containing the panel keys public state payload.</param>
    private static void DecodeReplyPanelKeysPublicState(HCIReply reply)
    {
        reply.PanelKeysPublicState = ReplyPanelKeysPublicState.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Panel Keys Status Auto Updates message (Message ID 0x013F) payload.
    /// </summary>
    /// <param name="reply">The reply containing the panel keys status auto updates payload.</param>
    private static void DecodeReplyPanelKeysStatusAutoUpdates(HCIReply reply)
    {
        reply.PanelKeysStatusAutoUpdates = ReplyPanelKeysStatusAutoUpdates.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Panel Keys Public Set State message (Message ID 0x0156) payload.
    /// </summary>
    /// <param name="reply">The reply containing the panel keys public set state payload.</param>
    private static void DecodeReplyPanelKeysPublicSetState(HCIReply reply)
    {
        reply.PanelKeysPublicSetState = ReplyPanelKeysPublicSetState.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Telephony Key Status message (Message ID 0x015C) payload.
    /// </summary>
    /// <param name="reply">The reply containing the telephony key status payload.</param>
    private static void DecodeReplyTelephonyKeyStatus(HCIReply reply)
    {
        reply.TelephonyKeyStatus = ReplyTelephonyKeyStatus.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Set Panel Audio Front End State Reply message (Message ID 0x0162) payload.
    /// </summary>
    /// <param name="reply">The reply containing the set panel audio front end state payload.</param>
    private static void DecodeReplySetPanelAudioFrontEndState(HCIReply reply)
    {
        reply.SetPanelAudioFrontEndState = ReplySetPanelAudioFrontEndState.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Get Panel Audio Front End State Reply message (Message ID 0x0164) payload.
    /// </summary>
    /// <param name="reply">The reply containing the get panel audio front end state payload.</param>
    private static void DecodeReplyGetPanelAudioFrontEndState(HCIReply reply)
    {
        reply.GetPanelAudioFrontEndState = ReplyGetPanelAudioFrontEndState.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Telephony Key Status Enable message (Message ID 0x015B) payload.
    /// </summary>
    /// <param name="reply">The reply containing the telephony key status enable payload.</param>
    private static void DecodeReplyTelephonyKeyStatusEnable(HCIReply reply)
    {
        reply.TelephonyKeyStatusEnable = ReplyTelephonyKeyStatusEnable.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply System Crosspoint message (Message ID 0x00C8) payload.
    /// </summary>
    /// <param name="reply">The reply containing the system crosspoint payload.</param>
    private static void DecodeReplySystemCrosspoint(HCIReply reply)
    {
        reply.SystemCrosspoint = ReplySystemCrosspoint.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Panel Keys Unlatch All message (Message ID 0x014F) payload.
    /// </summary>
    /// <param name="reply">The reply containing the panel keys unlatch all payload.</param>
    private static void DecodeReplyPanelKeysUnlatchAll(HCIReply reply)
    {
        reply.PanelKeysUnlatchAll = ReplyPanelKeysUnlatchAll.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Panel Discovery, Reply IP Panel List, or Reply IP Panel Settings Assign message (Message ID 0x00F8) payload.
    /// Differentiates between them based on Sub ID in the payload.
    /// Sub ID 1 = Panel Discovery Reply, Sub ID 3 = Panel Configuration Reply, Sub ID 9 = IP Panel List Reply.
    /// </summary>
    /// <param name="reply">The reply containing the panel discovery, IP panel list, or settings assign payload.</param>
    private static void DecodeReplyPanelDiscoveryOrIPPanelList(HCIReply reply)
    {
        // Need at least 6 bytes: Protocol Tag (4) + Schema (1) + Sub ID (1)
        if (reply.Payload == null || reply.Payload.Length < 6)
        {
            reply.PanelDiscovery = ReplyPanelDiscovery.Decode(reply.Payload ?? Array.Empty<byte>());
            return;
        }

        // Sub ID is at offset 5 (after 4-byte protocol tag and 1-byte schema)
        byte subId = reply.Payload[5];

        switch (subId)
        {
            case ReplyIPPanelList.SubIdPanelCacheReply: // Sub ID 9
                reply.IPPanelList = ReplyIPPanelList.Decode(reply.Payload);
                break;
            case ReplyIPPanelSettingsAssign.SubIdPanelConfigurationReply: // Sub ID 3
                reply.IPPanelSettingsAssign = ReplyIPPanelSettingsAssign.Decode(reply.Payload);
                break;
            default: // Sub ID 1 or others
                reply.PanelDiscovery = ReplyPanelDiscovery.Decode(reply.Payload);
                break;
        }
    }

    /// <summary>
    /// Decodes a Reply Panel Shift Page Action message (Message ID 0x0154) payload.
    /// </summary>
    /// <param name="reply">The reply containing the panel shift page action payload.</param>
    private static void DecodeReplyPanelShiftPageAction(HCIReply reply)
    {
        reply.PanelShiftPageAction = ReplyPanelShiftPageAction.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Key Group Status message (Message ID 0x00FC) payload.
    /// </summary>
    /// <param name="reply">The reply containing the key group status payload.</param>
    private static void DecodeReplyKeyGroupStatus(HCIReply reply)
    {
        reply.KeyGroupStatus = ReplyKeyGroupStatus.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Frame Status message (Message ID 0x0062) payload.
    /// </summary>
    /// <param name="reply">The reply containing the frame status payload.</param>
    private static void DecodeReplyFrameStatus(HCIReply reply)
    {
        reply.FrameStatus = ReplyFrameStatus.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Xpt and Level Status message (Message ID 0x0097) payload.
    /// </summary>
    /// <param name="reply">The reply containing the xpt and level status payload.</param>
    private static void DecodeReplyXptAndLevelStatus(HCIReply reply)
    {
        reply.XptAndLevelStatus = ReplyXptAndLevelStatus.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Trunk Usage Statistics message (Message ID 0x0171) payload.
    /// </summary>
    /// <param name="reply">The reply containing the trunk usage statistics payload.</param>
    private static void DecodeReplyTrunkUsageStatistics(HCIReply reply)
    {
        reply.TrunkUsageStatistics = ReplyTrunkUsageStatistics.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply IPA Card Redundancy Switch message (Message ID 0x0183) payload.
    /// </summary>
    /// <param name="reply">The reply containing the IPA card redundancy switch payload.</param>
    private static void DecodeReplyIpaCardRedundancySwitch(HCIReply reply)
    {
        reply.IpaCardRedundancySwitch = ReplyIpaCardRedundancySwitch.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Macro Panel Keys Public State message (Message ID 0x0173) payload.
    /// </summary>
    /// <param name="reply">The reply containing the macro panel keys public state payload.</param>
    private static void DecodeReplyMacroPanelKeysPublicState(HCIReply reply)
    {
        reply.MacroPanelKeysPublicState = ReplyMacroPanelKeysPublicState.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Alt Text Set message (Message ID 0x0181) payload.
    /// </summary>
    /// <param name="reply">The reply containing the alt text set payload.</param>
    private static void DecodeReplyAltTextSet(HCIReply reply)
    {
        reply.AltTextSet = ReplyAltTextSet.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Assigned Keys (With Labels) message (Message ID 0x017D) payload.
    /// </summary>
    /// <param name="reply">The reply containing the assigned keys with labels payload.</param>
    private static void DecodeReplyAssignedKeysWithLabels(HCIReply reply)
    {
        reply.AssignedKeysWithLabels = ReplyAssignedKeysWithLabels.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Role State Set message (Message ID 0x0187) payload.
    /// </summary>
    /// <param name="reply">The reply containing the role state set response payload.</param>
    private static void DecodeReplyRoleStateSet(HCIReply reply)
    {
        reply.RoleStateSet = ReplyRoleStateSet.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Network Redundancy Endpoint Status message (Message ID 0x0189) payload.
    /// </summary>
    /// <param name="reply">The reply containing the network redundancy endpoint status payload.</param>
    private static void DecodeReplyNetworkRedundancyEndpointStatus(HCIReply reply)
    {
        reply.NetworkRedundancyEndpointStatus = ReplyNetworkRedundancyEndpointStatus.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Network Redundancy Card Status message (Message ID 0x018B) payload.
    /// </summary>
    /// <param name="reply">The reply containing the network redundancy card status payload.</param>
    private static void DecodeReplyNetworkRedundancyCardStatus(HCIReply reply)
    {
        reply.NetworkRedundancyCardStatus = ReplyNetworkRedundancyCardStatus.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Panel Connection Management Action message (Message ID 0x018F) payload.
    /// </summary>
    /// <param name="reply">The reply containing the panel connection management action response payload.</param>
    private static void DecodeReplyPanelConnectionManagementAction(HCIReply reply)
    {
        reply.PanelConnectionManagementAction = ReplyPanelConnectionManagementAction.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Beltpack Information message (Message ID 0x0102) payload.
    /// </summary>
    /// <param name="reply">The reply containing the beltpack information payload.</param>
    private static void DecodeReplyBeltpackInformation(HCIReply reply)
    {
        reply.BeltpackInformation = ReplyBeltpackInformation.Decode(reply.Payload);
    }

    /// <summary>
    /// Decodes a Reply Beltpack Delete message (Message ID 0x0196) payload.
    /// </summary>
    /// <param name="reply">The reply containing the beltpack delete response payload.</param>
    private static void DecodeReplyBeltpackDelete(HCIReply reply)
    {
        reply.BeltpackDelete = ReplyBeltpackDelete.Decode(reply.Payload);
    }
}
