using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for Card and Port information requests.
/// </summary>
[Collection("HCI Connection")]
public class CardPortInfoTests : HCIRequestTestBase
{
    public CardPortInfoTests(HCIConnectionFixture fixture, ITestOutputHelper output) 
        : base(fixture, output) { }

    [Fact]
    public void RequestCardInfo_BuildsValidMessage()
    {
        var request = new RequestCardInfoRequest(Fixture.Config.TestPanel.Slot);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestCardInfo_ReceivesReply()
    {
        var request = new RequestCardInfoRequest(Fixture.Config.TestPanel.Slot);
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyCardInfo, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestPortInfo_BuildsValidMessage()
    {
        var request = new RequestPortInfoRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestPortInfo_ReceivesReply()
    {
        var request = new RequestPortInfoRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyPortInfo, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestPeripheralInfo_BuildsValidMessage()
    {
        var request = new RequestPeripheralInfoRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestPeripheralInfo_ReceivesReply()
    {
        var request = new RequestPeripheralInfoRequest(
            Fixture.Config.TestPanel.Slot,
            Fixture.Config.TestPanel.Port
        );
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyPeripheralInfo, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestEntityInfo_BuildsValidMessage()
    {
        var request = new RequestEntityInfoRequest(entityId: 1);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestEntityInfo_ReceivesReply()
    {
        var request = new RequestEntityInfoRequest(entityId: 1);
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyEntityInfo, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestRackConfigurationStatus_BuildsValidMessage()
    {
        var request = new RequestRackConfigurationStatusRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestRackPropertiesRackStateGet_BuildsValidMessage()
    {
        var request = new RequestRackPropertiesRackStateGetRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestRackPropertiesConfigBank_BuildsValidMessage()
    {
        var request = new RequestRackPropertiesConfigBankRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestIpaCardRedundancySwitch_BuildsValidMessage()
    {
        var request = new RequestIpaCardRedundancySwitchRequest(
            Fixture.Config.TestPanel.Slot,
            switchToSecondary: false
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestNetworkRedundancyEndpointStatus_BuildsValidMessage()
    {
        var request = new RequestNetworkRedundancyEndpointStatusRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestNetworkRedundancyCardStatus_BuildsValidMessage()
    {
        var request = new RequestNetworkRedundancyCardStatusRequest(
            Fixture.Config.TestPanel.Slot
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }
}
