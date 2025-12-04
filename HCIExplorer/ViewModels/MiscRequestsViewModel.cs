using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Services;
using HCILibrary.HCIRequests;
using HCILibrary.Enums;

namespace HCIExplorer.ViewModels;

public partial class MiscRequestsViewModel : ViewModelBase
{
    private readonly HCIConnectionService _connectionService;
    
    [ObservableProperty]
    private int _entityId = 1;
    
    [ObservableProperty]
    private int _targetPort = 1;
    
    [ObservableProperty]
    private string _aliasText = "";
    
    [ObservableProperty]
    private int _matrixId = 0;
    
    [ObservableProperty]
    private int _ifbId = 1;
    
    [ObservableProperty]
    private int _monitorPort = 2;
    
    [ObservableProperty]
    private int _proxyPort = 1;
    
    [ObservableProperty]
    private int _proxyKey = 1;
    
    public MiscRequestsViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
    }
    
    [RelayCommand]
    private async Task RequestEntityInfoAsync()
    {
        var request = new RequestEntityInfoRequest(EntityInfoType.All);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestUnicodeAliasListAsync()
    {
        var request = new RequestUnicodeAliasListRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestUnicodeAliasAddAsync()
    {
        var request = new RequestUnicodeAliasAddRequest();
        // Add alias with dialcode bytes and text
        byte[] dialcode = new byte[] { 0x00, 0x00, (byte)(TargetPort >> 8), (byte)(TargetPort & 0xFF) };
        request.AddAlias(dialcode, AliasText);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestAliasDeleteAsync()
    {
        var request = new RequestAliasDeleteRequest();
        request.AddDialcode((ushort)TargetPort, 0x01, 0); // entityInstance, entityType, targetSystemNumber
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestIfbStatusAsync()
    {
        var request = new RequestIfbStatusRequest((byte)MatrixId, (ushort)IfbId, IfbAttributeType.All);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestVoxThresholdStatusAsync()
    {
        var request = new RequestVoxThresholdStatusRequest((byte)TargetPort);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestAudioMonitorActionsAsync()
    {
        var request = new RequestAudioMonitorActionsRequest();
        request.AddMonitor((ushort)MonitorPort, (ushort)TargetPort);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestGetProxyDisplayDataAsync()
    {
        var request = new RequestGetProxyDisplayDataRequest((ushort)ProxyPort);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestGetProxyIndicationStateAsync()
    {
        var request = new RequestGetProxyIndicationStateRequest((ushort)ProxyPort);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestRemoteKeyActionStatusAsync()
    {
        var request = new RequestRemoteKeyActionStatusRequest((ushort)TargetPort);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestRackPropertiesRackStateGetAsync()
    {
        var request = new RequestRackPropertiesRackStateGetRequest();
        await _connectionService.SendRequestAsync(request);
    }
}
