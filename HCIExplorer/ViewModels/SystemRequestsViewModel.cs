using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Services;
using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using HCILibrary.HCIResponses;
using HCILibrary.Models;

// Add an explicit using alias to resolve ambiguity for RequestActionsStatusRequest
using ActionsStatusRequest = HCILibrary.HCIRequests.RequestActionsStatusRequest;

namespace HCIExplorer.ViewModels;

public partial class SystemRequestsViewModel : ViewModelBase
{
    private readonly HCIConnectionService _connectionService;
    
    [ObservableProperty]
    private ObservableCollection<CardStatus> _systemCards = new();
    
    [ObservableProperty]
    private string _lastSystemStatusMessage = string.Empty;
    
    [ObservableProperty]
    private bool _hasSystemCards;
    
    public SystemRequestsViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
        _connectionService.ReplyReceived += OnReplyReceived;
    }
    
    private void OnReplyReceived(object? sender, HCIReply reply)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (reply.SystemCardStatus is { } status)
            {
                SystemCards.Clear();
                foreach (var card in status.Cards)
                {
                    SystemCards.Add(card);
                }
                HasSystemCards = SystemCards.Count > 0;
                LastSystemStatusMessage = $"Received {status.Count} card(s) at {DateTime.Now:HH:mm:ss}";
                
                LogService.Instance.LogDebug(
                    $"System Status: {status.Count} cards — " +
                    string.Join(", ", status.Cards.Select(c => $"{c.CardType}:{c.Condition}")));
            }
        });
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
        // Use the alias to resolve ambiguity
        var request = new ActionsStatusRequest();
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
