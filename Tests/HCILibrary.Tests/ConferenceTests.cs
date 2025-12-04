using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for Conference-related requests.
/// </summary>
[Collection("HCI Connection")]
public class ConferenceTests : HCIRequestTestBase
{
    public ConferenceTests(HCIConnectionFixture fixture, ITestOutputHelper output) 
        : base(fixture, output) { }

    [Fact]
    public void RequestConferenceStatus_BuildsValidMessage()
    {
        var request = new RequestConferenceStatusRequest(Fixture.Config.TestTargets.ConferenceId);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestConferenceStatus_ReceivesReply()
    {
        var request = new RequestConferenceStatusRequest(Fixture.Config.TestTargets.ConferenceId);
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyConferenceStatus, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestConferenceActions_BuildsValidMessage()
    {
        var actions = new List<ConferenceAction>
        {
            new ConferenceAction(
                Fixture.Config.TestTargets.ConferenceId,
                Fixture.Config.TestTargets.SourcePort,
                ActionType.Activate
            )
        };
        var request = new RequestConferenceActionsRequest(actions);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestConferenceFixedGroupMembersEdits_BuildsValidMessage()
    {
        var request = new RequestConferenceFixedGroupMembersEditsRequest(
            Fixture.Config.TestTargets.ConferenceId,
            ConferenceEditType.AddMember,
            Fixture.Config.TestTargets.SourcePort
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }
}
