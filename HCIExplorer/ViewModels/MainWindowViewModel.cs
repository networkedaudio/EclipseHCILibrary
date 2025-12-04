using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Models;
using HCIExplorer.Services;

namespace HCIExplorer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _hostAddress = "192.168.1.100";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ConnectButtonText))]
    private bool _isConnected;
    
    [ObservableProperty]
    private string _connectionStatus = "Disconnected";
    
    public string ConnectButtonText => IsConnected ? "Disconnect" : "Connect";
    
    [ObservableProperty]
    private ObservableCollection<LogEntry> _logEntries;
    
    [ObservableProperty]
    private bool _showRequests = true;
    
    [ObservableProperty]
    private bool _showResponses = true;
    
    [ObservableProperty]
    private bool _showDebug = true;
    
    [ObservableProperty]
    private bool _showErrors = true;
    
    [ObservableProperty]
    private bool _autoScroll = true;
    
    [ObservableProperty]
    private ObservableCollection<LogEntry> _filteredLogEntries = new();
    
    // Tab ViewModels
    [ObservableProperty]
    private SystemRequestsViewModel _systemRequests;
    
    [ObservableProperty]
    private PanelRequestsViewModel _panelRequests;
    
    [ObservableProperty]
    private CrosspointRequestsViewModel _crosspointRequests;
    
    [ObservableProperty]
    private ConferenceRequestsViewModel _conferenceRequests;
    
    [ObservableProperty]
    private TelephonyRequestsViewModel _telephonyRequests;
    
    [ObservableProperty]
    private CardRequestsViewModel _cardRequests;
    
    [ObservableProperty]
    private BeltpackRequestsViewModel _beltpackRequests;
    
    [ObservableProperty]
    private MiscRequestsViewModel _miscRequests;
    
    private readonly HCIConnectionService _connectionService;
    private readonly LogService _logService;
    
    public MainWindowViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
        _logService = LogService.Instance;
        _logEntries = _logService.LogEntries;
        _filteredLogEntries = new ObservableCollection<LogEntry>();
        
        // Subscribe to log entry changes
        _logEntries.CollectionChanged += OnLogEntriesChanged;
        
        // Initialize tab view models
        _systemRequests = new SystemRequestsViewModel();
        _panelRequests = new PanelRequestsViewModel();
        _crosspointRequests = new CrosspointRequestsViewModel();
        _conferenceRequests = new ConferenceRequestsViewModel();
        _telephonyRequests = new TelephonyRequestsViewModel();
        _cardRequests = new CardRequestsViewModel();
        _beltpackRequests = new BeltpackRequestsViewModel();
        _miscRequests = new MiscRequestsViewModel();
        
        // Subscribe to connection changes
        _connectionService.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(HCIConnectionService.IsConnected))
                IsConnected = _connectionService.IsConnected;
            if (e.PropertyName == nameof(HCIConnectionService.ConnectionStatus))
                ConnectionStatus = _connectionService.ConnectionStatus;
        };
    }
    
    private void OnLogEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (LogEntry entry in e.NewItems)
            {
                if (ShouldShowEntry(entry))
                {
                    FilteredLogEntries.Add(entry);
                }
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            RefreshFilteredEntries();
        }
    }
    
    private bool ShouldShowEntry(LogEntry entry)
    {
        return entry.Type switch
        {
            LogEntryType.Request => ShowRequests,
            LogEntryType.Response => ShowResponses,
            LogEntryType.Debug => ShowDebug,
            LogEntryType.Error => ShowErrors,
            _ => true
        };
    }
    
    private void RefreshFilteredEntries()
    {
        FilteredLogEntries.Clear();
        foreach (var entry in LogEntries)
        {
            if (ShouldShowEntry(entry))
            {
                FilteredLogEntries.Add(entry);
            }
        }
    }
    
    [RelayCommand]
    private async Task ConnectAsync()
    {
        if (IsConnected)
        {
            await _connectionService.DisconnectAsync();
        }
        else
        {
            await _connectionService.ConnectAsync(HostAddress);
        }
    }
    
    [RelayCommand]
    private void ClearLog()
    {
        _logService.Clear();
    }
    
    partial void OnShowRequestsChanged(bool value)
    {
        _logService.ShowRequests = value;
        RefreshFilteredEntries();
    }
    
    partial void OnShowResponsesChanged(bool value)
    {
        _logService.ShowResponses = value;
        RefreshFilteredEntries();
    }
    
    partial void OnShowDebugChanged(bool value)
    {
        _logService.ShowDebug = value;
        RefreshFilteredEntries();
    }
    
    partial void OnShowErrorsChanged(bool value)
    {
        _logService.ShowErrors = value;
        RefreshFilteredEntries();
    }
    
    partial void OnAutoScrollChanged(bool value)
    {
        _logService.AutoScroll = value;
    }
}
