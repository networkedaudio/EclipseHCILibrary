using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Services;
using HCILibrary.Enums;
using HCILibrary.HCIRequests;

namespace HCIExplorer.ViewModels;

public partial class SystemRequestsViewModel : ViewModelBase
{
    private readonly HCIConnectionService _connectionService;
    
    public SystemRequestsViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
    }
    
    [RelayCommand]
    private async Task RequestSystemStatusAsync()
    {
        var request = new RequestSystemStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestFrameStatusAsync()
    {
        var request = new RequestFrameStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestRackConfigurationAsync()
    {
        var request = new RequestRackConfigurationStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestRoleStateAsync()
    {
        var request = new RequestRoleStateRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestSystemMessagesAsync()
    {
        var request = new RequestSystemMessagesRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestActionsStatusAsync()
    {
        var request = new RequestActionsStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [ObservableProperty]
    private CpuResetType _selectedResetType = CpuResetType.Red;
    
    [RelayCommand]
    private async Task RequestResetAsync()
    {
        var request = new RequestCpuResetRequest(SelectedResetType);
        await _connectionService.SendRequestAsync(request);
    }
    
    [ObservableProperty]
    private DateTime _systemTime = DateTime.Now;
    
    [RelayCommand]
    private async Task SetSystemTimeAsync()
    {
        var request = new RequestSetSystemTimeRequest(SystemTime);
        await _connectionService.SendRequestAsync(request);
    }
}
