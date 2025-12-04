using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Services;
using HCILibrary.HCIRequests;
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
    
    public PanelRequestsViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
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
