using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for System Status and Information requests.
/// </summary>
[Collection("HCI Connection")]
public class SystemStatusTests : HCIRequestTestBase
{
    public SystemStatusTests(HCIConnectionFixture fixture, ITestOutputHelper output) 
        : base(fixture, output) { }

    [Fact]
    public void RequestSystemStatus_BuildsValidMessage()
    {
        var request = new RequestSystemStatusRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        Assert.Equal(0x5A, bytes[0]); // Start marker
        Assert.Equal(0x0F, bytes[1]);
        
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestSystemStatus_ReceivesReply()
    {
        var request = new RequestSystemStatusRequest();
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplySystemStatus, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestFrameStatus_BuildsValidMessage()
    {
        var request = new RequestFrameStatusRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestFrameStatus_ReceivesReply()
    {
        var request = new RequestFrameStatusRequest();
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyFrameStatus, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestActionsStatus_BuildsValidMessage()
    {
        var request = new RequestActionsStatusRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestActionsStatus_ReceivesReply()
    {
        var request = new RequestActionsStatusRequest();
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        LogReply(reply);
    }
}
