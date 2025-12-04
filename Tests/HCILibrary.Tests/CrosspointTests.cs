using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for Crosspoint and Level requests.
/// </summary>
[Collection("HCI Connection")]
public class CrosspointTests : HCIRequestTestBase
{
    public CrosspointTests(HCIConnectionFixture fixture, ITestOutputHelper output) 
        : base(fixture, output) { }

    [Fact]
    public void RequestCrosspointStatus_BuildsValidMessage()
    {
        var request = new RequestCrosspointStatusRequest(
            Fixture.Config.TestTargets.SourcePort,
            Fixture.Config.TestTargets.DestinationPort
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestCrosspointStatus_ReceivesReply()
    {
        var request = new RequestCrosspointStatusRequest(
            Fixture.Config.TestTargets.SourcePort,
            Fixture.Config.TestTargets.DestinationPort
        );
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyCrosspointStatus, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestCrosspointActions_BuildsValidMessage()
    {
        var actions = new List<CrosspointAction>
        {
            new CrosspointAction(
                Fixture.Config.TestTargets.SourcePort,
                Fixture.Config.TestTargets.DestinationPort,
                ActionType.Activate,
                CrosspointPriority.Standard
            )
        };
        var request = new RequestCrosspointActionsRequest(actions);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestCrosspointLevelStatus_BuildsValidMessage()
    {
        var request = new RequestCrosspointLevelStatusRequest(
            Fixture.Config.TestTargets.SourcePort,
            Fixture.Config.TestTargets.DestinationPort
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestCrosspointLevelStatus_ReceivesReply()
    {
        var request = new RequestCrosspointLevelStatusRequest(
            Fixture.Config.TestTargets.SourcePort,
            Fixture.Config.TestTargets.DestinationPort
        );
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyCrosspointLevelStatus, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestCrosspointLevelActions_BuildsValidMessage()
    {
        var actions = new List<CrosspointLevelAction>
        {
            new CrosspointLevelAction(
                Fixture.Config.TestTargets.SourcePort,
                Fixture.Config.TestTargets.DestinationPort,
                listenLevel: 0,
                talkLevel: 0
            )
        };
        var request = new RequestCrosspointLevelActionsRequest(actions);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestXptAndLevelStatus_BuildsValidMessage()
    {
        var request = new RequestXptAndLevelStatusRequest(
            Fixture.Config.TestTargets.SourcePort,
            Fixture.Config.TestTargets.DestinationPort
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestXptAndLevelStatus_ReceivesReply()
    {
        var request = new RequestXptAndLevelStatusRequest(
            Fixture.Config.TestTargets.SourcePort,
            Fixture.Config.TestTargets.DestinationPort
        );
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        LogReply(reply);
    }

    [Fact]
    public void RequestSystemCrosspoint_BuildsValidMessage()
    {
        var request = new RequestSystemCrosspointRequest(
            Fixture.Config.TestTargets.SourcePort,
            Fixture.Config.TestTargets.DestinationPort
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }
}
