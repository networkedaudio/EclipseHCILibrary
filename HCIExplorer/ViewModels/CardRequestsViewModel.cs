using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HCIExplorer.Services;
using HCILibrary.HCIRequests;
using HCILibrary.HCIResponses;
using HCILibrary.Models;
using HCILibrary.Enums;

namespace HCIExplorer.ViewModels;

public partial class CardRequestsViewModel : ViewModelBase
{
    private readonly HCIConnectionService _connectionService;

    [ObservableProperty]
    private int _slot = 1;

    [ObservableProperty]
    private int _port = 0;

    [ObservableProperty]
    private int _cardId = 1;

    [ObservableProperty]
    private ObservableCollection<CardInfo> _cardInfoEntries = new();

    [ObservableProperty]
    private string _lastCardInfoMessage = string.Empty;

    [ObservableProperty]
    private bool _hasCardInfo;

    [ObservableProperty]
    private ObservableCollection<PortInfo> _portInfoEntries = new();

    [ObservableProperty]
    private string _lastPortInfoMessage = string.Empty;

    [ObservableProperty]
    private bool _hasPortInfo;

    [ObservableProperty]
    private ObservableCollection<EhxControlCardInfo> _ehxCardEntries = new();

    [ObservableProperty]
    private string _lastEhxCardMessage = string.Empty;

    [ObservableProperty]
    private bool _hasEhxCards;

    [ObservableProperty]
    private ObservableCollection<NetworkRedundancyCardEntry> _netRedundancyCardEntries = new();

    [ObservableProperty]
    private string _lastNetRedundancyCardMessage = string.Empty;

    [ObservableProperty]
    private bool _hasNetRedundancyCards;

    [ObservableProperty]
    private ObservableCollection<NetworkRedundancyEndpointEntry> _netRedundancyEndpointEntries = new();

    [ObservableProperty]
    private string _lastNetRedundancyEndpointMessage = string.Empty;

    [ObservableProperty]
    private bool _hasNetRedundancyEndpoints;

    [ObservableProperty]
    private ObservableCollection<IpaCardSwitchStateEntry> _ipaCardEntries = new();

    [ObservableProperty]
    private string _lastIpaCardMessage = string.Empty;

    [ObservableProperty]
    private bool _hasIpaCards;

    [ObservableProperty]
    private ObservableCollection<GpioSfoPinStatus> _gpioSfoPins = new();

    [ObservableProperty]
    private string _lastGpioSfoMessage = string.Empty;

    [ObservableProperty]
    private bool _hasGpioSfoPins;

    public CardRequestsViewModel()
    {
        _connectionService = HCIConnectionService.Instance;
        _connectionService.ReplyReceived += OnReplyReceived;
    }

    private void OnReplyReceived(object? sender, HCIReply reply)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (reply.CardInfo is { } cardInfo)
            {
                CardInfoEntries.Clear();
                foreach (var card in cardInfo.Cards)
                {
                    CardInfoEntries.Add(card);
                }
                HasCardInfo = CardInfoEntries.Count > 0;
                LastCardInfoMessage = $"Received {cardInfo.Count} card(s) at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"Card Info: {cardInfo.Count} cards — " +
                    string.Join(", ", cardInfo.Cards.Select(c => $"Rack{c.RackNumber}/Slot{c.SlotNumber}:{c.CurrentCardType}")));
            }

            if (reply.PortInfo is { } portInfo)
            {
                PortInfoEntries.Clear();
                foreach (var port in portInfo.Ports)
                {
                    PortInfoEntries.Add(port);
                }
                HasPortInfo = PortInfoEntries.Count > 0;
                LastPortInfoMessage = $"Slot {portInfo.SlotNumber}: {portInfo.NumberPorts} port(s) at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"Port Info: Slot={portInfo.SlotNumber}, {portInfo.NumberPorts} ports");
            }

            if (reply.EhxControlCardStatus is { } ehxStatus)
            {
                EhxCardEntries.Clear();
                foreach (var card in ehxStatus.Cards)
                {
                    EhxCardEntries.Add(card);
                }
                HasEhxCards = EhxCardEntries.Count > 0;
                LastEhxCardMessage = $"Received {ehxStatus.Cards.Count} card(s) at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"EHX Control Cards: {ehxStatus.Cards.Count} cards");
            }

            if (reply.NetworkRedundancyCardStatus is { } nrCard)
            {
                NetRedundancyCardEntries.Clear();
                foreach (var card in nrCard.Cards)
                {
                    NetRedundancyCardEntries.Add(card);
                }
                HasNetRedundancyCards = NetRedundancyCardEntries.Count > 0;
                LastNetRedundancyCardMessage = $"Received {nrCard.Cards.Count} card(s) at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"Network Redundancy Cards: {nrCard.Cards.Count} entries");
            }

            if (reply.NetworkRedundancyEndpointStatus is { } nrEndpoint)
            {
                NetRedundancyEndpointEntries.Clear();
                foreach (var ep in nrEndpoint.Endpoints)
                {
                    NetRedundancyEndpointEntries.Add(ep);
                }
                HasNetRedundancyEndpoints = NetRedundancyEndpointEntries.Count > 0;
                LastNetRedundancyEndpointMessage = $"Received {nrEndpoint.Endpoints.Count} endpoint(s) at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"Network Redundancy Endpoints: {nrEndpoint.Endpoints.Count} entries");
            }

            if (reply.IpaCardRedundancySwitch is { } ipaSwitch)
            {
                IpaCardEntries.Clear();
                foreach (var card in ipaSwitch.CardSwitchStates)
                {
                    IpaCardEntries.Add(card);
                }
                HasIpaCards = IpaCardEntries.Count > 0;
                LastIpaCardMessage = $"Received {ipaSwitch.CardSwitchStates.Count} card(s) at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"IPA Redundancy Switch: {ipaSwitch.CardSwitchStates.Count} cards");
            }

            if (reply.GpioSfoStatus is { } gpioSfo)
            {
                GpioSfoPins.Clear();
                foreach (var pin in gpioSfo.Pins)
                {
                    GpioSfoPins.Add(pin);
                }
                HasGpioSfoPins = GpioSfoPins.Count > 0;
                LastGpioSfoMessage = $"Card {gpioSfo.CardNumber}: {gpioSfo.Pins.Count} pin(s) at {DateTime.Now:HH:mm:ss}";

                LogService.Instance.LogDebug(
                    $"GPIO/SFO Status: Card={gpioSfo.CardNumber}, {gpioSfo.Pins.Count} pins");
            }
        });
    }
    
    [RelayCommand]
    private async Task RequestCardInfoAsync()
    {
        var request = new RequestCardInfoRequest((byte)Slot);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestAllCardInfoAsync()
    {
        var request = new RequestCardInfoRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestPortInfoAsync()
    {
        var request = new RequestPortInfoRequest((ushort)Slot);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestPeripheralInfoAsync()
    {
        var request = new RequestPeripheralInfoRequest((byte)Slot);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestEhxControlCardStatusAsync()
    {
        var request = new RequestEhxControlCardStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestNetworkRedundancyCardStatusAsync()
    {
        var request = new RequestNetworkRedundancyCardStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestNetworkRedundancyEndpointStatusAsync()
    {
        var request = new RequestNetworkRedundancyEndpointStatusRequest();
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestGpioSfoStatusAsync()
    {
        var request = new RequestGpioSfoStatusRequest((byte)Slot);
        await _connectionService.SendRequestAsync(request);
    }
    
    [RelayCommand]
    private async Task RequestIpaCardRedundancySwitchAsync()
    {
        var request = new RequestIpaCardRedundancySwitchRequest();
        await _connectionService.SendRequestAsync(request);
    }
}
