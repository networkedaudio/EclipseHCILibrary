using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for Alias and System Configuration requests.
/// </summary>
[Collection("HCI Connection")]
public class AliasConfigTests : HCIRequestTestBase
{
    public AliasConfigTests(HCIConnectionFixture fixture, ITestOutputHelper output) 
        : base(fixture, output) { }

    [Fact]
    public void RequestUnicodeAliasList_BuildsValidMessage()
    {
        var request = new RequestUnicodeAliasListRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestUnicodeAliasList_ReceivesReply()
    {
        var request = new RequestUnicodeAliasListRequest();
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        LogReply(reply);
    }

    [Fact]
    public void RequestUnicodeAliasAdd_BuildsValidMessage()
    {
        var request = new RequestUnicodeAliasAddRequest(
            portId: Fixture.Config.TestTargets.SourcePort,
            alias: "TestAlias"
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestAliasDelete_BuildsValidMessage()
    {
        var request = new RequestAliasDeleteRequest(
            portId: Fixture.Config.TestTargets.SourcePort
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestSetSystemTime_BuildsValidMessage()
    {
        var request = new RequestSetSystemTimeRequest(DateTime.UtcNow);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestSystemMessages_BuildsValidMessage()
    {
        var request = new RequestSystemMessagesRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestSystemMessages_ReceivesReply()
    {
        var request = new RequestSystemMessagesRequest();
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        LogReply(reply);
    }

    [Fact]
    public void RequestSetConfigMultipleKeys_BuildsValidMessage()
    {
        var configs = new List<MultipleKeyConfig>
        {
            new MultipleKeyConfig(
                Fixture.Config.TestPanel.Slot,
                Fixture.Config.TestPanel.Port,
                Fixture.Config.TestPanel.KeyRegion,
                Fixture.Config.TestPanel.KeyNumber,
                targetEntity: Fixture.Config.TestTargets.DestinationPort
            )
        };
        var request = new RequestSetConfigMultipleKeysRequest(configs);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestRoleState_BuildsValidMessage()
    {
        var request = new RequestRoleStateRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestRoleStateSet_BuildsValidMessage()
    {
        var request = new RequestRoleStateSetRequest(
            roleId: 1,
            allocate: true
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }
}
