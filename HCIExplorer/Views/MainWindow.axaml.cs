using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Specialized;
using HCIExplorer.ViewModels;

namespace HCIExplorer.Views;

public partial class MainWindow : Window
{
    private ScrollViewer? _logScrollViewer;
    
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }
    
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        _logScrollViewer = this.FindControl<ScrollViewer>("LogScrollViewer");
        
        if (DataContext is MainWindowViewModel vm)
        {
            vm.FilteredLogEntries.CollectionChanged += OnLogEntriesChanged;
        }
    }
    
    private void OnLogEntriesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm && vm.AutoScroll && _logScrollViewer != null)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                _logScrollViewer.ScrollToEnd();
            });
        }
    }
}