using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Services;
using HCILibrary.HCIRequests;
using HCILibrary.HCIResponses;
using HCILibrary.Models;
using HCILibrary.Enums;

namespace HCIExplorer.ViewModels;

public partial class PanelRequestsViewModel : ViewModelBase
{
    private readonly HCIConnectionService _connectionService;

    [ObservableProperty]
    private int _slot = 1;

    [ObservableProperty]
    private int _portOffset = 0;

    [ObservableProperty]
    private int _panelPort = 1;

    [ObservableProperty]
    private int _keyNumber = 1;

    [ObservableProperty]
    private int _pageNumber = 1;

    [ObservableProperty]
    private ObservableCollection<PanelStatus> _panelStatuses = new();

    [ObservableProperty]
    private string _lastPanelStatusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasPanelStatuses;

    [ObservableProperty]
    private string _lastPanelDiscoveryMessage = string.Empty;

    [ObservableProperty]
    private bool _hasPanelDiscovery;

    [ObservableProperty]
    private ObservableCollection<IPPanelEntry> _ipPanelEntries = new();

    [ObservableProperty]
    private string _lastIPPanelListMessage = string.Empty;

    [ObservableProperty]
    private bool _hasIPPanelList;

    public PanelRequestsViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
        _connectionService.ReplyReceived += OnReplyReceived;
    }

    private void OnReplyReceived(object? sender, HCIReply reply)
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
                LastPanelStatusMessage = $"Received {panelStatus.Panels.Count} panel(s) at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"Panel Status: {panelStatus.Panels.Count} panels — " +
                    string.Join(", ", panelStatus.Panels.Select(p => $"{p.PanelNumber}:{p.State}")));
            }

            if (reply.PanelDiscovery is { } discovery)
            {
                HasPanelDiscovery = true;
                LastPanelDiscoveryMessage = $"Discovery reply received at {DateTime.Now:HH:mm:ss} (Schema={discovery.ProtocolSchema}, SubId={discovery.SubId})";

                LogService.Instance.LogDebug(
                    $"Panel Discovery: Schema={discovery.ProtocolSchema}, SubId={discovery.SubId}");
            }

            if (reply.IPPanelList is { } ipPanelList)
            {
                // Accumulate entries across multiple messages (protocol may split across messages)
                if (IpPanelEntries.Count == 0 || IpPanelEntries.Count >= ipPanelList.TotalCount)
                {
                    IpPanelEntries.Clear();
                }
                foreach (var entry in ipPanelList.Entries)
                {
                    IpPanelEntries.Add(entry);
                }
                HasIPPanelList = IpPanelEntries.Count > 0;
                LastIPPanelListMessage = $"Received {IpPanelEntries.Count}/{ipPanelList.TotalCount} panel(s) at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"IP Panel List: {IpPanelEntries.Count}/{ipPanelList.TotalCount} entries");
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
    private async Task RequestPanelKeysStatusAsync()
    {
        var request = new RequestPanelKeysStatusRequest((byte)Slot, (byte)PortOffset);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestPanelDiscoveryAsync()
    {
        var request = new RequestPanelDiscoveryRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestAssignedKeysAsync()
    {
        var request = new RequestAssignedKeysRequest((byte)Slot, (byte)PortOffset, AssignedKeysSchema.Schema1);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestAssignedKeysWithLabelsAsync()
    {
        var request = new RequestAssignedKeysWithLabelsRequest((byte)Slot, (byte)PortOffset);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestLocallyAssignedKeysAsync()
    {
        var request = new RequestLocallyAssignedKeysRequest((byte)Slot, (byte)PortOffset);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestPanelKeysPublicGetStateAsync()
    {
        var request = new RequestPanelKeysPublicGetStateRequest((ushort)PanelPort);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestIPPanelListAsync()
    {
        var request = new RequestIPPanelListRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestPanelKeysUnlatchAllAsync()
    {
        var request = new RequestPanelKeysUnlatchAllRequest((byte)Slot, (byte)PortOffset);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestShiftPageAsync()
    {
        var request = new RequestPanelShiftPageActionRequest(PanelShiftPageActionType.SetCurrentPage);
        request.AddEntry((byte)Slot, (byte)PortOffset, (byte)PageNumber);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestAltTextStateAsync()
    {
        var request = new RequestAltTextStateRequest((ushort)PanelPort);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestGetPanelAudioFrontEndStateAsync()
    {
        var request = new RequestGetPanelAudioFrontEndStateRequest((ushort)PanelPort);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestMacroPanelKeysPublicStateAsync()
    {
        var request = new RequestMacroPanelKeysPublicStateRequest((ushort)PanelPort);
        await _connectionService.SendRequestAsync(request);
    }
}
