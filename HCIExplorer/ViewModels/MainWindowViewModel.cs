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
    private string _hostAddress = MatrixDiscoveryService.DefaultHostAddress;
    
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
    
    // Discovery
    [ObservableProperty]
    private ObservableCollection<DiscoveredMatrix> _discoveredMatrices;
    
    [ObservableProperty]
    private DiscoveredMatrix? _selectedDiscoveredMatrix;
    
    [ObservableProperty]
    private bool _isDiscoveryActive;
    
    [ObservableProperty]
    private bool _isDropdownOpen;
    
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
    private readonly MatrixDiscoveryService _discoveryService;
    
    public MainWindowViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
        _logService = LogService.Instance;
        _discoveryService = MatrixDiscoveryService.Instance;
        
        _logEntries = _logService.LogEntries;
        _filteredLogEntries = new ObservableCollection<LogEntry>();
        _discoveredMatrices = _discoveryService.DiscoveredMatrices;
        
        // Subscribe to log entry changes
        _logEntries.CollectionChanged += OnLogEntriesChanged;
        
        // Subscribe to discovered matrices changes
        _discoveredMatrices.CollectionChanged += OnDiscoveredMatricesChanged;
        
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
        
        // Subscribe to discovery changes
        _discoveryService.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MatrixDiscoveryService.IsDiscoveryActive))
                IsDiscoveryActive = _discoveryService.IsDiscoveryActive;
        };
        
        // Start discovery automatically
        StartDiscoveryCommand.Execute(null);
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
    
    private void OnDiscoveredMatricesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            // If this is the first discovered matrix and we still have the default IP, replace it
            if (DiscoveredMatrices.Count == 1 && HostAddress == MatrixDiscoveryService.DefaultHostAddress)
            {
                var firstMatrix = e.NewItems[0] as DiscoveredMatrix;
                if (firstMatrix != null)
                {
                    HostAddress = firstMatrix.IpAddress;
                    LogService.Instance.LogDebug($"Auto-populated host address with discovered matrix: {firstMatrix.IpAddress}");
                }
            }
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
    
    [RelayCommand]
    private void StartDiscovery()
    {
        _discoveryService.StartDiscovery();
    }
    
    [RelayCommand]
    private async Task StopDiscoveryAsync()
    {
        await _discoveryService.StopDiscoveryAsync();
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
    
    partial void OnSelectedDiscoveredMatrixChanged(DiscoveredMatrix? value)
    {
        if (value != null)
        {
            HostAddress = value.IpAddress;
            IsDropdownOpen = false;
        }
    }
}
