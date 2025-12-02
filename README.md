# HCILibrary

A C# .NET 8 library for HCI protocol communication over TCP connections.

## Features

- **Automatic Port Failover**: Attempts connection on ports 52020 through 52001, automatically moving to the next port on failure
- **Message Buffering**: Buffers incoming data and extracts complete messages based on start (`5A 0F`) and end (`2E 8D`) markers
- **Protocol Support**: Handles both HCIv1 and HCIv2 message formats
- **Rate-Limited Request Queue**: Manages outgoing requests with configurable rate limiting
- **Priority Messages**: Supports urgent messages that bypass the normal queue order
- **Async/Await Pattern**: Fully asynchronous API for non-blocking operations

## Project Structure

```
HCILibrary/
├── Enums/                   # Protocol enumerations
│   ├── HCIMessageID.cs      # Message type identifiers
│   ├── HCIVersion.cs        # HCIv1/HCIv2 version enum
│   ├── PanelType.cs         # Panel type definitions
│   ├── PanelState.cs        # Panel state values
│   ├── CardType.cs          # Card type definitions
│   ├── CardHealth.cs        # Card health status
│   └── ...                  # Additional protocol enums
├── Models/
│   ├── HCIFlags.cs          # Flag byte parser (E, M, U, G, S, N)
│   ├── HCIReply.cs          # Decoded response message
│   └── HCIRequest.cs        # Base class for requests
├── HCIRequests/             # Request implementations
│   └── Request*.cs          # Individual request classes
├── HCIResponses/            # Response model classes
│   └── Reply*.cs            # Individual reply classes
├── Helpers/                 # Helper/utility classes
├── HCIConnection.cs         # TCP connection manager
├── HCIRequestQueue.cs       # Rate-limited message queue
├── HCIResponse.cs           # Message decoder
└── Program.cs               # Example usage
```

## Message Format

| Field | Size | Description |
|-------|------|-------------|
| Start Marker | 2 bytes | `0x5A 0x0F` |
| Length | 2 bytes | Total message length (big-endian) |
| Message ID | 2 bytes | Message type identifier (big-endian) |
| Flags | 1 byte | Bit flags (E, M, U, G, S, N) |
| HCIv2 Marker | 4 bytes | `0xAB 0xBA 0xCE 0xDE` (optional) |
| Protocol Indicator | 1 byte | Additional protocol info (HCIv2 only) |
| Payload | Variable | Message-specific data |
| End Marker | 2 bytes | `0x2E 0x8D` |

## Implemented Requests

The following requests are implemented in this library:

### System & Status Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestSystemMessagesRequest` | 2 | `0x0002` | Request system messages from the matrix |
| `RequestSystemStatusRequest` | 3 | `0x0003` | Request overall system status |
| `RequestPanelStatusRequest` | 5 | `0x0005` | Request status of panels connected to the matrix |
| `RequestActionsStatusRequest` | 15 | `0x000F` | Request status of pending actions |
| `RequestSetSystemTimeRequest` | 68 | `0x0044` | Set the system time on the config card |
| `RequestCpuResetRequest` | 41 | `0x0029` | Request a CPU reset with specified options |

### Crosspoint Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestCrosspointStatusRequest` | 13 | `0x000D` | Request crosspoint status between ports |
| `RequestCrosspointActionsRequest` | 17 | `0x0011` | Set crosspoint actions (talk/listen) |
| `RequestCrosspointLevelActionsRequest` | 38 | `0x0026` | Set crosspoint level adjustments |
| `RequestCrosspointLevelStatusRequest` | 39 | `0x0027` | Request crosspoint level status |
| `RequestSystemCrosspointRequest` | 201 | `0x00C9` | Request system crosspoints (remote routes) |

### Conference Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestConferenceActionsRequest` | 17 | `0x0011` | Add/remove conference members |
| `RequestConferenceStatusRequest` | 19 | `0x0013` | Request conference membership status |
| `RequestConferenceFixedGroupMembersEditsRequest` | 197 | `0x00C5` | Request locally edited fixed group members |

### Level Control Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestInputLevelActionsRequest` | 32 | `0x0020` | Set input level adjustments |
| `RequestInputLevelStatusRequest` | 33 | `0x0021` | Request input level status |
| `RequestOutputLevelActionsRequest` | 35 | `0x0023` | Set output level adjustments |
| `RequestOutputLevelStatusRequest` | 36 | `0x0024` | Request output level status |
| `RequestVoxThresholdStatusRequest` | 74 | `0x004A` | Request VOX threshold levels |
| `RequestVoxThresholdSetRequest` | 72 | `0x0048` | Set VOX threshold levels |

### EHX Control Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestEhxControlActionsRequest` | 17 | `0x0011` | Execute EHX control actions |
| `RequestEhxControlCardStatusRequest` | 21 | `0x0015` | Request EHX control card status |

### GPIO/SFO Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestGpioSfoStatusRequest` | 23 | `0x0017` | Request GPIO/SFO pin status |

### IFB Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestIfbStatusRequest` | 61 | `0x003D` | Request IFB attribute information |
| `RequestIfbSetRequest` | 63 | `0x003F` | Set, add, or delete IFB properties |

### Alias Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestUnicodeAliasAddRequest` | 244 | `0x00F4` | Add a Unicode alias to a port |
| `RequestUnicodeAliasListRequest` | 246 | `0x00F6` | Request list of Unicode aliases |
| `RequestAliasDeleteRequest` | 132 | `0x0084` | Delete an alias from a port |

### Entity & Peripheral Info Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestEntityInfoRequest` | 175 | `0x00AF` | Request entity info (conferences, groups, IFBs) |
| `RequestPeripheralInfoRequest` | 247 | `0x00F7` | Request peripheral version info (antennas, beltpacks) |

### Panel & Key Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestPanelKeysStatusRequest` | 177 | `0x00B1` | Request key latch status on a panel |
| `RequestPanelKeysActionRequest` | 179 | `0x00B3` | Set key states on a panel |
| `RequestLocallyAssignedKeysRequest` | 185 | `0x00B9` | Request locally assigned key configuration |
| `RequestAssignedKeysRequest` | 231 | `0x00E7` | Request all assigned key configuration |
| `RequestSetConfigMultipleKeysRequest` | 205 | `0x00CD` | Set config for multiple keys with advanced properties |
| `RequestPanelKeysStatusAutoUpdatesRequest` | 318 | `0x013E` | Enable/disable auto updates for panel key status |
| `RequestPanelKeysPublicGetStateRequest` | 320 | `0x0140` | Request public key latch state for panels |
| `RequestPanelKeysPublicSetStateRequest` | 341 | `0x0155` | Set key states (press/release) for panels |
| `RequestPanelKeysUnlatchAllRequest` | 334 | `0x014E` | Unlatch all keys on a specified panel |
| `RequestPanelShiftPageActionRequest` | 339 | `0x0153` | Get or set current page of a panel |
| `RequestSetPanelAudioFrontEndStateRequest` | 353 | `0x0161` | Set panel audio front end state (mic/speaker/sidetone) |
| `RequestGetPanelAudioFrontEndStateRequest` | 355 | `0x0163` | Get panel audio front end state |

### Port & Card Info Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestPortInfoRequest` | 183 | `0x00B7` | Request port type and info for a card |
| `RequestCardInfoRequest` | 195 | `0x00C3` | Request card info and health status |

### Forced Listen Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestForcedListenEditsRequest` | 201 | `0x00C9` | Request all forced listen edits |
| `RequestForcedListenActionsRequest` | 225 | `0x00E1` | Set forced listen crosspoints |

### Remote Key Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestRemoteKeyActionsRequest` | 235 | `0x00EB` | Remotely assign keys on a panel |
| `RequestRemoteKeyActionStatusRequest` | 237 | `0x00ED` | Request remote key assignment status |

### VoIP Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestVoIPStatusRequest` | 247 | `0x00F7` | Request VoIP channel status (IVC32/E-IPA) |

### IP Panel Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestPanelDiscoveryRequest` | 247 | `0x00F7` | Request IP panel discovery (Sub ID 0) |
| `RequestIPPanelListRequest` | 247 | `0x00F7` | Request discovered IP panels cache (Sub ID 8) |
| `RequestIPPanelSettingsAssignRequest` | 247 | `0x00F7` | Apply IP settings to V-Series panel (Sub ID 2) |

### Telephony Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestChangeTelephoneHookStateRequest` | 269 | `0x010D` | Toggle telephone hook state (TEL-14, SIP) |
| `RequestTelephonyClientGetStateRequest` | 285 | `0x011D` | Request telephony client port states |
| `RequestTelephonyClientDisconnectRequest` | 291 | `0x0123` | Disconnect incoming/connected call |
| `RequestTelephonyClientDisconnectOutgoingRequest` | 293 | `0x0125` | Disconnect outgoing SIP call |
| `RequestTelephonyClientDialInfoOutgoingRequest` | 295 | `0x0127` | Send dial info to SIP server |
| `RequestTelephonyClientDialInfoIncomingRequest` | 296 | `0x0128` | Send dial info from SIP server to matrix |
| `RequestTelephonyKeyStatusEnableRequest` | 346 | `0x015A` | Enable/disable telephony key status messages |

### Audio Monitor Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestAudioMonitorActionsRequest` | 17 | `0x0011` | Add/remove audio monitors in the matrix |

### Proxy Indication Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestGetProxyIndicationStateRequest` | 310 | `0x0136` | Request proxy key LED indication states for panels |
| `RequestSetProxyIndicationStateRequest` | 312 | `0x0138` | Set proxy key LED indication states for panel keys |
| `RequestGetProxyDisplayDataRequest` | 314 | `0x013A` | Request proxy display data (OLED text) for panels |
| `RequestSetProxyDisplayDataRequest` | 316 | `0x013C` | Set proxy display data (OLED text) for panel keys |

### SRecord & Rack Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestSRecordTransmissionRequest` | — | — | Transmit SRecord data to matrix |
| `RequestSRecordTransmissionInitiationRequest` | 69 | `0x0045` | Initiate SRecord transfer |
| `RequestRackPropertiesConfigBankRequest` | 44 | `0x002C` | Request rack configuration bank |
| `RequestRackPropertiesRackStateGetRequest` | 44 | `0x002C` | Request rack state information |
| `RequestRackConfigurationStatusRequest` | 44 | `0x002C` | Request rack configuration status (Sub ID 12) |

### Key Group Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestKeyGroupActionRequest` | 251 | `0x00FB` | Perform key group actions (Sub ID 1) |
| `RequestKeyGroupStatusRequest` | 251 | `0x00FB` | Request key group status (Sub ID 2) |
| `RequestAllKeyGroupStatusesRequest` | 251 | `0x00FB` | Request all key group statuses (Sub ID 3) |

### Frame & Card Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestFrameStatusRequest` | 97 | `0x0061` | Request frame status information |
| `RequestXptAndLevelStatusRequest` | 150 | `0x0096` | Request crosspoint and level status |
| `RequestIpaCardRedundancySwitchRequest` | 386 | `0x0182` | Request IPA card redundancy switch |

### Trunk Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestTrunkUsageStatisticsRequest` | 368 | `0x0170` | Request trunk usage statistics |

### Macro Panel Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestMacroPanelKeysPublicStateRequest` | 370 | `0x0172` | Request macro panel keys public state |

### Alt Text Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestAltTextStateRequest` | 376 | `0x0178` | Request alt text state for panel keys |
| `RequestAltTextSetRequest` | 378 | `0x017A` | Set alt text for panel keys |

### Assigned Keys Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestAssignedKeysRequest` | 231 | `0x00E7` | Request all assigned key configuration |
| `RequestAssignedKeysWithLabelsRequest` | 380 | `0x017C` | Request assigned keys with labels |

### Role State Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestRoleStateRequest` | 388 | `0x0184` | Request role state information |
| `RequestRoleStateSetRequest` | 390 | `0x0186` | Set role assignment state |

### Network Redundancy Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestNetworkRedundancyEndpointStatusRequest` | 392 | `0x0188` | Request network redundancy endpoint status |
| `RequestNetworkRedundancyCardStatusRequest` | 394 | `0x018A` | Request network redundancy card status |

### Panel Connection Management Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestPanelConnectionManagementActionRequest` | 398 | `0x018E` | Request panel connection actions (disconnect) |

### Beltpack Requests

| Request Class | Message ID | Hex | Description |
|---------------|------------|-----|-------------|
| `RequestBeltpackInformationRequest` | 257 | `0x0101` | Request beltpack information records |
| `RequestBeltpackAddRequest` | 403 | `0x0193` | Add a wireless beltpack to the matrix |
| `RequestBeltpackDeleteRequest` | 405 | `0x0195` | Delete a wireless beltpack from the matrix |

## Implemented Replies

The following reply messages are decoded by this library:

### System & Status Replies

| Reply Class | Message ID | Hex | Description |
|-------------|------------|-----|-------------|
| `ReplySystemCardStatus` | 4 | `0x0004` | System card status information |
| `ReplyPanelStatus` | 30 | `0x001E` | Panel status information |
| `ReplyFrameStatus` | 98 | `0x0062` | Frame status information |

### Crosspoint & Conference Replies

| Reply Class | Message ID | Hex | Description |
|-------------|------------|-----|-------------|
| `ReplyCrosspointStatus` | 14 | `0x000E` | Crosspoint status between ports |
| `ReplyConferenceStatus` | 20 | `0x0014` | Conference membership status |
| `ReplyXptAndLevelStatus` | 151 | `0x0097` | Crosspoint and level status |

### Key & Panel Replies

| Reply Class | Message ID | Hex | Description |
|-------------|------------|-----|-------------|
| `ReplyPanelKeysStatus` | 178 | `0x00B2` | Panel key latch status |
| `ReplyKeyGroupStatus` | 252 | `0x00FC` | Key group status |
| `ReplyPanelShiftPageAction` | 340 | `0x0154` | Panel page action result |
| `ReplyMacroPanelKeysPublicState` | 371 | `0x0173` | Macro panel keys public state |
| `ReplyAltTextSet` | 379 | `0x017B` | Alt text set confirmation |
| `ReplyAssignedKeysWithLabels` | 381 | `0x017D` | Assigned keys with labels |

### IP Panel Replies

| Reply Class | Message ID | Hex | Description |
|-------------|------------|-----|-------------|
| `ReplyIPPanelList` | 248 | `0x00F8` | Discovered IP panels list (Sub ID 9) |
| `ReplyIPPanelSettingsAssign` | 248 | `0x00F8` | IP panel settings assign result (Sub ID 3) |

### Trunk & Rack Replies

| Reply Class | Message ID | Hex | Description |
|-------------|------------|-----|-------------|
| `ReplyTrunkUsageStatistics` | 369 | `0x0171` | Trunk usage statistics |
| `ReplyRackConfigurationStatus` | 45 | `0x002D` | Rack configuration status (Sub ID 13) |

### Role State Replies

| Reply Class | Message ID | Hex | Description |
|-------------|------------|-----|-------------|
| `ReplyRoleState` | 389 | `0x0185` | Role state information |
| `ReplyRoleStateSet` | 391 | `0x0187` | Role state set result |

### Network Redundancy Replies

| Reply Class | Message ID | Hex | Description |
|-------------|------------|-----|-------------|
| `ReplyNetworkRedundancyEndpointStatus` | 393 | `0x0189` | Network redundancy endpoint status |
| `ReplyNetworkRedundancyCardStatus` | 395 | `0x018B` | Network redundancy card status |

### IPA Card Replies

| Reply Class | Message ID | Hex | Description |
|-------------|------------|-----|-------------|
| `ReplyIpaCardRedundancySwitch` | 387 | `0x0183` | IPA card redundancy switch result |

### Panel Connection Management Replies

| Reply Class | Message ID | Hex | Description |
|-------------|------------|-----|-------------|
| `ReplyPanelConnectionManagementAction` | 399 | `0x018F` | Panel connection action result |

### Beltpack Replies

| Reply Class | Message ID | Hex | Description |
|-------------|------------|-----|-------------|
| `ReplyBeltpackInformation` | 258 | `0x0102` | Beltpack information records |
| `ReplyBeltpackDelete` | 406 | `0x0196` | Beltpack delete result |

## Usage

### Basic Connection

```csharp
using HCILibrary;

// Create connection (will try ports 52020 down to 52001)
using var connection = new HCIConnection("192.168.1.100");

// Subscribe to received messages
connection.MessageReceived += (sender, reply) =>
{
    Console.WriteLine($"Received: {reply.MessageID}");
};

// Connect
if (await connection.ConnectAsync())
{
    Console.WriteLine($"Connected on port {connection.CurrentPort}");
    
    // Use connection.RequestQueue to send messages
}

await connection.DisconnectAsync();
```

### Sending Requests

```csharp
// Create a custom request by inheriting HCIRequest
public class MyRequest : HCIRequest
{
    public MyRequest() : base(HCIMessageID.MyMessageType) { }
    
    protected override byte[] GeneratePayload()
    {
        // Return your payload bytes
        return new byte[] { 0x01, 0x02, 0x03 };
    }
}

// Queue a normal request
connection.RequestQueue?.Enqueue(new MyRequest());

// Queue an urgent request (goes to front of queue)
connection.RequestQueue?.Enqueue(new MyRequest(), urgent: true);

// Queue and wait for response
var reply = await connection.RequestQueue?.EnqueueAndWaitAsync(
    new MyRequest { ExpectedReplyMessageID = HCIMessageID.MyResponseType }
);
```

### Example: Request Panel Status

```csharp
using HCILibrary;
using HCILibrary.HCIRequests;

// Request status of all panels
var request = new RequestPanelStatusRequest();
connection.RequestQueue?.Enqueue(request);
```

### Example: Set Crosspoint Action

```csharp
using HCILibrary.HCIRequests;

// Create crosspoint action to enable talk from port 1 to port 2
var action = new CrosspointAction
{
    SourcePort = 1,
    DestinationPort = 2,
    TalkEnable = true,
    ListenEnable = false
};

var request = new RequestCrosspointActionsRequest(new[] { action });
connection.RequestQueue?.Enqueue(request);
```

## Adding New Message Types

1. Add the message ID to `Enums/HCIMessageID.cs`
2. Create a request class in `HCIRequests/` inheriting from `HCIRequest`
3. Override `GeneratePayload()` to return the message-specific payload bytes
4. Create a response model in `HCIResponses/` if needed
5. Add a decoder in `HCIResponse.cs` under the appropriate version switch

## Building

```bash
dotnet build
```

## Running

```bash
dotnet run
```

## License

[Add your license here]
