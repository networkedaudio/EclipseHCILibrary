using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for Key-related requests.
/// </summary>
[Collection("HCI Connection")]
public class KeyTests : HCIRequestTestBase
{
    public KeyTests(HCIConnectionFixture fixture, ITestOutputHelper output) 
        : base(fixture, output) { }

    [Fact]
    public void RequestAssignedKeys_BuildsValidMessage()
    {
        var request = new RequestAssignedKeysRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestAssignedKeys_ReceivesReply()
    {
        var request = new RequestAssignedKeysRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyAssignedKeys, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestAssignedKeysWithLabels_BuildsValidMessage()
    {
        var request = new RequestAssignedKeysWithLabelsRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestAssignedKeysWithLabels_ReceivesReply()
    {
        var request = new RequestAssignedKeysWithLabelsRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyAssignedKeysWithLabels, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestLocallyAssignedKeys_BuildsValidMessage()
    {
        var request = new RequestLocallyAssignedKeysRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestPanelKeysAction_BuildsValidMessage()
    {
        var request = new RequestPanelKeysActionRequest(
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
    public void RequestPanelKeysUnlatchAll_BuildsValidMessage()
    {
        var request = new RequestPanelKeysUnlatchAllRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestKeyGroupStatus_BuildsValidMessage()
    {
        var request = new RequestKeyGroupStatusRequest(groupId: 1);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestAllKeyGroupStatuses_BuildsValidMessage()
    {
        var request = new RequestAllKeyGroupStatusesRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestKeyGroupAction_BuildsValidMessage()
    {
        var request = new RequestKeyGroupActionRequest(
            groupId: 1,
            actionType: ActionType.Activate
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestRemoteKeyActions_BuildsValidMessage()
    {
        var actions = new List<RemoteKeyAction>
        {
            new RemoteKeyAction(
                Fixture.Config.TestPanel.Slot,
                Fixture.Config.TestPanel.Port,
                Fixture.Config.TestPanel.KeyRegion,
                Fixture.Config.TestPanel.KeyNumber,
                KeyEvent.SetKeyDown
            )
        };
        var request = new RequestRemoteKeyActionsRequest(actions);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestRemoteKeyActionStatus_BuildsValidMessage()
    {
        var request = new RequestRemoteKeyActionStatusRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port,
            Fixture.Config.TestPanel.KeyRegion,
            Fixture.Config.TestPanel.KeyNumber
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestAltTextState_BuildsValidMessage()
    {
        var request = new RequestAltTextStateRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port,
            Fixture.Config.TestPanel.KeyRegion,
            Fixture.Config.TestPanel.KeyNumber
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestAltTextSet_BuildsValidMessage()
    {
        var request = new RequestAltTextSetRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port,
            Fixture.Config.TestPanel.KeyRegion,
            Fixture.Config.TestPanel.KeyNumber,
            altText: "TEST"
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }
}
