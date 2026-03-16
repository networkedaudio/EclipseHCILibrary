using RESTLibrary.Models;
using System.Text.Json;

namespace RESTLibrary;

/// <summary>
/// REST API connection for Clear-Com LQ devices.
/// All methods correspond to operationIds from the LQ OpenAPI specification.
/// Responses are returned as <see cref="JsonElement"/> to accommodate the
/// self-describing nature of the device API — callers can inspect the JSON
/// structure directly or deserialize into strongly-typed models as needed.
/// </summary>
public class LQConnection : RestConnection
{
    /// <inheritdoc/>
    protected override string ApiBasePath => "/api/1";

    /// <summary>
    /// Creates a new connection to an LQ device.
    /// </summary>
    /// <param name="host">IP address or hostname.</param>
    /// <param name="username">Login username.</param>
    /// <param name="password">Login password.</param>
    /// <param name="port">HTTPS port (default 443).</param>
    public LQConnection(string host, string username, string password, int port = 443)
        : base(host, username, password, port)
    {
    }

    // ── Capabilities ──

    /// <summary>Returns capabilities for all supported devices.</summary>
    public Task<RestResult<JsonElement>> GetDevicesCapabilitiesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/capabilities/devices", ct);

    /// <summary>Returns capability for the specified device type.</summary>
    public Task<RestResult<JsonElement>> GetDeviceCapabilitiesByTypeAsync(string deviceType, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/capabilities/devices/{Uri.EscapeDataString(deviceType)}", ct);

    /// <summary>Returns capabilities for all supported connection types.</summary>
    public Task<RestResult<JsonElement>> GetConnectionsCapabilitiesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/capabilities/connections", ct);

    /// <summary>Returns capability for the specified connection type.</summary>
    public Task<RestResult<JsonElement>> GetConnectionCapabilitiesByTypeAsync(string connectionType, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/capabilities/connections/{Uri.EscapeDataString(connectionType)}", ct);

    /// <summary>Returns capabilities for all supported interface types.</summary>
    public Task<RestResult<JsonElement>> GetInterfacesCapabilitiesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/capabilities/interfaces", ct);

    /// <summary>Returns capability for the specified interface type.</summary>
    public Task<RestResult<JsonElement>> GetInterfaceCapabilitiesByTypeAsync(string interfaceType, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/capabilities/interfaces/{Uri.EscapeDataString(interfaceType)}", ct);

    /// <summary>Returns link group capabilities.</summary>
    public Task<RestResult<JsonElement>> GetLinkGroupCapabilitiesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/capabilities/linkgroup", ct);

    // ── Version ──

    /// <summary>Returns version information from the device.</summary>
    public Task<RestResult<JsonElement>> GetVersionAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/version", ct);

    // ── Devices ──

    /// <summary>Returns all devices.</summary>
    public Task<RestResult<JsonElement>> GetDevicesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/devices", ct);

    /// <summary>Returns a single device by ID.</summary>
    public Task<RestResult<JsonElement>> GetDeviceByIdAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}", ct);

    /// <summary>Updates a device.</summary>
    public Task<RestResult<JsonElement>> UpdateDeviceAsync(string deviceId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}", body, ct);

    /// <summary>Deletes a device.</summary>
    public Task<RestResult> DeleteDeviceAsync(string deviceId, CancellationToken ct = default)
        => DeleteAsync($"/devices/{Uri.EscapeDataString(deviceId)}", ct);

    /// <summary>Returns the capability of a specific device.</summary>
    public Task<RestResult<JsonElement>> GetDeviceCapabilityAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/capability", ct);

    /// <summary>Returns device live status.</summary>
    public Task<RestResult<JsonElement>> GetDevicesLiveStatusAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/liveStatus", ct);

    /// <summary>Uploads firmware to a device.</summary>
    public Task<RestResult> DeviceUploadFirmwareAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/upload", body, ct);

    /// <summary>Gets the device upgrade status.</summary>
    public Task<RestResult<JsonElement>> GetDeviceUpgradeStatusAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/upgrade", ct);

    /// <summary>Initiates a device upgrade.</summary>
    public Task<RestResult> DeviceUpgradeAsync(string deviceId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/upgrade", body, ct);

    /// <summary>Updates the device license.</summary>
    public Task<RestResult> DeviceUpdateLicenseAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/license", body, ct);

    /// <summary>Gets the license context for a device.</summary>
    public Task<RestResult<JsonElement>> DeviceGetLicenseContextAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/license/context", ct);

    /// <summary>Uploads a license file to a device.</summary>
    public Task<RestResult> UploadLicenseFileAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/license/upload", body, ct);

    /// <summary>Gets license ticket information.</summary>
    public Task<RestResult<JsonElement>> GetLicenseTicketInfoAsync(string deviceId, string ticketId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/license/ticket/{Uri.EscapeDataString(ticketId)}", ct);

    /// <summary>Updates activations with a ticket.</summary>
    public Task<RestResult> UpdateActivationsWithTicketAsync(string deviceId, string ticketId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/license/ticket/{Uri.EscapeDataString(ticketId)}", body, ct);

    /// <summary>Updates device linking configuration.</summary>
    public Task<RestResult> UpdateLinkConfigAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/updatelinkingconfig", body, ct);

    /// <summary>Enables or disables OTA on a device.</summary>
    public Task<RestResult> DeviceEnableOtaAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/otastate", body, ct);

    /// <summary>Reboots a device.</summary>
    public Task<RestResult> DeviceRebootAsync(string deviceId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/reboot", body, ct);

    /// <summary>Resets a device to factory defaults.</summary>
    public Task<RestResult> DeviceResetToDefaultAsync(string deviceId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/resettodefault", body, ct);

    /// <summary>Sets the network mode on a device.</summary>
    public Task<RestResult> DeviceSetNetModeAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/setnetmode", body, ct);

    /// <summary>Sets the link group locked state on a device.</summary>
    public Task<RestResult> DeviceSetLinkGrouplockedAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/setlinkgrouplocked", body, ct);

    /// <summary>Registers a device with the cloud.</summary>
    public Task<RestResult> RegisterCloudAsync(string deviceId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/registerCloud", body, ct);

    /// <summary>Regenerates the authentication certificate.</summary>
    public Task<RestResult> RegenerateAuthCertificateAsync(string deviceId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/regenerateAuthCertificate", body, ct);

    /// <summary>Sets up network configuration on a device.</summary>
    public Task<RestResult> DeviceSetupNetworkAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/setupnetwork", body, ct);

    /// <summary>Initiates a device snapshot.</summary>
    public Task<RestResult> DeviceInitiateSnapshotAsync(string deviceId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/snapshot", body, ct);

    /// <summary>Gets a device snapshot.</summary>
    public Task<RestResult<JsonElement>> DeviceGetSnapshotAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/snapshot", ct);

    /// <summary>Gets snapshot info for a device.</summary>
    public Task<RestResult<JsonElement>> DeviceGetSnapshotInfoAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/snapshotinfo", ct);

    // ── Endpoints ──

    /// <summary>Returns endpoints on all devices.</summary>
    public Task<RestResult<JsonElement>> GetEndpointsOnAllDevicesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/devices/endpoints", ct);

    /// <summary>Returns endpoints on a specific device.</summary>
    public Task<RestResult<JsonElement>> GetEndpointsOnDeviceAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/endpoints", ct);

    /// <summary>Returns a specific endpoint on a device.</summary>
    public Task<RestResult<JsonElement>> GetEndpointOnDeviceAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}", ct);

    /// <summary>Updates an endpoint on a device.</summary>
    public Task<RestResult<JsonElement>> UpdateEndpointOnDeviceAsync(string deviceId, string endpointId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}", body, ct);

    /// <summary>Deletes an endpoint from a device.</summary>
    public Task<RestResult> DeleteEndpointOnDeviceAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => DeleteAsync($"/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}", ct);

    /// <summary>Changes the role of an endpoint.</summary>
    public Task<RestResult> EndpointChangeRoleAsync(string deviceId, string endpointId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/changerole", body, ct);

    /// <summary>Changes the state of an endpoint.</summary>
    public Task<RestResult> EndpointChangeStateAsync(string deviceId, string endpointId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/state", body, ct);

    /// <summary>Gets the live status of an endpoint.</summary>
    public Task<RestResult<JsonElement>> EndpointGetLiveStatusAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/liveStatus", ct);

    /// <summary>Reboots an endpoint on a device.</summary>
    public Task<RestResult> EndpointRebootOnDeviceAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/reboot", ct);

    /// <summary>Resets an endpoint to factory defaults.</summary>
    public Task<RestResult> EndpointResetToDefaultOnDeviceAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/resettodefault", ct);

    /// <summary>Gets an endpoint snapshot.</summary>
    public Task<RestResult<JsonElement>> EndpointGetSnapshotOnDeviceAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/snapshot", ct);

    /// <summary>Unregisters an endpoint from a device.</summary>
    public Task<RestResult> EndpointUnregisterAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/unregister", ct);

    // ── External Devices ──

    /// <summary>Returns all external devices.</summary>
    public Task<RestResult<JsonElement>> GetExternalDevicesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/externalDevices", ct);

    /// <summary>Adds an external device.</summary>
    public Task<RestResult<JsonElement>> AddExternalDeviceAsync(object body, CancellationToken ct = default)
        => PostAsync<JsonElement>("/externalDevices", body, ct);

    /// <summary>Returns an external device by ID.</summary>
    public Task<RestResult<JsonElement>> GetExternalDeviceByIdAsync(string externalDeviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/externalDevices/{Uri.EscapeDataString(externalDeviceId)}", ct);

    /// <summary>Updates an external device.</summary>
    public Task<RestResult<JsonElement>> UpdateExternalDeviceByIdAsync(string externalDeviceId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/externalDevices/{Uri.EscapeDataString(externalDeviceId)}", body, ct);

    /// <summary>Deletes an external device.</summary>
    public Task<RestResult> DeleteExternalDeviceAsync(string externalDeviceId, CancellationToken ct = default)
        => DeleteAsync($"/externalDevices/{Uri.EscapeDataString(externalDeviceId)}", ct);

    /// <summary>Gets ports on an external device.</summary>
    public Task<RestResult<JsonElement>> GetExternalDevicePortsAsync(string externalDeviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/externalDevices/{Uri.EscapeDataString(externalDeviceId)}/ports", ct);

    /// <summary>Adds a port to an external device.</summary>
    public Task<RestResult<JsonElement>> AddExternalDevicePortAsync(string externalDeviceId, object body, CancellationToken ct = default)
        => PostAsync<JsonElement>($"/externalDevices/{Uri.EscapeDataString(externalDeviceId)}/ports", body, ct);

    /// <summary>Updates a port on an external device.</summary>
    public Task<RestResult<JsonElement>> UpdateExternalDevicePortAsync(string externalDeviceId, string portId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/externalDevices/{Uri.EscapeDataString(externalDeviceId)}/ports/{Uri.EscapeDataString(portId)}", body, ct);

    /// <summary>Deletes a port from an external device.</summary>
    public Task<RestResult> DeleteExternalDevicePortAsync(string externalDeviceId, string portId, CancellationToken ct = default)
        => DeleteAsync($"/externalDevices/{Uri.EscapeDataString(externalDeviceId)}/ports/{Uri.EscapeDataString(portId)}", ct);

    /// <summary>Gets a specific port on an external device.</summary>
    public Task<RestResult<JsonElement>> GetExternalDevicePortAsync(string externalDeviceId, string portId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/externalDevices/{Uri.EscapeDataString(externalDeviceId)}/ports/{Uri.EscapeDataString(portId)}", ct);

    // ── Users ──

    /// <summary>Returns all users.</summary>
    public Task<RestResult<JsonElement>> GetUsersAsync(string? filter = null, CancellationToken ct = default)
        => GetAsync<JsonElement>(filter != null ? $"/users?filter={Uri.EscapeDataString(filter)}" : "/users", ct);

    /// <summary>Returns a user by username.</summary>
    public Task<RestResult<JsonElement>> GetUserByNameAsync(string username, string? filter = null, CancellationToken ct = default)
        => GetAsync<JsonElement>(filter != null
            ? $"/users/{Uri.EscapeDataString(username)}?filter={Uri.EscapeDataString(filter)}"
            : $"/users/{Uri.EscapeDataString(username)}", ct);

    /// <summary>Updates a user.</summary>
    public Task<RestResult<JsonElement>> UpdateUserAsync(string username, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/users/{Uri.EscapeDataString(username)}", body, ct);

    // ── Interfaces ──

    /// <summary>Returns all interfaces on all devices.</summary>
    public Task<RestResult<JsonElement>> GetInterfacesOnAllDevicesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/devices/interfaces", ct);

    /// <summary>Returns interfaces on a specific device.</summary>
    public Task<RestResult<JsonElement>> GetInterfacesOnDeviceAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces", ct);

    /// <summary>Returns a specific interface on a device.</summary>
    public Task<RestResult<JsonElement>> GetInterfaceOnDeviceAsync(string deviceId, string interfaceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}", ct);

    /// <summary>Updates an interface on a device.</summary>
    public Task<RestResult<JsonElement>> UpdateInterfaceOnDeviceAsync(string deviceId, string interfaceId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}", body, ct);

    /// <summary>Returns interface capabilities on a device.</summary>
    public Task<RestResult<JsonElement>> GetInterfacesCapabilitiesOnDeviceAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/capability", ct);

    /// <summary>Returns a specific interface's capabilities on a device.</summary>
    public Task<RestResult<JsonElement>> GetInterfaceCapabilitiesOnDeviceAsync(string deviceId, string interfaceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/capability", ct);

    // ── Ports ──

    /// <summary>Returns all ports on all devices.</summary>
    public Task<RestResult<JsonElement>> GetPortsOnDeviceAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/devices/interfaces/ports", ct);

    /// <summary>Updates ports on devices (bulk).</summary>
    public Task<RestResult<JsonElement>> UpdatePortsOnDeviceAsync(object body, CancellationToken ct = default)
        => PutAsync<JsonElement>("/devices/interfaces/ports", body, ct);

    /// <summary>Returns ports on a specific interface.</summary>
    public Task<RestResult<JsonElement>> GetPortsOnInterfaceOnDeviceAsync(string deviceId, string interfaceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports", ct);

    /// <summary>Returns a specific port on an interface.</summary>
    public Task<RestResult<JsonElement>> GetPortOnInterfaceOnDeviceAsync(string deviceId, string interfaceId, string portId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}", ct);

    /// <summary>Updates a specific port on an interface.</summary>
    public Task<RestResult<JsonElement>> UpdatePortOnDeviceAsync(string deviceId, string interfaceId, string portId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}", body, ct);

    /// <summary>Gets port nulling status.</summary>
    public Task<RestResult<JsonElement>> GetPortNullingStatusAsync(string deviceId, string interfaceId, string portId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/nulling", ct);

    /// <summary>Starts port nulling.</summary>
    public Task<RestResult> StartPortNullingAsync(string deviceId, string interfaceId, string portId, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/nulling", ct);

    /// <summary>Sets GPO state on a port.</summary>
    public Task<RestResult> SetPortGpoStateAsync(string deviceId, string interfaceId, string portId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/gpo", body, ct);

    /// <summary>Joins a port to a connection.</summary>
    public Task<RestResult> JoinPortToConnectionAsync(string deviceId, string interfaceId, string portId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/join", body, ct);

    /// <summary>Removes a port from a connection.</summary>
    public Task<RestResult> LeavePortToConnectionAsync(string deviceId, string interfaceId, string portId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/leave", body, ct);

    // ── Calls ──

    /// <summary>Gets all active calls for the link group.</summary>
    public Task<RestResult<JsonElement>> GetAllCallsAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/devices/interfaces/ports/calls", ct);

    /// <summary>Hangs up all calls across all devices.</summary>
    public Task<RestResult> HangupDevicesCallsAsync(CancellationToken ct = default)
        => DeleteAsync("/devices/interfaces/ports/calls", ct);

    /// <summary>Gets all active calls for a device.</summary>
    public Task<RestResult<JsonElement>> GetCallsForDeviceAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/ports/calls", ct);

    /// <summary>Hangs up all calls on a device.</summary>
    public Task<RestResult> HangupDeviceCallsAsync(string deviceId, CancellationToken ct = default)
        => DeleteAsync($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/ports/calls", ct);

    /// <summary>Gets all active calls for a port.</summary>
    public Task<RestResult<JsonElement>> GetCallsForPortAsync(string deviceId, string interfaceId, string portId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/calls", ct);

    /// <summary>Makes a call on a port.</summary>
    public Task<RestResult<JsonElement>> MakeCallAsync(string deviceId, string interfaceId, string portId, object body, CancellationToken ct = default)
        => PostAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/calls", body, ct);

    /// <summary>Gets a single active call.</summary>
    public Task<RestResult<JsonElement>> GetCallAsync(string deviceId, string interfaceId, string portId, string callId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/calls/{Uri.EscapeDataString(callId)}", ct);

    /// <summary>Hangs up a specific call.</summary>
    public Task<RestResult> HangupCallAsync(string deviceId, string interfaceId, string portId, string callId, CancellationToken ct = default)
        => DeleteAsync($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/calls/{Uri.EscapeDataString(callId)}", ct);

    /// <summary>Sends DTMF tones to an active call.</summary>
    public Task<RestResult> SendDTMFAsync(string deviceId, string interfaceId, string portId, string callId, object body, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/calls/{Uri.EscapeDataString(callId)}/senddtmf", body, ct);

    // ── Connections ──

    /// <summary>Returns all connections.</summary>
    public Task<RestResult<JsonElement>> GetConnectionsAsync(string? filter = null, CancellationToken ct = default)
        => GetAsync<JsonElement>(filter != null ? $"/connections?filter={Uri.EscapeDataString(filter)}" : "/connections", ct);

    /// <summary>Adds a connection.</summary>
    public Task<RestResult<JsonElement>> AddConnectionAsync(object body, CancellationToken ct = default)
        => PostAsync<JsonElement>("/connections", body, ct);

    /// <summary>Returns live status of all connections.</summary>
    public Task<RestResult<JsonElement>> GetConnectionsLiveStatusAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/connections/liveStatus", ct);

    /// <summary>Returns a connection by ID.</summary>
    public Task<RestResult<JsonElement>> GetConnectionByIdAsync(string connectionId, string? filter = null, CancellationToken ct = default)
        => GetAsync<JsonElement>(filter != null
            ? $"/connections/{Uri.EscapeDataString(connectionId)}?filter={Uri.EscapeDataString(filter)}"
            : $"/connections/{Uri.EscapeDataString(connectionId)}", ct);

    /// <summary>Updates a connection.</summary>
    public Task<RestResult<JsonElement>> UpdateConnectionAsync(string connectionId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/connections/{Uri.EscapeDataString(connectionId)}", body, ct);

    /// <summary>Deletes a connection.</summary>
    public Task<RestResult> DeleteConnectionAsync(string connectionId, CancellationToken ct = default)
        => DeleteAsync($"/connections/{Uri.EscapeDataString(connectionId)}", ct);

    /// <summary>Returns live status of a connection.</summary>
    public Task<RestResult<JsonElement>> GetConnectionLiveStatusAsync(string connectionId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/connections/{Uri.EscapeDataString(connectionId)}/liveStatus", ct);

    // ── Roles ──

    /// <summary>Returns all roles.</summary>
    public Task<RestResult<JsonElement>> GetRolesAsync(string? filter = null, CancellationToken ct = default)
        => GetAsync<JsonElement>(filter != null ? $"/roles?filter={Uri.EscapeDataString(filter)}" : "/roles", ct);

    /// <summary>Adds a role.</summary>
    public Task<RestResult<JsonElement>> AddRoleAsync(object body, CancellationToken ct = default)
        => PostAsync<JsonElement>("/roles", body, ct);

    /// <summary>Returns a role by ID.</summary>
    public Task<RestResult<JsonElement>> GetRoleByIdAsync(string roleId, string? filter = null, CancellationToken ct = default)
        => GetAsync<JsonElement>(filter != null
            ? $"/roles/{Uri.EscapeDataString(roleId)}?filter={Uri.EscapeDataString(filter)}"
            : $"/roles/{Uri.EscapeDataString(roleId)}", ct);

    /// <summary>Updates a role.</summary>
    public Task<RestResult<JsonElement>> UpdateRoleAsync(string roleId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/roles/{Uri.EscapeDataString(roleId)}", body, ct);

    /// <summary>Deletes a role.</summary>
    public Task<RestResult> DeleteRoleAsync(string roleId, CancellationToken ct = default)
        => DeleteAsync($"/roles/{Uri.EscapeDataString(roleId)}", ct);

    /// <summary>Resets a role.</summary>
    public Task<RestResult> RoleResetAsync(string roleId, CancellationToken ct = default)
        => PostAsync($"/roles/{Uri.EscapeDataString(roleId)}/reset", ct);

    // ── Agent-IC / IVP Users ──

    /// <summary>Gets all Agent-IC users.</summary>
    public Task<RestResult<JsonElement>> GetIVPUsersAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/ivpusers", ct);

    /// <summary>Adds an Agent-IC user.</summary>
    public Task<RestResult<JsonElement>> AddIVPUserAsync(object body, CancellationToken ct = default)
        => PostAsync<JsonElement>("/ivpusers", body, ct);

    /// <summary>Gets an Agent-IC user by ID.</summary>
    public Task<RestResult<JsonElement>> GetAgentICUserByIdAsync(string userId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/ivpusers/{Uri.EscapeDataString(userId)}", ct);

    /// <summary>Updates an Agent-IC user.</summary>
    public Task<RestResult<JsonElement>> UpdateAgentICUserAsync(string userId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/ivpusers/{Uri.EscapeDataString(userId)}", body, ct);

    /// <summary>Deletes an Agent-IC user.</summary>
    public Task<RestResult> DeleteAgentICUserAsync(string userId, CancellationToken ct = default)
        => DeleteAsync($"/ivpusers/{Uri.EscapeDataString(userId)}", ct);
}
