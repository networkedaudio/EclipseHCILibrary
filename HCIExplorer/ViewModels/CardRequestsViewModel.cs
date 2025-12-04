using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Services;
using HCILibrary.HCIRequests;
using HCILibrary.Enums;

namespace HCIExplorer.ViewModels;

public partial class CardRequestsViewModel : ViewModelBase
{
    private readonly HCIConnectionService _connectionService;
    
    [ObservableProperty]
    private int _slot = 1;
    
    [ObservableProperty]
    private int _port = 0;
    
    [ObservableProperty]
    private int _cardId = 1;
    
    public CardRequestsViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
    }
    
    [RelayCommand]
    private async Task RequestCardInfoAsync()
    {
        var request = new RequestCardInfoRequest((byte)Slot);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestAllCardInfoAsync()
    {
        var request = new RequestCardInfoRequest();
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
    private async Task RequestEhxControlCardStatusAsync()
    {
        var request = new RequestEhxControlCardStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestNetworkRedundancyCardStatusAsync()
    {
        var request = new RequestNetworkRedundancyCardStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestNetworkRedundancyEndpointStatusAsync()
    {
        var request = new RequestNetworkRedundancyEndpointStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestGpioSfoStatusAsync()
    {
        var request = new RequestGpioSfoStatusRequest((byte)Slot);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestIpaCardRedundancySwitchAsync()
    {
        var request = new RequestIpaCardRedundancySwitchRequest();
        await _connectionService.SendRequestAsync(request);
    }
}
