using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for Proxy, Display, and GPIO requests.
/// </summary>
[Collection("HCI Connection")]
public class ProxyGpioTests : HCIRequestTestBase
{
    public ProxyGpioTests(HCIConnectionFixture fixture, ITestOutputHelper output) 
        : base(fixture, output) { }

    [Fact]
    public void RequestGetProxyIndicationState_BuildsValidMessage()
    {
        var request = new RequestGetProxyIndicationStateRequest(
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
    public void RequestSetProxyIndicationState_BuildsValidMessage()
    {
        var request = new RequestSetProxyIndicationStateRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port,
            Fixture.Config.TestPanel.KeyRegion,
            Fixture.Config.TestPanel.KeyNumber,
            tally: ProxyKeyColour.Red,
            rate: ProxyKeyRate.Steady
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestGetProxyDisplayData_BuildsValidMessage()
    {
        var request = new RequestGetProxyDisplayDataRequest(
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
    public void RequestSetProxyDisplayData_BuildsValidMessage()
    {
        var request = new RequestSetProxyDisplayDataRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port,
            Fixture.Config.TestPanel.KeyRegion,
            Fixture.Config.TestPanel.KeyNumber,
            line1: "TEST1",
            line2: "TEST2"
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestGpioSfoStatus_BuildsValidMessage()
    {
        var request = new RequestGpioSfoStatusRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestGpioSfoStatus_ReceivesReply()
    {
        var request = new RequestGpioSfoStatusRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyGpioSfoStatus, reply!.MessageID);
        LogReply(reply);
    }
}
