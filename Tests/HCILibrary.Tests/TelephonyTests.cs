using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for Telephony-related requests.
/// </summary>
[Collection("HCI Connection")]
public class TelephonyTests : HCIRequestTestBase
{
    public TelephonyTests(HCIConnectionFixture fixture, ITestOutputHelper output) 
        : base(fixture, output) { }

    [Fact]
    public void RequestTelephonyClientGetState_BuildsValidMessage()
    {
        var request = new RequestTelephonyClientGetStateRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestTelephonyClientGetState_ReceivesReply()
    {
        var request = new RequestTelephonyClientGetStateRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyTelephonyClientState, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestTelephonyClientDisconnect_BuildsValidMessage()
    {
        var request = new RequestTelephonyClientDisconnectRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestTelephonyClientDisconnectOutgoing_BuildsValidMessage()
    {
        var request = new RequestTelephonyClientDisconnectOutgoingRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestTelephonyClientDialInfoOutgoing_BuildsValidMessage()
    {
        var request = new RequestTelephonyClientDialInfoOutgoingRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port,
            phoneNumber: "5551234567"
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestTelephonyClientDialInfoIncoming_BuildsValidMessage()
    {
        var request = new RequestTelephonyClientDialInfoIncomingRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestChangeTelephoneHookState_BuildsValidMessage()
    {
        var request = new RequestChangeTelephoneHookStateRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port,
            HookState.OnHook
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestTelephonyKeyStatusEnable_BuildsValidMessage()
    {
        var request = new RequestTelephonyKeyStatusEnableRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port,
            enabled: true
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestTrunkUsageStatistics_BuildsValidMessage()
    {
        var request = new RequestTrunkUsageStatisticsRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestTrunkUsageStatistics_ReceivesReply()
    {
        var request = new RequestTrunkUsageStatisticsRequest();
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyTrunkUsageStatistics, reply!.MessageID);
        LogReply(reply);
    }
}
