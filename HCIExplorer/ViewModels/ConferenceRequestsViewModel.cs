using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Services;
using HCILibrary.HCIRequests;
using HCILibrary.Enums;

namespace HCIExplorer.ViewModels;

public partial class ConferenceRequestsViewModel : ViewModelBase
{
    private readonly HCIConnectionService _connectionService;
    
    [ObservableProperty]
    private int _conferenceId = 1;
    
    [ObservableProperty]
    private int _memberPort = 1;
    
    [ObservableProperty]
    private int _keyGroupSystemNumber = 0;
    
    public ConferenceRequestsViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
    }
    
    [RelayCommand]
    private async Task RequestConferenceStatusAsync()
    {
        var request = new RequestConferenceStatusRequest((ushort)ConferenceId);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestAllConferencesAsync()
    {
        var request = new RequestConferenceStatusRequest(0); // 0 for all conferences
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestConferenceFixedGroupMembersAsync()
    {
        var request = new RequestConferenceFixedGroupMembersEditsRequest(ConferenceEditType.Conference);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestAllKeyGroupStatusesAsync()
    {
        var request = new RequestAllKeyGroupStatusesRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestKeyGroupStatusAsync()
    {
        var request = new RequestKeyGroupStatusRequest((byte)KeyGroupSystemNumber, (ushort)ConferenceId);
        await _connectionService.SendRequestAsync(request);
    }
}
