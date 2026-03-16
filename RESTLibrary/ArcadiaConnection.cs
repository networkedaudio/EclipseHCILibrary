using RESTLibrary.Models;
using System.Text.Json;

namespace RESTLibrary;

/// <summary>
/// REST API connection for Clear-Com Arcadia devices.
/// All methods correspond to operationIds from the Arcadia OpenAPI specification.
/// Arcadia paths include a version prefix (e.g. /1/ or /2/) within the path itself,
/// so the API base is just "/api".
/// </summary>
public class ArcadiaConnection : RestConnection
{
    /// <inheritdoc/>
    protected override string ApiBasePath => "/api";

    /// <summary>
    /// Creates a new connection to an Arcadia device.
    /// </summary>
    /// <param name="host">IP address or hostname.</param>
    /// <param name="username">Login username.</param>
    /// <param name="password">Login password.</param>
    /// <param name="port">HTTPS port (default 443).</param>
    public ArcadiaConnection(string host, string username, string password, int port = 443)
        : base(host, username, password, port)
    {
    }

    // ── Capabilities ──

    /// <summary>Returns capabilities for all supported devices.</summary>
    public Task<RestResult<JsonElement>> GetDevicesCapabilitiesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/capabilities/devices", ct);

    /// <summary>Returns capability for the specified device type.</summary>
    public Task<RestResult<JsonElement>> GetDeviceCapabilitiesByTypeAsync(string deviceType, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/capabilities/devices/{Uri.EscapeDataString(deviceType)}", ct);

    /// <summary>Returns capabilities for all supported connection types.</summary>
    public Task<RestResult<JsonElement>> GetConnectionsCapabilitiesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/capabilities/connections", ct);

    /// <summary>Returns capability for the specified connection type.</summary>
    public Task<RestResult<JsonElement>> GetConnectionCapabilitiesByTypeAsync(string connectionType, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/capabilities/connections/{Uri.EscapeDataString(connectionType)}", ct);

    /// <summary>Returns capabilities for all supported interface types.</summary>
    public Task<RestResult<JsonElement>> GetInterfacesCapabilitiesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/capabilities/interfaces", ct);

    /// <summary>Returns capability for the specified interface type.</summary>
    public Task<RestResult<JsonElement>> GetInterfaceCapabilitiesByTypeAsync(string interfaceType, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/capabilities/interfaces/{Uri.EscapeDataString(interfaceType)}", ct);

    /// <summary>Returns link group capabilities.</summary>
    public Task<RestResult<JsonElement>> GetLinkGroupCapabilitiesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/capabilities/linkgroup", ct);

    // ── Version ──

    /// <summary>Returns version information from the device.</summary>
    public Task<RestResult<JsonElement>> GetVersionAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/version", ct);

    // ── Devices ──

    /// <summary>Returns all devices.</summary>
    public Task<RestResult<JsonElement>> GetDevicesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/devices", ct);

    /// <summary>Restores a device configuration from backup.</summary>
    public Task<RestResult> RestoreAsync(object body, CancellationToken ct = default)
        => PostAsync("/1/devices/restore", body, ct);

    /// <summary>Creates a backup of the device configuration.</summary>
    public Task<RestResult<JsonElement>> BackupAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/devices/backup", ct);

    /// <summary>Returns a single device by ID.</summary>
    public Task<RestResult<JsonElement>> GetDeviceByIdAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}", ct);

    /// <summary>Updates a device.</summary>
    public Task<RestResult<JsonElement>> UpdateDeviceAsync(string deviceId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}", body, ct);

    /// <summary>Deletes a device.</summary>
    public Task<RestResult> DeleteDeviceAsync(string deviceId, CancellationToken ct = default)
        => DeleteAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}", ct);

    /// <summary>Returns the capability of a specific device.</summary>
    public Task<RestResult<JsonElement>> GetDeviceCapabilityAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/capability", ct);

    /// <summary>Returns device live status.</summary>
    public Task<RestResult<JsonElement>> GetDevicesLiveStatusAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/liveStatus", ct);

    /// <summary>Uploads firmware to a device.</summary>
    public Task<RestResult> DeviceUploadFirmwareAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/upload", body, ct);

    /// <summary>Gets the device upgrade status.</summary>
    public Task<RestResult<JsonElement>> GetDeviceUpgradeStatusAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/upgrade", ct);

    /// <summary>Initiates a device upgrade.</summary>
    public Task<RestResult> DeviceUpgradeAsync(string deviceId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/upgrade", body, ct);

    // ── Firmware Server ──

    /// <summary>Gets firmware server content for a device.</summary>
    public Task<RestResult<JsonElement>> GetFirmwareServerContentAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/firmware", ct);

    /// <summary>Adds a firmware server file.</summary>
    public Task<RestResult> AddFirmwareServerFileAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/firmware", body, ct);

    /// <summary>Gets a firmware server file by type.</summary>
    public Task<RestResult<JsonElement>> GetFirmwareServerFileAsync(string deviceId, string type, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/firmware/{Uri.EscapeDataString(type)}", ct);

    /// <summary>Deletes a firmware server file by type.</summary>
    public Task<RestResult> DeleteFirmwareServerFileAsync(string deviceId, string type, CancellationToken ct = default)
        => DeleteAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/firmware/{Uri.EscapeDataString(type)}", ct);

    // ── Licensing ──

    /// <summary>Updates licensed ports on a device.</summary>
    public Task<RestResult<JsonElement>> DeviceUpdateLicensedPortsAsync(string deviceId, object body, CancellationToken ct = default)
        => PatchAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/license/ports", body, ct);

    /// <summary>Gets resource allocation options.</summary>
    public Task<RestResult<JsonElement>> GetResourceAllocationOptionsAsync(string version = "1", CancellationToken ct = default)
        => GetAsync<JsonElement>($"/{Uri.EscapeDataString(version)}/resourceAllocationOptions", ct);

    /// <summary>Updates the device license.</summary>
    public Task<RestResult> DeviceUpdateLicenseAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/license", body, ct);

    /// <summary>Gets the license context for a device.</summary>
    public Task<RestResult<JsonElement>> DeviceGetLicenseContextAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/license/context", ct);

    /// <summary>Uploads a license file to a device.</summary>
    public Task<RestResult> UploadLicenseFileAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/license/upload", body, ct);

    /// <summary>Gets license ticket information.</summary>
    public Task<RestResult<JsonElement>> GetLicenseTicketInfoAsync(string deviceId, string ticketId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/license/ticket/{Uri.EscapeDataString(ticketId)}", ct);

    /// <summary>Updates activations with a ticket.</summary>
    public Task<RestResult> UpdateActivationsWithTicketAsync(string deviceId, string ticketId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/license/ticket/{Uri.EscapeDataString(ticketId)}", body, ct);

    /// <summary>Recovers activations with a ticket.</summary>
    public Task<RestResult> RecoverActivationsWithTicketAsync(string deviceId, string ticketId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/devices/{Uri.EscapeDataString(deviceId)}/license/recover/{Uri.EscapeDataString(ticketId)}", body, ct);

    // ── Device Actions ──

    /// <summary>Updates device linking configuration.</summary>
    public Task<RestResult> UpdateLinkConfigAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/updatelinkingconfig", body, ct);

    /// <summary>Enables or disables OTA on a device.</summary>
    public Task<RestResult> DeviceEnableOtaAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/otastate", body, ct);

    /// <summary>Reboots a device.</summary>
    public Task<RestResult> DeviceRebootAsync(string deviceId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/reboot", body, ct);

    /// <summary>Resets a device to factory defaults.</summary>
    public Task<RestResult> DeviceResetToDefaultAsync(string deviceId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/resettodefault", body, ct);

    /// <summary>Sets the network mode on a device.</summary>
    public Task<RestResult> DeviceSetNetModeAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/setnetmode", body, ct);

    /// <summary>Sets up network configuration on a device.</summary>
    public Task<RestResult> DeviceSetupNetworkAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/setupnetwork", body, ct);

    /// <summary>Initiates a device snapshot.</summary>
    public Task<RestResult> DeviceInitiateSnapshotAsync(string deviceId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/snapshot", body, ct);

    /// <summary>Gets a device snapshot.</summary>
    public Task<RestResult<JsonElement>> DeviceGetSnapshotAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/snapshot", ct);

    /// <summary>Gets snapshot info for a device.</summary>
    public Task<RestResult<JsonElement>> DeviceGetSnapshotInfoAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/snapshotinfo", ct);

    // ── Endpoints ──

    /// <summary>Returns endpoints on all devices.</summary>
    public Task<RestResult<JsonElement>> GetEndpointsOnAllDevicesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/devices/endpoints", ct);

    /// <summary>Returns endpoints on a specific device.</summary>
    public Task<RestResult<JsonElement>> GetEndpointsOnDeviceAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints", ct);

    /// <summary>Adds an endpoint to a device.</summary>
    public Task<RestResult<JsonElement>> AddEndpointToDeviceAsync(string deviceId, object body, CancellationToken ct = default)
        => PostAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints", body, ct);

    /// <summary>Returns a specific endpoint on a device.</summary>
    public Task<RestResult<JsonElement>> GetEndpointOnDeviceAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}", ct);

    /// <summary>Updates an endpoint on a device.</summary>
    public Task<RestResult<JsonElement>> UpdateEndpointOnDeviceAsync(string deviceId, string endpointId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}", body, ct);

    /// <summary>Deletes an endpoint from a device.</summary>
    public Task<RestResult> DeleteEndpointOnDeviceAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => DeleteAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}", ct);

    /// <summary>Changes the role of an endpoint.</summary>
    public Task<RestResult> EndpointChangeRoleAsync(string deviceId, string endpointId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/changerole", body, ct);

    /// <summary>Changes the user of an endpoint.</summary>
    public Task<RestResult> EndpointChangeUserAsync(string deviceId, string endpointId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/changeuser", body, ct);

    /// <summary>Changes the association of an endpoint.</summary>
    public Task<RestResult> EndpointChangeAssociationAsync(string deviceId, string endpointId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/changeassociation", body, ct);

    /// <summary>Changes the state of an endpoint.</summary>
    public Task<RestResult> EndpointChangeStateAsync(string deviceId, string endpointId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/state", body, ct);

    /// <summary>Gets the live status of an endpoint.</summary>
    public Task<RestResult<JsonElement>> EndpointGetLiveStatusAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/liveStatus", ct);

    /// <summary>Reboots an endpoint on a device.</summary>
    public Task<RestResult> EndpointRebootOnDeviceAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/reboot", ct);

    /// <summary>Resets an endpoint to factory defaults.</summary>
    public Task<RestResult> EndpointResetToDefaultOnDeviceAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/resettodefault", ct);

    /// <summary>Gets an endpoint snapshot.</summary>
    public Task<RestResult<JsonElement>> EndpointGetSnapshotOnDeviceAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/snapshot", ct);

    /// <summary>Deletes an endpoint snapshot.</summary>
    public Task<RestResult> EndpointDeleteSnapshotOnDeviceAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => DeleteAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/deleteSnapshot", ct);

    /// <summary>Unregisters an endpoint from a device.</summary>
    public Task<RestResult> EndpointUnregisterAsync(string deviceId, string endpointId, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/unregister", ct);

    /// <summary>Discovers endpoints on a device.</summary>
    public Task<RestResult<JsonElement>> EndpointsDiscoveryAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/discovery", ct);

    /// <summary>Scans for Wi-Fi endpoints (GET).</summary>
    public Task<RestResult<JsonElement>> DeviceWifiScanGetAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/wifiscan", ct);

    /// <summary>Initiates a Wi-Fi scan (POST).</summary>
    public Task<RestResult> DeviceWifiScanPostAsync(string deviceId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/wifiscan", body, ct);

    /// <summary>Auto-selects all Edge TCVRs.</summary>
    public Task<RestResult> EndpointAutoScanAllEdgeTcvrAsync(string deviceId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/autoselectalledgetcvrs", body, ct);

    /// <summary>Auto-selects an endpoint.</summary>
    public Task<RestResult> EndpointsAutoSelectAsync(string deviceId, string endpointId, object? body = null, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/endpoints/{Uri.EscapeDataString(endpointId)}/autoselect", body, ct);

    // ── External Devices ──

    /// <summary>Returns all external devices.</summary>
    public Task<RestResult<JsonElement>> GetExternalDevicesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/externalDevices", ct);

    /// <summary>Adds an external device.</summary>
    public Task<RestResult<JsonElement>> AddExternalDeviceAsync(object body, CancellationToken ct = default)
        => PostAsync<JsonElement>("/1/externalDevices", body, ct);

    /// <summary>Returns an external device by ID.</summary>
    public Task<RestResult<JsonElement>> GetExternalDeviceByIdAsync(string externalDeviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/externalDevices/{Uri.EscapeDataString(externalDeviceId)}", ct);

    /// <summary>Updates an external device.</summary>
    public Task<RestResult<JsonElement>> UpdateExternalDeviceByIdAsync(string externalDeviceId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/1/externalDevices/{Uri.EscapeDataString(externalDeviceId)}", body, ct);

    /// <summary>Deletes an external device.</summary>
    public Task<RestResult> DeleteExternalDeviceAsync(string externalDeviceId, CancellationToken ct = default)
        => DeleteAsync($"/1/externalDevices/{Uri.EscapeDataString(externalDeviceId)}", ct);

    /// <summary>Gets ports on an external device.</summary>
    public Task<RestResult<JsonElement>> GetExternalDevicePortsAsync(string externalDeviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/externalDevices/{Uri.EscapeDataString(externalDeviceId)}/ports", ct);

    /// <summary>Adds a port to an external device.</summary>
    public Task<RestResult<JsonElement>> AddExternalDevicePortAsync(string externalDeviceId, object body, CancellationToken ct = default)
        => PostAsync<JsonElement>($"/1/externalDevices/{Uri.EscapeDataString(externalDeviceId)}/ports", body, ct);

    /// <summary>Updates a port on an external device.</summary>
    public Task<RestResult<JsonElement>> UpdateExternalDevicePortAsync(string externalDeviceId, string portId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/1/externalDevices/{Uri.EscapeDataString(externalDeviceId)}/ports/{Uri.EscapeDataString(portId)}", body, ct);

    /// <summary>Deletes a port from an external device.</summary>
    public Task<RestResult> DeleteExternalDevicePortAsync(string externalDeviceId, string portId, CancellationToken ct = default)
        => DeleteAsync($"/1/externalDevices/{Uri.EscapeDataString(externalDeviceId)}/ports/{Uri.EscapeDataString(portId)}", ct);

    /// <summary>Gets a specific port on an external device.</summary>
    public Task<RestResult<JsonElement>> GetExternalDevicePortAsync(string externalDeviceId, string portId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/externalDevices/{Uri.EscapeDataString(externalDeviceId)}/ports/{Uri.EscapeDataString(portId)}", ct);

    // ── Users (v1 — CCM Root/Admin) ──

    /// <summary>Returns all v1 users.</summary>
    public Task<RestResult<JsonElement>> GetUsers1Async(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/users", ct);

    /// <summary>Returns a v1 user by username.</summary>
    public Task<RestResult<JsonElement>> GetUserByNameAsync(string username, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/users/{Uri.EscapeDataString(username)}", ct);

    /// <summary>Updates a v1 user.</summary>
    public Task<RestResult<JsonElement>> UpdateUserAsync(string username, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/1/users/{Uri.EscapeDataString(username)}", body, ct);

    // ── Users (v2 — Standard Users) ──

    /// <summary>Returns all v2 users.</summary>
    public Task<RestResult<JsonElement>> GetUsers2Async(CancellationToken ct = default)
        => GetAsync<JsonElement>("/2/users", ct);

    /// <summary>Returns a v2 user by identity.</summary>
    public Task<RestResult<JsonElement>> GetUserByIdentityAsync(string identity, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/2/users/{Uri.EscapeDataString(identity)}", ct);

    /// <summary>Adds a standard user.</summary>
    public Task<RestResult<JsonElement>> AddStdUserAsync(string identity, object body, CancellationToken ct = default)
        => PostAsync<JsonElement>($"/2/users/{Uri.EscapeDataString(identity)}", body, ct);

    /// <summary>Updates a standard user.</summary>
    public Task<RestResult<JsonElement>> UpdateStdUserAsync(string identity, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/2/users/{Uri.EscapeDataString(identity)}", body, ct);

    /// <summary>Deletes a standard user.</summary>
    public Task<RestResult> DeleteStdUserAsync(string identity, CancellationToken ct = default)
        => DeleteAsync($"/2/users/{Uri.EscapeDataString(identity)}", ct);

    // ── User Sessions ──

    /// <summary>Gets sessions for a user.</summary>
    public Task<RestResult<JsonElement>> GetUserSessionsByIdentityAsync(string identity, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/2/users/{Uri.EscapeDataString(identity)}/sessions", ct);

    /// <summary>Gets sessions by identity and session type.</summary>
    public Task<RestResult<JsonElement>> GetUserSessionByIdentityAndSessionTypeAsync(string identity, string sessionType, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/2/users/{Uri.EscapeDataString(identity)}/sessions/{Uri.EscapeDataString(sessionType)}", ct);

    /// <summary>Adds a user session.</summary>
    public Task<RestResult<JsonElement>> AddUserSessionAsync(string identity, string sessionType, object body, CancellationToken ct = default)
        => PostAsync<JsonElement>($"/2/users/{Uri.EscapeDataString(identity)}/sessions/{Uri.EscapeDataString(sessionType)}", body, ct);

    /// <summary>Gets a specific user session.</summary>
    public Task<RestResult<JsonElement>> GetUserSessionByIdentityAndSessionSpecAsync(string identity, string sessionType, string sessionId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/2/users/{Uri.EscapeDataString(identity)}/sessions/{Uri.EscapeDataString(sessionType)}/{Uri.EscapeDataString(sessionId)}", ct);

    /// <summary>Updates a user session.</summary>
    public Task<RestResult<JsonElement>> UpdateUserSessionAsync(string identity, string sessionType, string sessionId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/2/users/{Uri.EscapeDataString(identity)}/sessions/{Uri.EscapeDataString(sessionType)}/{Uri.EscapeDataString(sessionId)}", body, ct);

    /// <summary>Deletes a user session.</summary>
    public Task<RestResult> DeleteUserSessionAsync(string identity, string sessionType, string sessionId, CancellationToken ct = default)
        => DeleteAsync($"/2/users/{Uri.EscapeDataString(identity)}/sessions/{Uri.EscapeDataString(sessionType)}/{Uri.EscapeDataString(sessionId)}", ct);

    // ── Rolesets ──

    /// <summary>Returns all rolesets.</summary>
    public Task<RestResult<JsonElement>> GetRolesetsAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/2/rolesets", ct);

    /// <summary>Adds a standard roleset.</summary>
    public Task<RestResult<JsonElement>> AddStdRolesetAsync(object body, CancellationToken ct = default)
        => PostAsync<JsonElement>("/2/rolesets", body, ct);

    /// <summary>Returns a roleset by ID.</summary>
    public Task<RestResult<JsonElement>> GetRolesetByIdAsync(string id, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/2/rolesets/{Uri.EscapeDataString(id)}", ct);

    /// <summary>Updates a standard roleset.</summary>
    public Task<RestResult<JsonElement>> UpdateStdRolesetAsync(string id, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/2/rolesets/{Uri.EscapeDataString(id)}", body, ct);

    /// <summary>Deletes a standard roleset.</summary>
    public Task<RestResult> DeleteStdRolesetAsync(string id, CancellationToken ct = default)
        => DeleteAsync($"/2/rolesets/{Uri.EscapeDataString(id)}", ct);

    /// <summary>Bulk creates rolesets.</summary>
    public Task<RestResult<JsonElement>> BulkCreateRolesetsAsync(object body, CancellationToken ct = default)
        => PostAsync<JsonElement>("/2/rolesets/create", body, ct);

    /// <summary>Clones a roleset.</summary>
    public Task<RestResult<JsonElement>> CloneRolesetAsync(string id, object? body = null, CancellationToken ct = default)
        => PostAsync<JsonElement>($"/2/rolesets/{Uri.EscapeDataString(id)}/clone", body, ct);

    // ── Roleset Sessions ──

    /// <summary>Gets sessions for a roleset.</summary>
    public Task<RestResult<JsonElement>> GetRolesetSessionsByIdAsync(string id, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/2/rolesets/{Uri.EscapeDataString(id)}/sessions", ct);

    /// <summary>Gets a roleset session by type.</summary>
    public Task<RestResult<JsonElement>> GetRolesetSessionByIdAndSessionTypeAsync(string id, string sessionType, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/2/rolesets/{Uri.EscapeDataString(id)}/sessions/{Uri.EscapeDataString(sessionType)}", ct);

    /// <summary>Gets a specific roleset session.</summary>
    public Task<RestResult<JsonElement>> GetRolesetSessionByIdAndSessionSpecAsync(string id, string sessionType, string sessionId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/2/rolesets/{Uri.EscapeDataString(id)}/sessions/{Uri.EscapeDataString(sessionType)}/{Uri.EscapeDataString(sessionId)}", ct);

    /// <summary>Updates a roleset session.</summary>
    public Task<RestResult<JsonElement>> UpdateRolesetSessionAsync(string id, string sessionType, string sessionId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/2/rolesets/{Uri.EscapeDataString(id)}/sessions/{Uri.EscapeDataString(sessionType)}/{Uri.EscapeDataString(sessionId)}", body, ct);

    /// <summary>Deletes a roleset session.</summary>
    public Task<RestResult> DeleteRolesetSessionAsync(string id, string sessionType, string sessionId, CancellationToken ct = default)
        => DeleteAsync($"/2/rolesets/{Uri.EscapeDataString(id)}/sessions/{Uri.EscapeDataString(sessionType)}/{Uri.EscapeDataString(sessionId)}", ct);

    // ── Interfaces ──

    /// <summary>Returns all interfaces on all devices.</summary>
    public Task<RestResult<JsonElement>> GetInterfacesOnAllDevicesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/devices/interfaces", ct);

    /// <summary>Returns interfaces on a specific device.</summary>
    public Task<RestResult<JsonElement>> GetInterfacesOnDeviceAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces", ct);

    /// <summary>Returns a specific interface on a device.</summary>
    public Task<RestResult<JsonElement>> GetInterfaceOnDeviceAsync(string deviceId, string interfaceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}", ct);

    /// <summary>Updates an interface on a device.</summary>
    public Task<RestResult<JsonElement>> UpdateInterfaceOnDeviceAsync(string deviceId, string interfaceId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}", body, ct);

    /// <summary>Returns interface capabilities on a device.</summary>
    public Task<RestResult<JsonElement>> GetInterfacesCapabilitiesOnDeviceAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/capability", ct);

    /// <summary>Returns a specific interface's capabilities on a device.</summary>
    public Task<RestResult<JsonElement>> GetInterfaceCapabilitiesOnDeviceAsync(string deviceId, string interfaceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/capability", ct);

    // ── Ports ──

    /// <summary>Returns all ports on all devices.</summary>
    public Task<RestResult<JsonElement>> GetPortsOnDeviceAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/devices/interfaces/ports", ct);

    /// <summary>Updates ports on devices (bulk).</summary>
    public Task<RestResult<JsonElement>> UpdatePortsOnDeviceAsync(object body, CancellationToken ct = default)
        => PutAsync<JsonElement>("/1/devices/interfaces/ports", body, ct);

    /// <summary>Returns ports on a specific interface.</summary>
    public Task<RestResult<JsonElement>> GetPortsOnInterfaceOnDeviceAsync(string deviceId, string interfaceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports", ct);

    /// <summary>Returns a specific port on an interface.</summary>
    public Task<RestResult<JsonElement>> GetPortOnInterfaceOnDeviceAsync(string deviceId, string interfaceId, string portId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}", ct);

    /// <summary>Updates a specific port on an interface.</summary>
    public Task<RestResult<JsonElement>> UpdatePortOnDeviceAsync(string deviceId, string interfaceId, string portId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}", body, ct);

    /// <summary>Deletes a port on a device.</summary>
    public Task<RestResult> DeletePortOnDeviceAsync(string deviceId, string interfaceId, string portId, CancellationToken ct = default)
        => DeleteAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}", ct);

    /// <summary>Adds a port on a device.</summary>
    public Task<RestResult<JsonElement>> AddPortOnDeviceAsync(string deviceId, string interfaceId, string portId, object body, CancellationToken ct = default)
        => PostAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}", body, ct);

    /// <summary>Gets port nulling status.</summary>
    public Task<RestResult<JsonElement>> GetPortNullingStatusAsync(string deviceId, string interfaceId, string portId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/nulling", ct);

    /// <summary>Starts port nulling.</summary>
    public Task<RestResult> StartPortNullingAsync(string deviceId, string interfaceId, string portId, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/nulling", ct);

    /// <summary>Sets GPO state on a port.</summary>
    public Task<RestResult> SetPortGpoStateAsync(string deviceId, string interfaceId, string portId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/gpo", body, ct);

    /// <summary>Joins a port to a connection.</summary>
    public Task<RestResult> JoinPortToConnectionAsync(string deviceId, string interfaceId, string portId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/join", body, ct);

    /// <summary>Removes a port from a connection.</summary>
    public Task<RestResult> LeavePortToConnectionAsync(string deviceId, string interfaceId, string portId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/leave", body, ct);

    // ── Calls ──

    /// <summary>Gets all active calls for the link group.</summary>
    public Task<RestResult<JsonElement>> GetAllCallsAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/devices/interfaces/ports/calls", ct);

    /// <summary>Hangs up all calls across all devices.</summary>
    public Task<RestResult> HangupDevicesCallsAsync(CancellationToken ct = default)
        => DeleteAsync("/1/devices/interfaces/ports/calls", ct);

    /// <summary>Gets all active calls for a device.</summary>
    public Task<RestResult<JsonElement>> GetCallsForDeviceAsync(string deviceId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/ports/calls", ct);

    /// <summary>Hangs up all calls on a device.</summary>
    public Task<RestResult> HangupDeviceCallsAsync(string deviceId, CancellationToken ct = default)
        => DeleteAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/ports/calls", ct);

    /// <summary>Gets all active calls for a port.</summary>
    public Task<RestResult<JsonElement>> GetCallsForPortAsync(string deviceId, string interfaceId, string portId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/calls", ct);

    /// <summary>Makes a call on a port.</summary>
    public Task<RestResult<JsonElement>> MakeCallAsync(string deviceId, string interfaceId, string portId, object body, CancellationToken ct = default)
        => PostAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/calls", body, ct);

    /// <summary>Gets a single active call.</summary>
    public Task<RestResult<JsonElement>> GetCallAsync(string deviceId, string interfaceId, string portId, string callId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/calls/{Uri.EscapeDataString(callId)}", ct);

    /// <summary>Hangs up a specific call.</summary>
    public Task<RestResult> HangupCallAsync(string deviceId, string interfaceId, string portId, string callId, CancellationToken ct = default)
        => DeleteAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/calls/{Uri.EscapeDataString(callId)}", ct);

    /// <summary>Sends DTMF tones to an active call.</summary>
    public Task<RestResult> SendDTMFAsync(string deviceId, string interfaceId, string portId, string callId, object body, CancellationToken ct = default)
        => PostAsync($"/1/devices/{Uri.EscapeDataString(deviceId)}/interfaces/{Uri.EscapeDataString(interfaceId)}/ports/{Uri.EscapeDataString(portId)}/calls/{Uri.EscapeDataString(callId)}/senddtmf", body, ct);

    // ── Connections ──

    /// <summary>Returns all connections.</summary>
    public Task<RestResult<JsonElement>> GetConnectionsAsync(string? filter = null, CancellationToken ct = default)
        => GetAsync<JsonElement>(filter != null ? $"/1/connections?filter={Uri.EscapeDataString(filter)}" : "/1/connections", ct);

    /// <summary>Adds a connection.</summary>
    public Task<RestResult<JsonElement>> AddConnectionAsync(object body, CancellationToken ct = default)
        => PostAsync<JsonElement>("/1/connections", body, ct);

    /// <summary>Returns live status of all connections.</summary>
    public Task<RestResult<JsonElement>> GetConnectionsLiveStatusAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/connections/liveStatus", ct);

    /// <summary>Returns a connection by ID.</summary>
    public Task<RestResult<JsonElement>> GetConnectionByIdAsync(string connectionId, string? filter = null, CancellationToken ct = default)
        => GetAsync<JsonElement>(filter != null
            ? $"/1/connections/{Uri.EscapeDataString(connectionId)}?filter={Uri.EscapeDataString(filter)}"
            : $"/1/connections/{Uri.EscapeDataString(connectionId)}", ct);

    /// <summary>Updates a connection.</summary>
    public Task<RestResult<JsonElement>> UpdateConnectionAsync(string connectionId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/1/connections/{Uri.EscapeDataString(connectionId)}", body, ct);

    /// <summary>Deletes a connection.</summary>
    public Task<RestResult> DeleteConnectionAsync(string connectionId, CancellationToken ct = default)
        => DeleteAsync($"/1/connections/{Uri.EscapeDataString(connectionId)}", ct);

    /// <summary>Returns live status of a connection.</summary>
    public Task<RestResult<JsonElement>> GetConnectionLiveStatusAsync(string connectionId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/connections/{Uri.EscapeDataString(connectionId)}/liveStatus", ct);

    // ── Events ──

    /// <summary>Gets events.</summary>
    public Task<RestResult<JsonElement>> GetEventsAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/events", ct);

    /// <summary>Purges all events.</summary>
    public Task<RestResult> PurgeEventsAsync(CancellationToken ct = default)
        => DeleteAsync("/1/events", ct);

    /// <summary>Gets events for a specific entity.</summary>
    public Task<RestResult<JsonElement>> GetEventsByEntityAsync(string entityId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/events/{Uri.EscapeDataString(entityId)}", ct);

    /// <summary>Purges events for a specific entity.</summary>
    public Task<RestResult> PurgeEventsByEntityAsync(string entityId, CancellationToken ct = default)
        => DeleteAsync($"/1/events/{Uri.EscapeDataString(entityId)}", ct);

    /// <summary>Exports all events.</summary>
    public Task<RestResult<JsonElement>> ExportEventsAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/events/export", ct);

    /// <summary>Exports events for a specific entity.</summary>
    public Task<RestResult<JsonElement>> ExportEventsByEntityAsync(string entityId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/events/export/{Uri.EscapeDataString(entityId)}", ct);

    // ── Keysets (v2) ──

    /// <summary>Gets all keysets.</summary>
    public Task<RestResult<JsonElement>> GetKeysetsV2Async(CancellationToken ct = default)
        => GetAsync<JsonElement>("/2/keysets", ct);

    /// <summary>Adds a keyset.</summary>
    public Task<RestResult<JsonElement>> AddKeysetV2Async(object body, CancellationToken ct = default)
        => PostAsync<JsonElement>("/2/keysets", body, ct);

    /// <summary>Bulk updates keysets.</summary>
    public Task<RestResult<JsonElement>> UpdateKeysetsBulkAsync(object body, CancellationToken ct = default)
        => PutAsync<JsonElement>("/2/keysets", body, ct);

    /// <summary>Gets a keyset by ID.</summary>
    public Task<RestResult<JsonElement>> GetKeysetByIdV2Async(string keysetId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/2/keysets/{Uri.EscapeDataString(keysetId)}", ct);

    /// <summary>Updates a keyset.</summary>
    public Task<RestResult<JsonElement>> UpdateKeysetV2Async(string keysetId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/2/keysets/{Uri.EscapeDataString(keysetId)}", body, ct);

    // ── Agent-IC / IVP Users ──

    /// <summary>Gets all Agent-IC users.</summary>
    public Task<RestResult<JsonElement>> GetIVPUsersAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/1/ivpusers", ct);

    /// <summary>Adds an Agent-IC user.</summary>
    public Task<RestResult<JsonElement>> AddIVPUserAsync(object body, CancellationToken ct = default)
        => PostAsync<JsonElement>("/1/ivpusers", body, ct);

    /// <summary>Gets an Agent-IC user by ID.</summary>
    public Task<RestResult<JsonElement>> GetAgentICUserByIdAsync(string userId, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/1/ivpusers/{Uri.EscapeDataString(userId)}", ct);

    /// <summary>Updates an Agent-IC user.</summary>
    public Task<RestResult<JsonElement>> UpdateAgentICUserAsync(string userId, object body, CancellationToken ct = default)
        => PutAsync<JsonElement>($"/1/ivpusers/{Uri.EscapeDataString(userId)}", body, ct);

    /// <summary>Deletes an Agent-IC user.</summary>
    public Task<RestResult> DeleteAgentICUserAsync(string userId, CancellationToken ct = default)
        => DeleteAsync($"/1/ivpusers/{Uri.EscapeDataString(userId)}", ct);

    // ── Entities (v2) ──

    /// <summary>Gets all entities.</summary>
    public Task<RestResult<JsonElement>> GetEntitiesAsync(CancellationToken ct = default)
        => GetAsync<JsonElement>("/2/entities", ct);

    /// <summary>Gets a specific entity by ID.</summary>
    public Task<RestResult<JsonElement>> GetEntityAsync(string id, CancellationToken ct = default)
        => GetAsync<JsonElement>($"/2/entity/{Uri.EscapeDataString(id)}", ct);
}
