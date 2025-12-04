using HCILibrary.Enums;
using HCILibrary.Models;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for live communication with EclipseHX matrix.
/// These tests require a configured connection and will be skipped if not connected.
/// </summary>
[Collection("HCI Connection")]
public class HCILiveConnectionTests
{
    private readonly HCIConnectionFixture _fixture;
    private readonly ITestOutputHelper _output;

    public HCILiveConnectionTests(HCIConnectionFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [SkippableFact]
    public void Connection_CanConnect()
    {
        Skip.If(!_fixture.IsConnected, "Not connected to EclipseHX matrix. Configure testsettings.json with valid IP address.");
        
        _output.WriteLine($"Connected to {_fixture.Config.IpAddress} on port {_fixture.Connection!.CurrentPort}");
        Assert.True(_fixture.Connection.IsConnected);
    }

    [SkippableFact]
    public async Task RequestSystemStatus_ReceivesReply()
    {
        Skip.If(!_fixture.IsConnected, "Not connected to EclipseHX matrix.");

        var request = new HCILibrary.HCIRequests.RequestSystemStatusRequest();
        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        _output.WriteLine($"Received reply for message ID: {reply!.MessageID}");
        _output.WriteLine($"Version: {reply.Version}");
        _output.WriteLine($"Payload size: {reply.Payload?.Length ?? 0} bytes");
    }

    [SkippableFact]
    public async Task RequestFrameStatus_ReceivesReply()
    {
        Skip.If(!_fixture.IsConnected, "Not connected to EclipseHX matrix.");

        var request = new HCILibrary.HCIRequests.RequestFrameStatusRequest();
        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        _output.WriteLine($"Received reply for message ID: {reply!.MessageID}");
    }

    [SkippableFact]
    public async Task RequestAssignedKeys_ReceivesReply()
    {
        Skip.If(!_fixture.IsConnected, "Not connected to EclipseHX matrix.");

        var request = new HCILibrary.HCIRequests.RequestAssignedKeysRequest(
            _fixture.Config.Slot, 
            _fixture.Config.Port
        );
        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyAssignedKeys, reply!.MessageID);
        _output.WriteLine($"Received reply for message ID: {reply.MessageID}");
    }

    [SkippableFact]
    public async Task RequestCardInfo_ReceivesReply()
    {
        Skip.If(!_fixture.IsConnected, "Not connected to EclipseHX matrix.");

        var request = new HCILibrary.HCIRequests.RequestCardInfoRequest(_fixture.Config.Slot);
        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyCardInfo, reply!.MessageID);
        _output.WriteLine($"Received reply for message ID: {reply.MessageID}");
    }

    [SkippableFact]
    public async Task RequestVoIPStatus_ReceivesReply()
    {
        Skip.If(!_fixture.IsConnected, "Not connected to EclipseHX matrix.");

        var request = new HCILibrary.HCIRequests.RequestVoIPStatusRequest();
        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyVoIPStatus, reply!.MessageID);
        _output.WriteLine($"Received reply for message ID: {reply.MessageID}");
    }

    /// <summary>
    /// Sends a request and waits for response.
    /// </summary>
    private async Task<HCIReply?> SendAndWaitAsync(HCIRequest request)
    {
        var tcs = new TaskCompletionSource<HCIReply>();
        
        void Handler(object? sender, HCIReply reply)
        {
            if (request.ExpectedReplyMessageID.HasValue && 
                reply.MessageID == request.ExpectedReplyMessageID.Value)
            {
                tcs.TrySetResult(reply);
            }
        }

        _fixture.Connection!.MessageReceived += Handler;

        try
        {
            _fixture.Connection.RequestQueue!.Enqueue(request);

            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(_fixture.Config.RequestTimeoutMs));
            cts.Token.Register(() => tcs.TrySetCanceled());

            return await tcs.Task;
        }
        finally
        {
            _fixture.Connection.MessageReceived -= Handler;
        }
    }
}
