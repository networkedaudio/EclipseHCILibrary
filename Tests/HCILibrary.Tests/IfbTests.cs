using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for IFB (Interruptible Foldback) requests.
/// </summary>
[Collection("HCI Connection")]
public class IfbTests : HCIRequestTestBase
{
    public IfbTests(HCIConnectionFixture fixture, ITestOutputHelper output) 
        : base(fixture, output) { }

    [Fact]
    public void RequestIfbStatus_BuildsValidMessage()
    {
        var request = new RequestIfbStatusRequest(ifbPort: 1);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestIfbStatus_ReceivesReply()
    {
        var request = new RequestIfbStatusRequest(ifbPort: 1);
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyIfbStatus, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestIfbSet_BuildsValidMessage()
    {
        var request = new RequestIfbSetRequest(
            ifbPort: 1,
            attributeType: IfbAttributeType.DimLevel,
            value: (byte)IfbDimLevel.Level3
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestForcedListenEdits_BuildsValidMessage()
    {
        var request = new RequestForcedListenEditsRequest(
            sourcePort: Fixture.Config.TestTargets.SourcePort,
            destinationPort: Fixture.Config.TestTargets.DestinationPort,
            action: ForcedListenAction.Add
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestForcedListenActions_BuildsValidMessage()
    {
        var request = new RequestForcedListenActionsRequest(
            sourcePort: Fixture.Config.TestTargets.SourcePort,
            destinationPort: Fixture.Config.TestTargets.DestinationPort,
            activate: true
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }
}
