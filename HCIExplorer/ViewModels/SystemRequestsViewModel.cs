using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Models;
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

    [ObservableProperty]
    private ObservableCollection<FrameStatusEntry> _frameStatusEntries = new();

    [ObservableProperty]
    private string _lastFrameStatusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasFrameStatus;

    [ObservableProperty]
    private ObservableCollection<FrameStatusEntry> _rackConfigEntries = new();

    [ObservableProperty]
    private string _lastRackConfigMessage = string.Empty;

    [ObservableProperty]
    private bool _hasRackConfig;

    [ObservableProperty]
    private ObservableCollection<FrameStatusEntry> _rackPropertiesEntries = new();

    [ObservableProperty]
    private string _lastRackPropertiesMessage = string.Empty;

    [ObservableProperty]
    private bool _hasRackProperties;

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

            if (reply.FrameStatus is { } frameStatus)
            {
                FrameStatusEntries.Clear();
                FrameStatusEntries.Add(new FrameStatusEntry { Name = "Protocol Schema", Status = frameStatus.ProtocolSchema.ToString() });
                FrameStatusEntries.Add(new FrameStatusEntry { Name = "CPU Temperature", Status = $"{frameStatus.CpuCardTemperature} °C" });
                FrameStatusEntries.Add(new FrameStatusEntry { Name = "External PSU 1", Status = frameStatus.IsExtPsu1Failed ? "FAILED" : "OK" });
                FrameStatusEntries.Add(new FrameStatusEntry { Name = "External PSU 2", Status = frameStatus.IsExtPsu2Failed ? "FAILED" : "OK" });
                FrameStatusEntries.Add(new FrameStatusEntry { Name = "Internal PSU 1", Status = frameStatus.IsIntPsu1Failed ? "FAILED" : "OK" });
                FrameStatusEntries.Add(new FrameStatusEntry { Name = "Internal PSU 2", Status = frameStatus.IsIntPsu2Failed ? "FAILED" : "OK" });
                FrameStatusEntries.Add(new FrameStatusEntry { Name = "Fan 1", Status = frameStatus.IsFan1Failed ? "FAILED" : "OK" });
                FrameStatusEntries.Add(new FrameStatusEntry { Name = "Fan 2", Status = frameStatus.IsFan2Failed ? "FAILED" : "OK" });
                FrameStatusEntries.Add(new FrameStatusEntry { Name = "Configuration", Status = frameStatus.IsConfigFailed ? "FAILED" : "OK" });
                FrameStatusEntries.Add(new FrameStatusEntry { Name = "External Alarm", Status = frameStatus.IsExtAlarmActive ? "ACTIVE" : "Inactive" });
                FrameStatusEntries.Add(new FrameStatusEntry { Name = "Over-Temperature", Status = frameStatus.IsOvertemp ? "ALARM" : "Normal" });
                FrameStatusEntries.Add(new FrameStatusEntry { Name = "PSU Status Raw", Status = $"0x{(ushort)frameStatus.PsuStatus:X4}" });
                HasFrameStatus = true;
                LastFrameStatusMessage = $"Frame status received at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"Frame Status: CPU Temp={frameStatus.CpuCardTemperature}°C, PSU=0x{(ushort)frameStatus.PsuStatus:X4}, Alarms={frameStatus.HasAnyAlarm}");
            }

            if (reply.RackConfigurationStatus is { } rackConfig)
            {
                RackConfigEntries.Clear();
                RackConfigEntries.Add(new FrameStatusEntry { Name = "Protocol Schema", Status = rackConfig.ProtocolSchema.ToString() });
                RackConfigEntries.Add(new FrameStatusEntry { Name = "Matrix ID", Status = rackConfig.MatrixId.ToString() });
                RackConfigEntries.Add(new FrameStatusEntry { Name = "System Identity", Status = rackConfig.SystemIdentityString });
                RackConfigEntries.Add(new FrameStatusEntry { Name = "Matrix Description", Status = rackConfig.MatrixDescriptionString });
                RackConfigEntries.Add(new FrameStatusEntry { Name = "Download Timestamp", Status = rackConfig.DownloadTimestamp?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A" });
                RackConfigEntries.Add(new FrameStatusEntry { Name = "Download Flags", Status = $"0x{rackConfig.DownloadFlags:X2}" });
                RackConfigEntries.Add(new FrameStatusEntry { Name = "Checksum", Status = $"0x{rackConfig.Checksum:X8}" });
                RackConfigEntries.Add(new FrameStatusEntry { Name = "Map Format", Status = BitConverter.ToString(rackConfig.MapFormat) });
                RackConfigEntries.Add(new FrameStatusEntry { Name = "HCI License", Status = rackConfig.HciLicense.ToString() });
                RackConfigEntries.Add(new FrameStatusEntry { Name = "HCI Extended License", Status = rackConfig.HciExtendedLicense.ToString() });
                RackConfigEntries.Add(new FrameStatusEntry { Name = "Virtual Client Users", Status = rackConfig.VirtualClientUsers.ToString() });
                RackConfigEntries.Add(new FrameStatusEntry { Name = "Dynam-EC Licenses", Status = rackConfig.DynamEcLicenses.ToString() });
                RackConfigEntries.Add(new FrameStatusEntry { Name = "Delta Lite Extension", Status = rackConfig.DeltaLiteExtensionLicense.ToString() });
                HasRackConfig = true;
                LastRackConfigMessage = $"Rack configuration received at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"Rack Config: Matrix={rackConfig.MatrixId}, Identity={rackConfig.SystemIdentityString}, Desc={rackConfig.MatrixDescriptionString}");
            }

            if (reply.RackPropertiesConfigBank is { } configBank)
            {
                RackPropertiesEntries.Clear();
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Type", Status = "Config Bank" });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Schema", Status = configBank.Schema.ToString() });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Sub Message ID", Status = configBank.SubMessageId.ToString() });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Success", Status = configBank.Success ? "Yes" : "No" });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Bank Number", Status = configBank.BankNumber.ToString() });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Default Map", Status = configBank.IsDefaultMap ? "Yes (embedded)" : "No" });
                HasRackProperties = true;
                LastRackPropertiesMessage = $"Config bank received at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug($"Rack Properties Config Bank: {configBank}");
            }

            if (reply.RackPropertiesRackState is { } rackState)
            {
                RackPropertiesEntries.Clear();
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Type", Status = "Rack State" });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Schema", Status = rackState.Schema.ToString() });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Sub Message ID", Status = rackState.SubMessageId.ToString() });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Current State", Status = rackState.CurrentState.ToString() });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Current State Info", Status = rackState.CurrentStateAdditionalInfo.ToString() });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Previous State", Status = rackState.PreviousState.ToString() });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Previous State Info", Status = rackState.PreviousStateAdditionalInfo.ToString() });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Ready", Status = rackState.IsReady ? "Yes" : "No" });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Downloading", Status = rackState.IsDownloading ? "Yes" : "No" });
                RackPropertiesEntries.Add(new FrameStatusEntry { Name = "Resetting", Status = rackState.IsResetting ? "Yes" : "No" });
                HasRackProperties = true;
                LastRackPropertiesMessage = $"Rack state received at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug($"Rack Properties Rack State: {rackState}");
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
