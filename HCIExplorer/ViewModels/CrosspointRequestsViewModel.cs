using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Services;
using HCILibrary.HCIRequests;
using HCILibrary.Enums;

namespace HCIExplorer.ViewModels;

public partial class CrosspointRequestsViewModel : ViewModelBase
{
    private readonly HCIConnectionService _connectionService;
    
    [ObservableProperty]
    private int _sourcePort = 1;
    
    [ObservableProperty]
    private int _destinationPort = 1;
    
    [ObservableProperty]
    private int _levelValue = 144; // Unity gain (0 dB)
    
    public CrosspointRequestsViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
    }
    
    [RelayCommand]
    private async Task RequestCrosspointStatusAsync()
    {
        var request = new RequestCrosspointStatusRequest();
        request.AddPort((ushort)SourcePort);
        request.AddPort((ushort)DestinationPort);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestCrosspointLevelStatusAsync()
    {
        var request = new RequestCrosspointLevelStatusRequest();
        request.AddDestinationPort((ushort)DestinationPort);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestSystemCrosspointAsync()
    {
        var request = new RequestSystemCrosspointRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestXptAndLevelStatusAsync()
    {
        var request = new RequestXptAndLevelStatusRequest((ushort)SourcePort, (ushort)DestinationPort);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task SetCrosspointTalkAsync()
    {
        var request = new RequestCrosspointActionsRequest();
        request.AddAction(true, (ushort)SourcePort, (ushort)DestinationPort, false, CrosspointPriority.Normal);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task ClearCrosspointTalkAsync()
    {
        var request = new RequestCrosspointActionsRequest();
        request.AddAction(false, (ushort)SourcePort, (ushort)DestinationPort, false, CrosspointPriority.Normal);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task SetCrosspointLevelAsync()
    {
        var request = new RequestCrosspointLevelActionsRequest();
        request.AddAction((ushort)DestinationPort, (ushort)SourcePort, (ushort)LevelValue);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestInputLevelStatusAsync()
    {
        var request = new RequestInputLevelStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestOutputLevelStatusAsync()
    {
        var request = new RequestOutputLevelStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestForcedListenEditsAsync()
    {
        var request = new RequestForcedListenEditsRequest();
        await _connectionService.SendRequestAsync(request);
    }
}
