using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests for EHX Control and SRecord requests.
/// </summary>
[Collection("HCI Connection")]
public class EhxControlTests : HCIRequestTestBase
{
    public EhxControlTests(HCIConnectionFixture fixture, ITestOutputHelper output) 
        : base(fixture, output) { }

    [Fact]
    public void RequestEhxControlCardStatus_BuildsValidMessage()
    {
        var request = new RequestEhxControlCardStatusRequest();
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [SkippableFact]
    public async Task RequestEhxControlCardStatus_ReceivesReply()
    {
        var request = new RequestEhxControlCardStatusRequest();
        LogRequestBytes(request);

        var reply = await SendAndWaitAsync(request);

        Assert.NotNull(reply);
        Assert.Equal(HCIMessageID.ReplyEhxControlCardStatus, reply!.MessageID);
        LogReply(reply);
    }

    [Fact]
    public void RequestEhxControlActions_BuildsValidMessage()
    {
        var actions = new List<EhxControlAction>
        {
            new EhxControlAction(EhxCardType.Ivt, ActionType.Activate)
        };
        var request = new RequestEhxControlActionsRequest(actions);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestCpuReset_BuildsValidMessage()
    {
        var request = new RequestCpuResetRequest(CpuResetType.SoftReset);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestSRecordTransmissionInitiation_BuildsValidMessage()
    {
        var request = new RequestSRecordTransmissionInitiationRequest(
            applyType: SRecordApplyType.ApplyNow,
            fileSize: 1024
        );
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }

    [Fact]
    public void RequestSRecordTransmission_BuildsValidMessage()
    {
        var recordData = new byte[] { 0x53, 0x30, 0x30, 0x30, 0x30, 0x30, 0x46, 0x43 }; // Sample S-record
        var request = new RequestSRecordTransmissionRequest(recordData);
        var bytes = request.BuildMessage();

        Assert.NotEmpty(bytes);
        LogRequestBytes(request);
    }
}
