using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HCIExplorer.Services;

public partial class LogService : ObservableObject
{
    private static LogService? _instance;
    public static LogService Instance => _instance ??= new LogService();
    
    [ObservableProperty]
    private ObservableCollection<Models.LogEntry> _logEntries = new();
    
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
    
    private LogService() { }
    
    public void LogRequest(string message, string? hexData = null)
    {
        AddEntry(new Models.LogEntry(Models.LogEntryType.Request, message, hexData));
    }
    
    public void LogResponse(string message, string? hexData = null)
    {
        AddEntry(new Models.LogEntry(Models.LogEntryType.Response, message, hexData));
    }
    
    public void LogDebug(string message, string? hexData = null)
    {
        AddEntry(new Models.LogEntry(Models.LogEntryType.Debug, message, hexData));
    }
    
    public void LogError(string message, string? hexData = null)
    {
        AddEntry(new Models.LogEntry(Models.LogEntryType.Error, message, hexData));
    }
    
    private void AddEntry(Models.LogEntry entry)
    {
        // Ensure we're on the UI thread
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            LogEntries.Add(entry);
            
            // Keep log size manageable
            while (LogEntries.Count > 10000)
            {
                LogEntries.RemoveAt(0);
            }
        });
    }
    
    public void Clear()
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            LogEntries.Clear();
        });
    }
}
