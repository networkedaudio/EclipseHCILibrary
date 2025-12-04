using HCILibrary.Enums;
using HCILibrary.HCIRequests;
using HCILibrary.Models;
using Xunit.Abstractions;

namespace HCILibrary.Tests;

/// <summary>
/// Tests that verify HCI requests can be built and produce valid message format.
/// </summary>
[Collection("HCI Connection")]
public class HCIRequestBuildTests
{
    private readonly HCIConnectionFixture _fixture;
    private readonly ITestOutputHelper _output;

    // HCI message markers
    private static readonly byte[] StartMarker = { 0x5A, 0x0F };
    private static readonly byte[] EndMarker = { 0x2E, 0x8D };

    public HCIRequestBuildTests(HCIConnectionFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    private void LogRequestBytes(string name, byte[] bytes)
    {
        var hexString = string.Join(" ", bytes.Select(b => $"0x{b:X2}"));
        _output.WriteLine($"{name} ({bytes.Length} bytes):");
        _output.WriteLine(hexString);
    }

    private void AssertValidHCIMessage(byte[] message, string requestName)
    {
        // Verify start marker
        Assert.True(message.Length >= 7, $"{requestName}: Message too short");
        Assert.Equal(StartMarker[0], message[0]);
        Assert.Equal(StartMarker[1], message[1]);

        // Verify end marker
        Assert.Equal(EndMarker[0], message[^2]);
        Assert.Equal(EndMarker[1], message[^1]);

        // Verify length field matches actual length
        ushort lengthField = (ushort)((message[2] << 8) | message[3]);
        Assert.Equal(message.Length - 2, lengthField);
    }

    [Fact]
    public void RequestSystemStatus_BuildsValidMessage()
    {
        var request = new RequestSystemStatusRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestSystemStatus", bytes);
        AssertValidHCIMessage(bytes, "RequestSystemStatus");
    }

    [Fact]
    public void RequestFrameStatus_BuildsValidMessage()
    {
        var request = new RequestFrameStatusRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestFrameStatus", bytes);
        AssertValidHCIMessage(bytes, "RequestFrameStatus");
    }

    [Fact]
    public void RequestActionsStatus_BuildsValidMessage()
    {
        var request = new HCILibrary.HCIRequests.RequestActionsStatusRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestActionsStatus", bytes);
        AssertValidHCIMessage(bytes, "RequestActionsStatus");
    }

    [Fact]
    public void RequestPanelDiscovery_BuildsValidMessage()
    {
        var request = new RequestPanelDiscoveryRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestPanelDiscovery", bytes);
        AssertValidHCIMessage(bytes, "RequestPanelDiscovery");
    }

    [Fact]
    public void RequestAssignedKeys_BuildsValidMessage()
    {
        var request = new RequestAssignedKeysRequest(_fixture.Config.Slot, _fixture.Config.Port);
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestAssignedKeys", bytes);
        AssertValidHCIMessage(bytes, "RequestAssignedKeys");
    }

    [Fact]
    public void RequestAssignedKeysWithLabels_BuildsValidMessage()
    {
        var request = new RequestAssignedKeysWithLabelsRequest(_fixture.Config.Slot, _fixture.Config.Port);
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestAssignedKeysWithLabels", bytes);
        AssertValidHCIMessage(bytes, "RequestAssignedKeysWithLabels");
    }

    [Fact]
    public void RequestConferenceStatus_BuildsValidMessage()
    {
        var request = new RequestConferenceStatusRequest(1);
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestConferenceStatus", bytes);
        AssertValidHCIMessage(bytes, "RequestConferenceStatus");
    }

    [Fact]
    public void RequestUnicodeAliasList_BuildsValidMessage()
    {
        var request = new RequestUnicodeAliasListRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestUnicodeAliasList", bytes);
        AssertValidHCIMessage(bytes, "RequestUnicodeAliasList");
    }

    [Fact]
    public void RequestCardInfo_BuildsValidMessage()
    {
        var request = new RequestCardInfoRequest(_fixture.Config.Slot);
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestCardInfo", bytes);
        AssertValidHCIMessage(bytes, "RequestCardInfo");
    }

    [Fact]
    public void RequestIPPanelList_BuildsValidMessage()
    {
        var request = new RequestIPPanelListRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestIPPanelList", bytes);
        AssertValidHCIMessage(bytes, "RequestIPPanelList");
    }

    [Fact]
    public void RequestTrunkUsageStatistics_BuildsValidMessage()
    {
        var request = new RequestTrunkUsageStatisticsRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestTrunkUsageStatistics", bytes);
        AssertValidHCIMessage(bytes, "RequestTrunkUsageStatistics");
    }

    [Fact]
    public void RequestRackConfigurationStatus_BuildsValidMessage()
    {
        var request = new RequestRackConfigurationStatusRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestRackConfigurationStatus", bytes);
        AssertValidHCIMessage(bytes, "RequestRackConfigurationStatus");
    }

    [Fact]
    public void RequestVoIPStatus_BuildsValidMessage()
    {
        var request = new RequestVoIPStatusRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestVoIPStatus", bytes);
        AssertValidHCIMessage(bytes, "RequestVoIPStatus");
    }

    [Fact]
    public void RequestEhxControlCardStatus_BuildsValidMessage()
    {
        var request = new RequestEhxControlCardStatusRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestEhxControlCardStatus", bytes);
        AssertValidHCIMessage(bytes, "RequestEhxControlCardStatus");
    }

    [Fact]
    public void RequestBeltpackInformation_BuildsValidMessage()
    {
        var request = new RequestBeltpackInformationRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestBeltpackInformation", bytes);
        AssertValidHCIMessage(bytes, "RequestBeltpackInformation");
    }

    [Fact]
    public void RequestRoleState_BuildsValidMessage()
    {
        var request = new RequestRoleStateRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestRoleState", bytes);
        AssertValidHCIMessage(bytes, "RequestRoleState");
    }

    [Fact]
    public void RequestNetworkRedundancyEndpointStatus_BuildsValidMessage()
    {
        var request = new RequestNetworkRedundancyEndpointStatusRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestNetworkRedundancyEndpointStatus", bytes);
        AssertValidHCIMessage(bytes, "RequestNetworkRedundancyEndpointStatus");
    }

    [Fact]
    public void RequestAllKeyGroupStatuses_BuildsValidMessage()
    {
        var request = new RequestAllKeyGroupStatusesRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestAllKeyGroupStatuses", bytes);
        AssertValidHCIMessage(bytes, "RequestAllKeyGroupStatuses");
    }

    [Fact]
    public void RequestSystemMessages_BuildsValidMessage()
    {
        var request = new RequestSystemMessagesRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestSystemMessages", bytes);
        AssertValidHCIMessage(bytes, "RequestSystemMessages");
    }

    [Fact]
    public void RequestLocallyAssignedKeys_BuildsValidMessage()
    {
        var request = new RequestLocallyAssignedKeysRequest(_fixture.Config.Slot, _fixture.Config.Port);
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestLocallyAssignedKeys", bytes);
        AssertValidHCIMessage(bytes, "RequestLocallyAssignedKeys");
    }

    [Fact]
    public void RequestPanelKeysStatusAutoUpdates_BuildsValidMessage()
    {
        var request = new RequestPanelKeysStatusAutoUpdatesRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestPanelKeysStatusAutoUpdates", bytes);
        AssertValidHCIMessage(bytes, "RequestPanelKeysStatusAutoUpdates");
    }

    [Fact]
    public void RequestNetworkRedundancyCardStatus_BuildsValidMessage()
    {
        var request = new RequestNetworkRedundancyCardStatusRequest(_fixture.Config.Slot);
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestNetworkRedundancyCardStatus", bytes);
        AssertValidHCIMessage(bytes, "RequestNetworkRedundancyCardStatus");
    }

    [Fact]
    public void RequestRackPropertiesRackStateGet_BuildsValidMessage()
    {
        var request = new RequestRackPropertiesRackStateGetRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestRackPropertiesRackStateGet", bytes);
        AssertValidHCIMessage(bytes, "RequestRackPropertiesRackStateGet");
    }

    [Fact]
    public void RequestRackPropertiesConfigBank_BuildsValidMessage()
    {
        var request = new RequestRackPropertiesConfigBankRequest();
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestRackPropertiesConfigBank", bytes);
        AssertValidHCIMessage(bytes, "RequestRackPropertiesConfigBank");
    }

    [Fact]
    public void RequestPanelKeysUnlatchAll_BuildsValidMessage()
    {
        var request = new RequestPanelKeysUnlatchAllRequest(_fixture.Config.Slot, _fixture.Config.Port);
        var bytes = request.BuildMessage();

        LogRequestBytes("RequestPanelKeysUnlatchAll", bytes);
        AssertValidHCIMessage(bytes, "RequestPanelKeysUnlatchAll");
    }
}
