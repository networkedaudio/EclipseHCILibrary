using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Services;
using HCILibrary.HCIRequests;
using HCILibrary.Enums;

namespace HCIExplorer.ViewModels;

public partial class TelephonyRequestsViewModel : ViewModelBase
{
    private readonly HCIConnectionService _connectionService;
    
    [ObservableProperty]
    private int _telephonyClientId = 1;
    
    [ObservableProperty]
    private string _dialNumber = "";
    
    public TelephonyRequestsViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
    }
    
    [RelayCommand]
    private async Task RequestTelephonyClientStateAsync()
    {
        var request = new RequestTelephonyClientGetStateRequest((ushort)TelephonyClientId);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestTelephonyDialInfoIncomingAsync()
    {
        var request = new RequestTelephonyClientDialInfoIncomingRequest((ushort)TelephonyClientId, DialNumber);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestTelephonyDialInfoOutgoingAsync()
    {
        var request = new RequestTelephonyClientDialInfoOutgoingRequest((ushort)TelephonyClientId, DialNumber);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestTelephonyKeyStatusEnableAsync()
    {
        var request = new RequestTelephonyKeyStatusEnableRequest(true);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestTelephonyKeyStatusDisableAsync()
    {
        var request = new RequestTelephonyKeyStatusEnableRequest(false);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestDisconnectTelephonyAsync()
    {
        var request = new RequestTelephonyClientDisconnectRequest((ushort)TelephonyClientId);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestVoIPStatusAsync()
    {
        var request = new RequestVoIPStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestTrunkUsageStatisticsAsync()
    {
        var request = new RequestTrunkUsageStatisticsRequest();
        await _connectionService.SendRequestAsync(request);
    }
}
