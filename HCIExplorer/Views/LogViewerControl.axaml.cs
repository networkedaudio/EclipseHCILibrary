using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Specialized;
using HCIExplorer.Services;

namespace HCIExplorer.Views;

public partial class LogViewerControl : UserControl
{
    private ScrollViewer? _scrollViewer;
    
    public LogViewerControl()
    {
        InitializeComponent();
        DataContext = LogService.Instance;
        
        Loaded += OnLoaded;
    }
    
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        _scrollViewer = this.FindControl<ScrollViewer>("LogScrollViewer");
        
        var logService = LogService.Instance;
        // Subscribe to collection changes for auto-scroll
        logService.LogEntries.CollectionChanged += OnLogEntriesChanged;
    }
    
    private void OnLogEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var logService = LogService.Instance;
        if (logService.AutoScroll && _scrollViewer != null)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                _scrollViewer.ScrollToEnd();
            });
        }
    }
    
    private void OnClearClick(object? sender, RoutedEventArgs e)
    {
        LogService.Instance.Clear();
    }
}
