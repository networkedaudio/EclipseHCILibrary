using HCILibrary;
using HCILibrary.Discovery;
using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using HCILibrary.Models;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace HCITestConsole;

internal class Program
{
    private static HCIConnection? _connection;
    private static readonly ConcurrentQueue<HCIReply> _receivedReplies = new();
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("╔══════════════════════════════════════════════╗");
        Console.WriteLine("║          HCI Test Console v1.0              ║");
        Console.WriteLine("║    Eclipse HX Matrix Protocol Tester        ║");
        Console.WriteLine("╚══════════════════════════════════════════════╝");
        Console.WriteLine();

        string? ipAddress = await RunDiscoveryAndSelectAsync();

        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            Console.WriteLine("No matrix selected. Exiting.");
            return;
        }

        Console.WriteLine($"\nConnecting to {ipAddress}...");
        _connection = new HCIConnection(ipAddress);

        _connection.MessageReceived += OnMessageReceived;
        _connection.ConnectionStateChanged += (_, connected) =>
            Console.WriteLine(connected ? "  [Connected]" : "  [Disconnected]");
        _connection.ErrorOccurred += (_, ex) =>
            Console.WriteLine($"  [Error] {ex.Message}");

        bool connected = await _connection.ConnectAsync();

        if (!connected)
        {
            Console.WriteLine("Failed to connect to the matrix. Exiting.");
            _connection.Dispose();
            return;
        }

        Console.WriteLine($"Connected on port {_connection.CurrentPort}.");
        Console.WriteLine();

        await RunMainMenuAsync();

        await _connection.DisconnectAsync();
        _connection.Dispose();
        Console.WriteLine("Disconnected. Goodbye.");
    }

    private static async Task<string?> RunDiscoveryAndSelectAsync()
    {
        Console.WriteLine("Scanning for Eclipse HX matrices on the network (5 seconds)...");
        Console.WriteLine();

        var discovered = new List<MatrixSignature>();

        using var listener = new DiscoveryListener();

        listener.BroadcastReceived += (_, e) =>
        {
            if (e.ParsedData?.MatrixSignature != null)
            {
                var sig = e.ParsedData.MatrixSignature;
                // Avoid duplicates by IP address
                if (!discovered.Any(d => d.PrimaryAddress?.ToString() == sig.PrimaryAddress?.ToString()))
                {
                    discovered.Add(sig);
                    Console.WriteLine($"  Found: {sig}");
                }
            }
        };

        try
        {
            listener.Start();
            await Task.Delay(5000);
            await listener.StopAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Discovery error: {ex.Message}");
            Console.WriteLine("  (UDP port 42001 may be in use or unavailable)");
        }

        Console.WriteLine();

        if (discovered.Count > 0)
        {
            Console.WriteLine("Detected matrices:");
            for (int i = 0; i < discovered.Count; i++)
            {
                char letter = (char)('A' + i);
                var sig = discovered[i];
                Console.WriteLine($"  {letter}) {sig.FrameName} ({sig.Identity}) - {sig.MatrixType} @ {sig.PrimaryAddress}");
            }
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("No matrices detected via UDP broadcast.");
        }

        Console.Write("Enter a letter to select a matrix, or type an IP address: ");
        string? input = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(input))
            return null;

        // Check if it's a single letter selection
        if (input.Length == 1 && char.IsLetter(input[0]))
        {
            int index = char.ToUpper(input[0]) - 'A';
            if (index >= 0 && index < discovered.Count)
            {
                return discovered[index].PrimaryAddress?.ToString();
            }

            Console.WriteLine("Invalid selection.");
            return null;
        }

        // Assume it's an IP address
        if (System.Net.IPAddress.TryParse(input, out _))
        {
            return input;
        }

        Console.WriteLine("Invalid IP address.");
        return null;
    }

    private static void OnMessageReceived(object? sender, HCIReply reply)
    {
        _receivedReplies.Enqueue(reply);
    }

    private static async Task RunMainMenuAsync()
    {
        while (true)
        {
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║              HCI Message Menu                ║");
            Console.WriteLine("╠══════════════════════════════════════════════╣");
            Console.WriteLine("║  System                                     ║");
            Console.WriteLine("║   1) Request System Status                  ║");
            Console.WriteLine("║   2) Request Frame Status                   ║");
            Console.WriteLine("║   3) Request Rack Properties (Config Bank)  ║");
            Console.WriteLine("║   4) Request Rack Properties (Rack State)   ║");
            Console.WriteLine("║   5) Request Rack Configuration Status      ║");
            Console.WriteLine("║                                             ║");
            Console.WriteLine("║  Panels                                     ║");
            Console.WriteLine("║  10) Request Panel Status                   ║");
            Console.WriteLine("║  11) Request Panel Keys Status              ║");
            Console.WriteLine("║  12) Request Port Info                      ║");
            Console.WriteLine("║  13) Request Locally Assigned Keys          ║");
            Console.WriteLine("║  14) Request Assigned Keys                  ║");
            Console.WriteLine("║  15) Request Assigned Keys (With Labels)    ║");
            Console.WriteLine("║  16) Request Panel Discovery                ║");
            Console.WriteLine("║  17) Request IP Panel List                  ║");
            Console.WriteLine("║  18) Request Panel Shift Page Action (Get)  ║");
            Console.WriteLine("║  19) Request Panel Keys Public Get State    ║");
            Console.WriteLine("║                                             ║");
            Console.WriteLine("║  Cards                                      ║");
            Console.WriteLine("║  20) Request Card Info                      ║");
            Console.WriteLine("║  21) Request EHX Control Card Status        ║");
            Console.WriteLine("║  22) Request GPIO/SFO Status                ║");
            Console.WriteLine("║                                             ║");
            Console.WriteLine("║  Crosspoints & Levels                       ║");
            Console.WriteLine("║  30) Request Crosspoint Status              ║");
            Console.WriteLine("║  31) Request Crosspoint Level Status        ║");
            Console.WriteLine("║  32) Request Input Level Status             ║");
            Console.WriteLine("║  33) Request Output Level Status            ║");
            Console.WriteLine("║  34) Request Xpt and Level Status           ║");
            Console.WriteLine("║  35) Request System Crosspoint              ║");
            Console.WriteLine("║                                             ║");
            Console.WriteLine("║  Conferences & Entities                     ║");
            Console.WriteLine("║  40) Request Conference Status              ║");
            Console.WriteLine("║  41) Request Conference Members Edits       ║");
            Console.WriteLine("║  42) Request Entity Info                    ║");
            Console.WriteLine("║  43) Request Key Group Status               ║");
            Console.WriteLine("║                                             ║");
            Console.WriteLine("║  Conference Actions                         ║");
            Console.WriteLine("║ 100) Conference Status (by number)          ║");
            Console.WriteLine("║ 101) Add Port to Conference (Talk)          ║");
            Console.WriteLine("║ 102) Add Port to Conference (Listen)        ║");
            Console.WriteLine("║ 103) Remove Port from Conference (Talk)     ║");
            Console.WriteLine("║ 104) Remove Port from Conference (Listen)   ║");
            Console.WriteLine("║                                             ║");
            Console.WriteLine("║  IFB Actions                                ║");
            Console.WriteLine("║ 110) Request IFB Status                     ║");
            Console.WriteLine("║ 111) Set IFB Dim Level                      ║");
            Console.WriteLine("║ 112) Trigger IFB                            ║");
            Console.WriteLine("║ 113) Untrigger IFB                          ║");
            Console.WriteLine("║ 114) Add Source to IFB                      ║");
            Console.WriteLine("║ 115) Remove Source from IFB                 ║");
            Console.WriteLine("║                                             ║");
            Console.WriteLine("║  Wireless                                   ║");
            Console.WriteLine("║  50) Request VOX Threshold Status           ║");
            Console.WriteLine("║  51) Request Beltpack Information           ║");
            Console.WriteLine("║  52) Request Peripheral Info                ║");
            Console.WriteLine("║  53) Request VoIP Status                    ║");
            Console.WriteLine("║                                             ║");
            Console.WriteLine("║  Telephony                                  ║");
            Console.WriteLine("║  60) Request Telephony Client State         ║");
            Console.WriteLine("║                                             ║");
            Console.WriteLine("║  Proxy                                      ║");
            Console.WriteLine("║  70) Request Get Proxy Indication State     ║");
            Console.WriteLine("║  71) Request Get Proxy Display Data         ║");
            Console.WriteLine("║                                             ║");
            Console.WriteLine("║  Other                                      ║");
            Console.WriteLine("║  80) Request Role State                     ║");
            Console.WriteLine("║  81) Request Network Redundancy Endpoint    ║");
            Console.WriteLine("║  82) Request Network Redundancy Card        ║");
            Console.WriteLine("║  83) Request Remote Key Action Status       ║");
            Console.WriteLine("║  84) Request Forced Listen Edits            ║");
            Console.WriteLine("║  85) Request Trunk Usage Statistics         ║");
            Console.WriteLine("║  86) Request Macro Panel Keys Public State  ║");
            Console.WriteLine("║  87) Request Audio Monitor Actions          ║");
            Console.WriteLine("║  88) Request IPA Card Redundancy Switch     ║");
            Console.WriteLine("║                                             ║");
            Console.WriteLine("║  Utilities                                  ║");
            Console.WriteLine("║  90) Show received message log              ║");
            Console.WriteLine("║  91) Clear received message log             ║");
            Console.WriteLine("║                                             ║");
            Console.WriteLine("║   0) Quit                                   ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.Write("\nSelect option: ");

            string? choice = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(choice) || choice == "0")
                break;

            await HandleMenuChoiceAsync(choice);
        }
    }

    private static async Task HandleMenuChoiceAsync(string choice)
    {
        // Clear the reply queue before sending a new request
        while (_receivedReplies.TryDequeue(out _)) { }

        HCIRequest? request = null;

        try
        {
            switch (choice)
            {
                // System
                case "1":
                    request = new RequestSystemStatusRequest();
                    break;
                case "2":
                    request = new RequestFrameStatusRequest();
                    break;
                case "3":
                    request = new RequestRackPropertiesConfigBankRequest();
                    break;
                case "4":
                    request = new RequestRackPropertiesRackStateGetRequest();
                    break;
                case "5":
                    request = new RequestRackConfigurationStatusRequest();
                    break;

                // Panels
                case "10":
                    request = new RequestPanelStatusRequest();
                    break;
                case "11":
                    request = BuildPanelKeysStatusRequest();
                    break;
                case "12":
                    request = BuildPortInfoRequest();
                    break;
                case "13":
                    request = BuildLocallyAssignedKeysRequest();
                    break;
                case "14":
                    request = BuildAssignedKeysRequest();
                    break;
                case "15":
                    request = BuildAssignedKeysWithLabelsRequest();
                    break;
                case "16":
                    request = new RequestPanelDiscoveryRequest();
                    break;
                case "17":
                    request = new RequestIPPanelListRequest();
                    break;
                case "18":
                    request = BuildPanelShiftPageActionRequest();
                    break;
                case "19":
                    request = BuildPanelKeysPublicGetStateRequest();
                    break;

                // Cards
                case "20":
                    request = BuildCardInfoRequest();
                    break;
                case "21":
                    request = new RequestEhxControlCardStatusRequest();
                    break;
                case "22":
                    request = new RequestGpioSfoStatusRequest();
                    break;

                // Crosspoints & Levels
                case "30":
                    request = BuildCrosspointStatusRequest();
                    break;
                case "31":
                    request = BuildCrosspointLevelStatusRequest();
                    break;
                case "32":
                    request = new RequestInputLevelStatusRequest();
                    break;
                case "33":
                    request = new RequestOutputLevelStatusRequest();
                    break;
                case "34":
                    request = new RequestXptAndLevelStatusRequest();
                    break;
                case "35":
                    request = new RequestSystemCrosspointRequest();
                    break;

                // Conferences & Entities
                case "40":
                    request = new RequestConferenceStatusRequest();
                    break;
                case "41":
                    request = new RequestConferenceFixedGroupMembersEditsRequest();
                    break;
                case "42":
                    request = BuildEntityInfoRequest();
                    break;
                case "43":
                    request = BuildKeyGroupStatusRequest();
                    break;

                // Conference Actions
                case "100":
                    request = BuildConferenceStatusByNumberRequest();
                    break;
                case "101":
                    request = BuildConferenceActionRequest(isAdd: true, isTalk: true);
                    break;
                case "102":
                    request = BuildConferenceActionRequest(isAdd: true, isTalk: false);
                    break;
                case "103":
                    request = BuildConferenceActionRequest(isAdd: false, isTalk: true);
                    break;
                case "104":
                    request = BuildConferenceActionRequest(isAdd: false, isTalk: false);
                    break;

                // IFB Actions
                case "110":
                    request = BuildIfbStatusRequest();
                    break;
                case "111":
                    request = BuildIfbSetDimLevelRequest();
                    break;
                case "112":
                    request = BuildIfbTriggerRequest(triggered: true);
                    break;
                case "113":
                    request = BuildIfbTriggerRequest(triggered: false);
                    break;
                case "114":
                    request = BuildIfbSourceRequest(isAdd: true);
                    break;
                case "115":
                    request = BuildIfbSourceRequest(isAdd: false);
                    break;

                // Wireless
                case "50":
                    request = new RequestVoxThresholdStatusRequest();
                    break;
                case "51":
                    request = new RequestBeltpackInformationRequest();
                    break;
                case "52":
                    request = new RequestPeripheralInfoRequest();
                    break;
                case "53":
                    request = new RequestVoIPStatusRequest();
                    break;

                // Telephony
                case "60":
                    request = new RequestTelephonyClientGetStateRequest();
                    break;

                // Proxy
                case "70":
                    request = BuildGetProxyIndicationStateRequest();
                    break;
                case "71":
                    request = BuildGetProxyDisplayDataRequest();
                    break;

                // Other
                case "80":
                    request = BuildRoleStateRequest();
                    break;
                case "81":
                    request = new RequestNetworkRedundancyEndpointStatusRequest();
                    break;
                case "82":
                    request = new RequestNetworkRedundancyCardStatusRequest();
                    break;
                case "83":
                    request = new RequestRemoteKeyActionStatusRequest();
                    break;
                case "84":
                    request = new RequestForcedListenEditsRequest();
                    break;
                case "85":
                    request = new RequestTrunkUsageStatisticsRequest();
                    break;
                case "86":
                    request = BuildMacroPanelKeysPublicStateRequest();
                    break;
                case "87":
                    request = new RequestAudioMonitorActionsRequest();
                    break;
                case "88":
                    request = new RequestIpaCardRedundancySwitchRequest();
                    break;

                // Utilities
                case "90":
                    ShowReceivedLog();
                    return;
                case "91":
                    while (_receivedReplies.TryDequeue(out _)) { }
                    Console.WriteLine("Log cleared.");
                    return;

                default:
                    Console.WriteLine("Unknown option.");
                    return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error building request: {ex.Message}");
            return;
        }

        if (request == null)
        {
            Console.WriteLine("Request cancelled.");
            return;
        }

        await SendAndWaitForReplyAsync(request);
    }

    private static async Task SendAndWaitForReplyAsync(HCIRequest request)
    {
        if (_connection?.RequestQueue == null)
        {
            Console.WriteLine("Not connected or request queue unavailable.");
            return;
        }

        Console.WriteLine($"\nSending: {request.MessageID} (0x{(ushort)request.MessageID:X4})...");

        _connection.RequestQueue.Enqueue(request);

        // Wait up to 5 seconds for a reply
        Console.WriteLine("Waiting for reply (up to 5 seconds)...");

        var deadline = DateTime.UtcNow.AddSeconds(5);
        var replies = new List<HCIReply>();

        while (DateTime.UtcNow < deadline)
        {
            while (_receivedReplies.TryDequeue(out var reply))
            {
                replies.Add(reply);
            }

            if (replies.Count > 0)
            {
                // Wait a short extra period for continuation messages
                await Task.Delay(500);
                while (_receivedReplies.TryDequeue(out var reply))
                {
                    replies.Add(reply);
                }
                break;
            }

            await Task.Delay(100);
        }

        if (replies.Count == 0)
        {
            Console.WriteLine("  No reply received (timeout).");
        }
        else
        {
            foreach (var reply in replies)
            {
                PrintReply(reply);
            }
        }

        Console.WriteLine();
    }

    private static void PrintReply(HCIReply reply)
    {
        Console.WriteLine("┌─────────────────────────────────────────────");
        Console.WriteLine($"│ Message ID: {reply.MessageID} (0x{(ushort)reply.MessageID:X4})");
        Console.WriteLine($"│ Version:    {reply.Version}");
        Console.WriteLine($"│ Length:     {reply.MessageLength} bytes");
        Console.WriteLine($"│ Flags:      {reply.Flags}");

        if (reply.Version == HCIVersion.HCIv2)
            Console.WriteLine($"│ Schema:     {reply.Schema}");

        Console.WriteLine($"│ Payload:    {reply.Payload.Length} bytes");

        // Print the decoded data if available
        PrintDecodedData(reply);

        if (reply.Payload.Length > 0 && reply.Payload.Length <= 256)
        {
            Console.WriteLine($"│ Raw Payload: {BitConverter.ToString(reply.Payload)}");
        }
        else if (reply.Payload.Length > 256)
        {
            Console.WriteLine($"│ Raw Payload (first 256): {BitConverter.ToString(reply.Payload, 0, 256)}");
        }

        Console.WriteLine("└─────────────────────────────────────────────");
    }

    private static void PrintDecodedData(HCIReply reply)
    {
        try
        {
            if (reply.Event != null)
                PrintJson("Event", reply.Event);
            if (reply.SystemCardStatus != null)
                PrintJson("System Card Status", reply.SystemCardStatus);
            if (reply.PanelStatus != null)
                PrintJson("Panel Status", reply.PanelStatus);
            if (reply.CrosspointStatus != null)
                PrintJson("Crosspoint Status", reply.CrosspointStatus);
            if (reply.CrosspointLevelStatus != null)
                PrintJson("Crosspoint Level Status", reply.CrosspointLevelStatus);
            if (reply.ConferenceStatus != null)
                PrintJson("Conference Status", reply.ConferenceStatus);
            if (reply.EhxControlCardStatus != null)
                PrintJson("EHX Control Card Status", reply.EhxControlCardStatus);
            if (reply.GpioSfoStatus != null)
                PrintJson("GPIO/SFO Status", reply.GpioSfoStatus);
            if (reply.InputLevelStatus != null)
                PrintJson("Input Level Status", reply.InputLevelStatus);
            if (reply.OutputLevelStatus != null)
                PrintJson("Output Level Status", reply.OutputLevelStatus);
            if (reply.PanelKeysStatus != null)
                PrintJson("Panel Keys Status", reply.PanelKeysStatus);
            if (reply.PanelKeysActionStatus != null)
                PrintJson("Panel Keys Action Status", reply.PanelKeysActionStatus);
            if (reply.PortInfo != null)
                PrintJson("Port Info", reply.PortInfo);
            if (reply.LocallyAssignedKeys != null)
                PrintJson("Locally Assigned Keys", reply.LocallyAssignedKeys);
            if (reply.AssignedKeys != null)
                PrintJson("Assigned Keys", reply.AssignedKeys);
            if (reply.CardInfo != null)
                PrintJson("Card Info", reply.CardInfo);
            if (reply.PeripheralInfo != null)
                PrintJson("Peripheral Info", reply.PeripheralInfo);
            if (reply.ConferenceAssignments != null)
                PrintJson("Conference Assignments", reply.ConferenceAssignments);
            if (reply.SetConfigMultipleKeys != null)
                PrintJson("Set Config Multiple Keys", reply.SetConfigMultipleKeys);
            if (reply.ForcedListenEdits != null)
                PrintJson("Forced Listen Edits", reply.ForcedListenEdits);
            if (reply.RemoteKeyActions != null)
                PrintJson("Remote Key Actions", reply.RemoteKeyActions);
            if (reply.RemoteKeyActionStatus != null)
                PrintJson("Remote Key Action Status", reply.RemoteKeyActionStatus);
            if (reply.VoxThresholdStatus != null)
                PrintJson("VOX Threshold Status", reply.VoxThresholdStatus);
            if (reply.BeltpackStatus != null)
                PrintJson("Beltpack Status", reply.BeltpackStatus);
            if (reply.RackPropertiesConfigBank != null)
                PrintJson("Rack Properties Config Bank", reply.RackPropertiesConfigBank);
            if (reply.RackPropertiesRackState != null)
                PrintJson("Rack Properties Rack State", reply.RackPropertiesRackState);
            if (reply.RackConfigurationStatus != null)
                PrintJson("Rack Configuration Status", reply.RackConfigurationStatus);
            if (reply.AudioMonitorActions != null)
                PrintJson("Audio Monitor Actions", reply.AudioMonitorActions);
            if (reply.TelephonyClientState != null)
                PrintJson("Telephony Client State", reply.TelephonyClientState);
            if (reply.TelephonyClientDisconnect != null)
                PrintJson("Telephony Client Disconnect", reply.TelephonyClientDisconnect);
            if (reply.TelephonyClientDisconnectOutgoing != null)
                PrintJson("Telephony Client Disconnect Outgoing", reply.TelephonyClientDisconnectOutgoing);
            if (reply.ProxyIndicationState != null)
                PrintJson("Proxy Indication State", reply.ProxyIndicationState);
            if (reply.SetProxyIndicationState != null)
                PrintJson("Set Proxy Indication State", reply.SetProxyIndicationState);
            if (reply.ProxyDisplayData != null)
                PrintJson("Proxy Display Data", reply.ProxyDisplayData);
            if (reply.SetProxyDisplayData != null)
                PrintJson("Set Proxy Display Data", reply.SetProxyDisplayData);
            if (reply.PanelKeysPublicState != null)
                PrintJson("Panel Keys Public State", reply.PanelKeysPublicState);
            if (reply.PanelKeysStatusAutoUpdates != null)
                PrintJson("Panel Keys Status Auto Updates", reply.PanelKeysStatusAutoUpdates);
            if (reply.PanelKeysPublicSetState != null)
                PrintJson("Panel Keys Public Set State", reply.PanelKeysPublicSetState);
            if (reply.TelephonyKeyStatus != null)
                PrintJson("Telephony Key Status", reply.TelephonyKeyStatus);
            if (reply.TelephonyKeyStatusEnable != null)
                PrintJson("Telephony Key Status Enable", reply.TelephonyKeyStatusEnable);
            if (reply.UnicodeAliasStatus != null)
                PrintJson("Unicode Alias Status", reply.UnicodeAliasStatus);
            if (reply.AliasDeleteStatus != null)
                PrintJson("Alias Delete Status", reply.AliasDeleteStatus);
            if (reply.PanelDiscovery != null)
                PrintJson("Panel Discovery", reply.PanelDiscovery);
            if (reply.IPPanelList != null)
                PrintJson("IP Panel List", reply.IPPanelList);
            if (reply.IPPanelSettingsAssign != null)
                PrintJson("IP Panel Settings Assign", reply.IPPanelSettingsAssign);
            if (reply.PanelShiftPageAction != null)
                PrintJson("Panel Shift Page Action", reply.PanelShiftPageAction);
            if (reply.KeyGroupStatus != null)
                PrintJson("Key Group Status", reply.KeyGroupStatus);
            if (reply.FrameStatus != null)
                PrintJson("Frame Status", reply.FrameStatus);
            if (reply.XptAndLevelStatus != null)
                PrintJson("Xpt and Level Status", reply.XptAndLevelStatus);
            if (reply.TrunkUsageStatistics != null)
                PrintJson("Trunk Usage Statistics", reply.TrunkUsageStatistics);
            if (reply.IpaCardRedundancySwitch != null)
                PrintJson("IPA Card Redundancy Switch", reply.IpaCardRedundancySwitch);
            if (reply.MacroPanelKeysPublicState != null)
                PrintJson("Macro Panel Keys Public State", reply.MacroPanelKeysPublicState);
            if (reply.AltTextSet != null)
                PrintJson("Alt Text Set", reply.AltTextSet);
            if (reply.AssignedKeysWithLabels != null)
                PrintJson("Assigned Keys With Labels", reply.AssignedKeysWithLabels);
            if (reply.RoleStateSet != null)
                PrintJson("Role State Set", reply.RoleStateSet);
            if (reply.NetworkRedundancyEndpointStatus != null)
                PrintJson("Network Redundancy Endpoint Status", reply.NetworkRedundancyEndpointStatus);
            if (reply.NetworkRedundancyCardStatus != null)
                PrintJson("Network Redundancy Card Status", reply.NetworkRedundancyCardStatus);
            if (reply.PanelConnectionManagementAction != null)
                PrintJson("Panel Connection Management Action", reply.PanelConnectionManagementAction);
            if (reply.BeltpackInformation != null)
                PrintJson("Beltpack Information", reply.BeltpackInformation);
            if (reply.BeltpackDelete != null)
                PrintJson("Beltpack Delete", reply.BeltpackDelete);
            if (reply.SystemCrosspoint != null)
                PrintJson("System Crosspoint", reply.SystemCrosspoint);
            if (reply.PanelKeysUnlatchAll != null)
                PrintJson("Panel Keys Unlatch All", reply.PanelKeysUnlatchAll);
            if (reply.SetPanelAudioFrontEndState != null)
                PrintJson("Set Panel Audio Front End State", reply.SetPanelAudioFrontEndState);
            if (reply.GetPanelAudioFrontEndState != null)
                PrintJson("Get Panel Audio Front End State", reply.GetPanelAudioFrontEndState);
            if (reply.EntityInfo != null)
                PrintJson("Entity Info", reply.EntityInfo);
            if (reply.IfbStatus != null)
                PrintJson("IFB Status", reply.IfbStatus);
            if (reply.RoleState != null)
                PrintJson("Role State", reply.RoleState);
            if (reply.VoIPStatus != null)
                PrintJson("VoIP Status", reply.VoIPStatus);
            if (reply.AltTextState != null)
                PrintJson("Alt Text State", reply.AltTextState);
            if (reply.BeltpackAdd != null)
                PrintJson("Beltpack Add", reply.BeltpackAdd);
        }
        catch
        {
            // Serialization errors are non-fatal; raw payload is always shown
        }
    }

    private static void PrintJson(string label, object data)
    {
        Console.WriteLine($"│ ── {label} ──");
        try
        {
            string json = JsonSerializer.Serialize(data, data.GetType(), _jsonOptions);
            foreach (var line in json.Split('\n'))
            {
                Console.WriteLine($"│   {line.TrimEnd()}");
            }
        }
        catch
        {
            Console.WriteLine($"│   (could not serialize)");
        }
    }

    private static void ShowReceivedLog()
    {
        var snapshot = _receivedReplies.ToArray();
        if (snapshot.Length == 0)
        {
            Console.WriteLine("No messages in log.");
            return;
        }

        Console.WriteLine($"\n── Received Message Log ({snapshot.Length} messages) ──");
        foreach (var reply in snapshot)
        {
            PrintReply(reply);
        }
    }

    // ── Request builders that prompt for parameters ──

    private static HCIRequest? BuildPanelKeysStatusRequest()
    {
        byte slot = PromptByte("Enter card slot number");
        byte portOffset = PromptByte("Enter port offset");
        return new RequestPanelKeysStatusRequest(slot, portOffset);
    }

    private static HCIRequest? BuildPortInfoRequest()
    {
        ushort slot = PromptUShort("Enter slot number");
        return new RequestPortInfoRequest(slot);
    }

    private static HCIRequest? BuildLocallyAssignedKeysRequest()
    {
        byte slot = PromptByte("Enter card slot number");
        byte port = PromptByte("Enter port offset");
        return new RequestLocallyAssignedKeysRequest(slot, port);
    }

    private static HCIRequest? BuildAssignedKeysRequest()
    {
        byte slot = PromptByte("Enter card slot number");
        byte port = PromptByte("Enter port offset");
        return new RequestAssignedKeysRequest(slot, port);
    }

    private static HCIRequest? BuildAssignedKeysWithLabelsRequest()
    {
        byte slot = PromptByte("Enter card slot number");
        byte port = PromptByte("Enter port offset");
        return new RequestAssignedKeysWithLabelsRequest(slot, port);
    }

    private static HCIRequest? BuildPanelShiftPageActionRequest()
    {
        byte slot = PromptByte("Enter card slot number");
        byte portOffset = PromptByte("Enter port offset");
        var request = new RequestPanelShiftPageActionRequest();
        request.AddEntry(slot, portOffset);
        return request;
    }

    private static HCIRequest? BuildPanelKeysPublicGetStateRequest()
    {
        ushort port = PromptUShort("Enter port number (0-1023), or 65535 for all");
        return new RequestPanelKeysPublicGetStateRequest(port);
    }

    private static HCIRequest? BuildCardInfoRequest()
    {
        byte slot = PromptByte("Enter slot number (0-255)");
        return new RequestCardInfoRequest(slot);
    }

    private static HCIRequest? BuildCrosspointStatusRequest()
    {
        Console.Write("Enter port number(s) separated by commas (0-1023): ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input)) return null;

        var request = new RequestCrosspointStatusRequest();
        foreach (var part in input.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            if (ushort.TryParse(part.Trim(), out ushort port))
            {
                request.AddPort(port);
            }
        }
        return request.Ports.Count > 0 ? request : null;
    }

    private static HCIRequest? BuildCrosspointLevelStatusRequest()
    {
        Console.Write("Enter destination port number(s) separated by commas (0-1023): ");
        string? input = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(input)) return null;

        var request = new RequestCrosspointLevelStatusRequest();
        foreach (var part in input.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            if (ushort.TryParse(part.Trim(), out ushort port))
            {
                request.AddDestinationPort(port);
            }
        }
        return request.DestinationPorts.Count > 0 ? request : null;
    }

    private static HCIRequest? BuildEntityInfoRequest()
    {
        return new RequestEntityInfoRequest();
    }

    private static HCIRequest? BuildKeyGroupStatusRequest()
    {
        return new RequestKeyGroupStatusRequest();
    }

    private static HCIRequest? BuildGetProxyIndicationStateRequest()
    {
        ushort port = PromptUShort("Enter port number (0-1023), or 65535 for all");
        return new RequestGetProxyIndicationStateRequest(port);
    }

    private static HCIRequest? BuildGetProxyDisplayDataRequest()
    {
        ushort port = PromptUShort("Enter port number (0-1023), or 65535 for all");
        return new RequestGetProxyDisplayDataRequest(port);
    }

    private static HCIRequest? BuildRoleStateRequest()
    {
        ushort port = PromptUShort("Enter port number (0-1023), or 65535 for all");
        return new RequestRoleStateRequest(port);
    }

    private static HCIRequest? BuildMacroPanelKeysPublicStateRequest()
    {
        ushort port = PromptUShort("Enter port number (0-1023)");
        return new RequestMacroPanelKeysPublicStateRequest(port);
    }

    // ── Conference Actions builders ──

    private static HCIRequest? BuildConferenceStatusByNumberRequest()
    {
        ushort confNum = PromptUShort("Enter conference number (0-1023)");
        return new RequestConferenceStatusRequest(confNum);
    }

    private static HCIRequest? BuildConferenceActionRequest(bool isAdd, bool isTalk)
    {
        string action = isAdd ? "Add" : "Remove";
        string direction = isTalk ? "talk" : "listen";
        Console.WriteLine($"  [{action} port as {direction}]");

        ushort port = PromptUShort("Enter port number (0-1023)");
        ushort confNum = PromptUShort("Enter conference number (0-1023)");

        var request = new HCILibrary.HCIRequests.RequestConferenceActionsRequest();
        request.AddAction(isAdd, port, isTalk, confNum);
        return request;
    }

    // ── IFB Actions builders ──

    private static HCIRequest? BuildIfbStatusRequest()
    {
        byte matrixId = PromptByte("Enter matrix identifier");
        ushort ifbId = PromptUShort("Enter IFB identifier");

        Console.WriteLine("Attribute types: 0=IntLevel, 1=DimLevel, 2=Priority,");
        Console.WriteLine("  3=ActiveCallers, 4=Sources, 5=Destination,");
        Console.WriteLine("  6=Returns, 7=PotentialCallers, 255=All");
        byte attrType = PromptByte("Enter attribute type (255 for all)");

        return new RequestIfbStatusRequest(matrixId, ifbId, (IfbAttributeType)attrType);
    }

    private static HCIRequest? BuildIfbSetDimLevelRequest()
    {
        ushort ifbId = PromptUShort("Enter IFB identifier");

        Console.WriteLine("Dim levels: 0=0dB, 1=-3dB, 2=-6dB, 3=-9dB, 4=-12dB,");
        Console.WriteLine("  5=-15dB, 6=-18dB, 7=-21dB, 8=-24dB, 9=-27dB, 10=-30dB, 15=Off");
        byte level = PromptByte("Enter dim level");

        return RequestIfbSetRequest.CreateSetDimLevel(ifbId, (IfbDimLevel)level);
    }

    private static HCIRequest? BuildIfbTriggerRequest(bool triggered)
    {
        ushort ifbId = PromptUShort("Enter IFB identifier");
        return RequestIfbSetRequest.CreateSetIntLevel(ifbId, triggered);
    }

    private static HCIRequest? BuildIfbSourceRequest(bool isAdd)
    {
        ushort ifbId = PromptUShort("Enter IFB identifier");

        Console.WriteLine("Dial code format: 4 bytes as hex (e.g. 01020003)");
        Console.WriteLine("  Byte 1: System Number, Byte 2: Entity Type,");
        Console.WriteLine("  Bytes 3-4: Instance");
        Console.Write("Enter dial code (hex): ");
        string? input = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(input) || !uint.TryParse(input,
            System.Globalization.NumberStyles.HexNumber, null, out uint dialCode))
        {
            Console.WriteLine("Invalid dial code.");
            return null;
        }

        return isAdd
            ? RequestIfbSetRequest.CreateAddSource(ifbId, dialCode)
            : RequestIfbSetRequest.CreateDeleteSource(ifbId, dialCode);
    }

    // ── Input helpers ──

    private static ushort PromptUShort(string prompt)
    {
        Console.Write($"{prompt}: ");
        string? input = Console.ReadLine()?.Trim();
        if (ushort.TryParse(input, out ushort value))
            return value;
        return 0;
    }

    private static byte PromptByte(string prompt)
    {
        Console.Write($"{prompt}: ");
        string? input = Console.ReadLine()?.Trim();
        if (byte.TryParse(input, out byte value))
            return value;
        return 0;
    }
}
