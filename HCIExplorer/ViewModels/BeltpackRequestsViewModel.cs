using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Services;
using HCILibrary.HCIRequests;
using HCILibrary.Enums;

namespace HCIExplorer.ViewModels;

public partial class BeltpackRequestsViewModel : ViewModelBase
{
    private readonly HCIConnectionService _connectionService;
    
    [ObservableProperty]
    private int _beltpackId = 1;
    
    [ObservableProperty]
    private string _beltpackLabel = "BP01";
    
    [ObservableProperty]
    private uint _serialNumber = 12345678;
    
    [ObservableProperty]
    private uint _pmid = 1;
    
    public BeltpackRequestsViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
    }
    
    [RelayCommand]
    private async Task RequestBeltpackInformationAsync()
    {
        // Schema 2 with HciAddedEntries
        var request = new RequestBeltpackInformationRequest(BeltpackRequestType.HciAddedEntries);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestAllBeltpacksAsync()
    {
        // Schema 1 - returns all entries
        var request = new RequestBeltpackInformationRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestBeltpackAddAsync()
    {
        // Use the static PoolMode factory method
        var request = RequestBeltpackAddRequest.PoolMode(SerialNumber, Pmid, BeltpackLabel);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestBeltpackDeleteAsync()
    {
        var request = new RequestBeltpackDeleteRequest((ushort)BeltpackId);
        await _connectionService.SendRequestAsync(request);
    }
}
