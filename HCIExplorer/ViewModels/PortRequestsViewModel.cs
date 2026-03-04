using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Services;
using HCILibrary.HCIRequests;
using HCILibrary.HCIResponses;
using HCILibrary.Models;

namespace HCIExplorer.ViewModels;

public partial class PortRequestsViewModel : ViewModelBase
{
    private readonly HCIConnectionService _connectionService;

    [ObservableProperty]
    private int _slot = 1;

    // -- Panel Status (bulk port list) --
    [ObservableProperty]
    private ObservableCollection<PanelStatus> _panelStatuses = new();

    [ObservableProperty]
    private string _lastPanelStatusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasPanelStatuses;

    // -- Port Info (detailed per-slot) --
    [ObservableProperty]
    private ObservableCollection<PortInfo> _portInfoEntries = new();

    [ObservableProperty]
    private string _lastPortInfoMessage = string.Empty;

    [ObservableProperty]
    private bool _hasPortInfo;

    // -- Beltpack Status (0x004C) --
    [ObservableProperty]
    private ObservableCollection<BeltpackStatusEntry> _beltpackStatuses = new();

    [ObservableProperty]
    private string _lastBeltpackStatusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasBeltpackStatuses;

    // -- Peripheral Info --
    [ObservableProperty]
    private ObservableCollection<PeripheralInfoEntry> _peripheralInfoEntries = new();

    [ObservableProperty]
    private string _lastPeripheralInfoMessage = string.Empty;

    [ObservableProperty]
    private bool _hasPeripheralInfo;

    public PortRequestsViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
        _connectionService.ReplyReceived += OnMessageReceived;
    }

    private void OnMessageReceived(object? sender, HCIReply reply)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (reply.PanelStatus is { } panelStatus)
            {
                PanelStatuses.Clear();
                foreach (var panel in panelStatus.Panels)
                {
                    PanelStatuses.Add(panel);
                }
                HasPanelStatuses = PanelStatuses.Count > 0;
                LastPanelStatusMessage = $"Received {panelStatus.Panels.Count} port(s) at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"Panel Status: {panelStatus.Panels.Count} ports");
            }

            if (reply.PortInfo is { } portInfo)
            {
                PortInfoEntries.Clear();
                foreach (var port in portInfo.Ports)
                {
                    PortInfoEntries.Add(port);
                }
                HasPortInfo = PortInfoEntries.Count > 0;
                LastPortInfoMessage = $"Slot {portInfo.SlotNumber}: {portInfo.NumberPorts} port(s) at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"Port Info: Slot={portInfo.SlotNumber}, {portInfo.NumberPorts} ports");
            }

            if (reply.BeltpackStatus is { } beltpackStatus)
            {
                BeltpackStatuses.Clear();
                foreach (var bp in beltpackStatus.Entries)
                {
                    BeltpackStatuses.Add(bp);
                }
                HasBeltpackStatuses = BeltpackStatuses.Count > 0;
                LastBeltpackStatusMessage = $"Received {beltpackStatus.Entries.Count} beltpack(s) at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"Beltpack Status: {beltpackStatus.Entries.Count} entries");
            }

            if (reply.PeripheralInfo is { } peripheralInfo)
            {
                PeripheralInfoEntries.Clear();
                foreach (var entry in peripheralInfo.Entries)
                {
                    PeripheralInfoEntries.Add(entry);
                }
                HasPeripheralInfo = PeripheralInfoEntries.Count > 0;
                LastPeripheralInfoMessage = $"Slot {peripheralInfo.RequestedSlotNumber}: {peripheralInfo.Count} peripheral(s) at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"Peripheral Info: Slot={peripheralInfo.RequestedSlotNumber}, {peripheralInfo.Count} entries");
            }
        });
    }

    [RelayCommand]
    private async Task RequestPanelStatusAsync()
    {
        var request = new RequestPanelStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }

    [RelayCommand]
    private async Task RequestPortInfoAsync()
    {
        var request = new RequestPortInfoRequest((ushort)Slot);
        await _connectionService.SendRequestAsync(request);
    }

    [RelayCommand]
    private async Task RequestPeripheralInfoAsync()
    {
        var request = new RequestPeripheralInfoRequest((byte)Slot);
        await _connectionService.SendRequestAsync(request);
    }

    [RelayCommand]
    private async Task RequestWirelessPeripheralInfoAsync()
    {
        var request = new RequestPeripheralInfoRequest(RequestPeripheralInfoRequest.WirelessDeviceSlot);
        await _connectionService.SendRequestAsync(request);
    }
}
