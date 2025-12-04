using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for Audio level and monitoring requests.
/// </summary>
[Collection("HCI Connection")]
public class AudioLevelTests : HCIRequestTestBase
{
    public AudioLevelTests(HCIConnectionFixture fixture, ITestOutputHelper output) 
        : base(fixture, output) { }

    [Fact]
    public void RequestInputLevelStatus_BuildsValidMessage()
    {
        var request = new RequestInputLevelStatusRequest(Fixture.Config.TestTargets.SourcePort);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestInputLevelStatus_ReceivesReply()
    {
        var request = new RequestInputLevelStatusRequest(Fixture.Config.TestTargets.SourcePort);
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyInputLevelStatus, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestOutputLevelStatus_BuildsValidMessage()
    {
        var request = new RequestOutputLevelStatusRequest(Fixture.Config.TestTargets.DestinationPort);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestOutputLevelStatus_ReceivesReply()
    {
        var request = new RequestOutputLevelStatusRequest(Fixture.Config.TestTargets.DestinationPort);
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyOutputLevelStatus, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestInputLevelActions_BuildsValidMessage()
    {
        var request = new RequestInputLevelActionsRequest(
            Fixture.Config.TestTargets.SourcePort,
            inputLevel: 0
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestOutputLevelActions_BuildsValidMessage()
    {
        var request = new RequestOutputLevelActionsRequest(
            Fixture.Config.TestTargets.DestinationPort,
            outputLevel: 0
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestAudioMonitorActions_BuildsValidMessage()
    {
        var actions = new List<AudioMonitorAction>
        {
            new AudioMonitorAction(
                Fixture.Config.TestPanel.Slot,
                Fixture.Config.TestPanel.Port,
                AudioMonitorActionType.StartMonitor,
                AudioMonitorPoint.PreFader
            )
        };
        var request = new RequestAudioMonitorActionsRequest(actions);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestVoxThresholdStatus_BuildsValidMessage()
    {
        var request = new RequestVoxThresholdStatusRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestVoxThresholdSet_BuildsValidMessage()
    {
        var request = new RequestVoxThresholdSetRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port,
            threshold: 50
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestGetPanelAudioFrontEndState_BuildsValidMessage()
    {
        var request = new RequestGetPanelAudioFrontEndStateRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestSetPanelAudioFrontEndState_BuildsValidMessage()
    {
        var request = new RequestSetPanelAudioFrontEndStateRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port,
            micGain: 0,
            sidetoneLevel: 0
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }
}
