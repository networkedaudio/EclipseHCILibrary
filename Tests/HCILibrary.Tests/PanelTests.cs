using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for Panel-related requests.
/// </summary>
[Collection("HCI Connection")]
public class PanelTests : HCIRequestTestBase
{
    public PanelTests(HCIConnectionFixture fixture, ITestOutputHelper output) 
        : base(fixture, output) { }

    [Fact]
    public void RequestPanelStatus_BuildsValidMessage()
    {
        var request = new RequestPanelStatusRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestPanelStatus_ReceivesReply()
    {
        var request = new RequestPanelStatusRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyPanelStatus, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestPanelDiscovery_BuildsValidMessage()
    {
        var request = new RequestPanelDiscoveryRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestPanelDiscovery_ReceivesReply()
    {
        var request = new RequestPanelDiscoveryRequest();
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        LogReply(reply);
    }

    [Fact]
    public void RequestPanelKeysStatus_BuildsValidMessage()
    {
        var request = new RequestPanelKeysStatusRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestPanelKeysStatus_ReceivesReply()
    {
        var request = new RequestPanelKeysStatusRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyPanelKeysStatus, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestPanelKeysPublicGetState_BuildsValidMessage()
    {
        var request = new RequestPanelKeysPublicGetStateRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestPanelKeysPublicSetState_BuildsValidMessage()
    {
        var request = new RequestPanelKeysPublicSetStateRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port,
            Fixture.Config.TestPanel.KeyRegion,
            Fixture.Config.TestPanel.KeyNumber,
            page: 1,
            action: KeyEvent.SetKeyDown
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestPanelShiftPageAction_BuildsValidMessage()
    {
        var request = new RequestPanelShiftPageActionRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port,
            PanelShiftPageActionType.ShiftUp
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestIPPanelList_BuildsValidMessage()
    {
        var request = new RequestIPPanelListRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestIPPanelList_ReceivesReply()
    {
        var request = new RequestIPPanelListRequest();
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        LogReply(reply);
    }

    [Fact]
    public void RequestMacroPanelKeysPublicState_BuildsValidMessage()
    {
        var request = new RequestMacroPanelKeysPublicStateRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestPanelConnectionManagementAction_BuildsValidMessage()
    {
        var request = new RequestPanelConnectionManagementActionRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port,
            PanelConnectionActionType.Connect
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }
}
