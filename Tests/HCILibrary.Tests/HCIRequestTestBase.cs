using HCILibrary.Enums;
using HCILibrary.Models;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Base class for HCI request tests providing common functionality.
/// </summary>
[Collection("HCI Connection")]
public abstract class HCIRequestTestBase
{
    protected readonly HCIConnectionFixture Fixture;
    protected readonly ITestOutputHelper Output;
    protected readonly TimeSpan DefaultTimeout;

    protected HCIRequestTestBase(HCIConnectionFixture fixture, ITestOutputHelper output)
    {
        Fixture = fixture;
        Output = output;
        DefaultTimeout = TimeSpan.FromMilliseconds(fixture.Config.EclipseHX.RequestTimeoutMs);
    }

    /// <summary>
    /// Checks if connected and skips test if not.
    /// </summary>
    protected void SkipIfNotConnected()
    {
        Skip.If(!Fixture.IsConnected, "Not connected to EclipseHX matrix. Configure testsettings.json with valid IP address.");
    }

    /// <summary>
    /// Sends a request and waits for response.
    /// </summary>
    protected async Task<HCIReply?> SendAndWaitAsync(HCIRequest request)
    {
        SkipIfNotConnected();

        var tcs = new TaskCompletionSource<HCIReply>();
        
        void Handler(object? sender, HCIReply reply)
        {
            if (request.ExpectedReplyMessageID.HasValue && 
                reply.MessageID == request.ExpectedReplyMessageID.Value)
            {
                tcs.TrySetResult(reply);
            }
        }

        Fixture.Connection!.MessageReceived += Handler;

        try
        {
            Fixture.Connection.RequestQueue!.Enqueue(request);

            using var cts = new CancellationTokenSource(DefaultTimeout);
            cts.Token.Register(() => tcs.TrySetCanceled());

            return await tcs.Task;
        }
        finally
        {
            Fixture.Connection.MessageReceived -= Handler;
        }
    }

    /// <summary>
    /// Logs request bytes to test output.
    /// </summary>
    protected void LogRequestBytes(HCIRequest request)
    {
        var bytes = request.BuildMessage();
        var hexString = string.Join(" ", bytes.Select(b => $"0x{b:X2}"));
        Output.WriteLine($"Request [{request.MessageID}] ({bytes.Length} bytes):");
        Output.WriteLine(hexString);
    }

    /// <summary>
    /// Logs reply information to test output.
    /// </summary>
    protected void LogReply(HCIReply reply)
    {
        Output.WriteLine($"Reply [{reply.MessageID}]:");
        Output.WriteLine($"  Version: {reply.Version}");
        Output.WriteLine($"  Flags: E={reply.Flags.E}, M={reply.Flags.M}, U={reply.Flags.U}, G={reply.Flags.G}, S={reply.Flags.S}, N={reply.Flags.N}");
        Output.WriteLine($"  Payload: {reply.Payload?.Length ?? 0} bytes");
    }
}
