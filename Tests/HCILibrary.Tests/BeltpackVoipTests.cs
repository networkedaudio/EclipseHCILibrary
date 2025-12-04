using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for Beltpack and VoIP requests.
/// </summary>
[Collection("HCI Connection")]
public class BeltpackVoipTests : HCIRequestTestBase
{
    public BeltpackVoipTests(HCIConnectionFixture fixture, ITestOutputHelper output) 
        : base(fixture, output) { }

    [Fact]
    public void RequestBeltpackInformation_BuildsValidMessage()
    {
        var request = new RequestBeltpackInformationRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestBeltpackInformation_ReceivesReply()
    {
        var request = new RequestBeltpackInformationRequest();
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyBeltpackInformation, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestBeltpackAdd_BuildsValidMessage()
    {
        var request = new RequestBeltpackAddRequest(
            beltpackId: 12345,
            radioMode: RadioMode.FiveGHz,
            roleId: 1
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestBeltpackDelete_BuildsValidMessage()
    {
        var request = new RequestBeltpackDeleteRequest(beltpackId: 12345);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestVoIPStatus_BuildsValidMessage()
    {
        var request = new RequestVoIPStatusRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestVoIPStatus_ReceivesReply()
    {
        var request = new RequestVoIPStatusRequest();
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyVoIPStatus, reply!.MessageID);
        LogReply(reply);
    }
}
