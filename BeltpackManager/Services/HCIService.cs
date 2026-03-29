using HCILibrary;
using HCILibrary.HCIRequests;
using HCILibrary.HCIResponses;
using HCILibrary.Models;
using HCILibrary.Enums;
using System.Diagnostics;

namespace BeltpackManager.Services;

/// <summary>
/// Service for managing HCI connection to the Eclipse matrix.
/// </summary>
public class HCIService : IAsyncDisposable
{
    private HCIConnection? _connection;
    private readonly object _lock = new();
    private TaskCompletionSource? _connectionReadyTcs;

    /// <summary>
    /// Event raised when a message is received from the matrix.
    /// </summary>
    public event EventHandler<HCIReply>? MessageReceived;

    /// <summary>
    /// Event raised when the connection state changes.
    /// </summary>
    public event EventHandler<bool>? ConnectionStateChanged;

    /// <summary>
    /// Gets whether the service is currently connected.
    /// </summary>
    public bool IsConnected => _connection?.IsConnected ?? false;

    /// <summary>
    /// Connects to the HCI matrix at the specified IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address of the matrix.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if connected successfully, false otherwise.</returns>
    public async Task<bool> ConnectAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_connection != null)
            {
                return _connection.IsConnected;
            }

            _connectionReadyTcs = new TaskCompletionSource();
            _connection = new HCIConnection(ipAddress);
            _connection.MessageReceived += OnMessageReceived;
            _connection.ConnectionStateChanged += OnConnectionStateChanged;
        }

        bool connected = await _connection.ConnectAsync(cancellationToken);

        if (connected)
        {
            _connectionReadyTcs?.TrySetResult();
        }
        else
        {
            _connectionReadyTcs?.TrySetException(new Exception("Failed to connect"));
            await DisconnectAsync();
        }

        return connected;
    }

    /// <summary>
    /// Disconnects from the matrix.
    /// </summary>
    public async Task DisconnectAsync()
    {
        HCIConnection? conn;
        lock (_lock)
        {
            conn = _connection;
            _connection = null;
            _connectionReadyTcs = null;
        }

        if (conn != null)
        {
            await conn.DisconnectAsync();
            conn.Dispose();
        }
    }

    /// <summary>
    /// Requests beltpack information from the matrix.
    /// </summary>
    /// <returns>The beltpack info reply, or null if the request failed.</returns>
    public async Task<ReplyBeltpackInformation?> GetBeltpackInformationAsync()
    {
        return await GetBeltpackInformationAsync(RequestBeltpackInformationRequest.AllEntries());
    }

    /// <summary>
    /// Requests Map/OTA beltpack information from the matrix.
    /// </summary>
    /// <returns>The beltpack info reply for Map/OTA entries, or null if the request failed.</returns>
    public async Task<ReplyBeltpackInformation?> GetMapOtaBeltpacksAsync()
    {
        return await GetBeltpackInformationAsync(RequestBeltpackInformationRequest.MapOrOtaEntries());
    }

    /// <summary>
    /// Requests HCI-added beltpack information from the matrix.
    /// </summary>
    /// <returns>The beltpack info reply for HCI-added entries, or null if the request failed.</returns>
    public async Task<ReplyBeltpackInformation?> GetHciAddedBeltpacksAsync()
    {
        return await GetBeltpackInformationAsync(RequestBeltpackInformationRequest.HciAddedEntries());
    }

    /// <summary>
    /// Internal method to request beltpack information with a specific request type.
    /// </summary>
    private async Task<ReplyBeltpackInformation?> GetBeltpackInformationAsync(RequestBeltpackInformationRequest request)
    {
        if (_connection == null || !_connection.IsConnected)
        {
            Console.WriteLine("[HCIService] Not connected");
            return null;
        }

        // Wait for connection to be ready
        if (_connectionReadyTcs != null)
        {
            await _connectionReadyTcs.Task;
        }

        var requestTypeDesc = request.ProtocolSchema == 2 
            ? $"Schema 2, RequestType={(int?)request.RequestType}" 
            : "Schema 1 (All entries)";
        Console.WriteLine($"[HCIService] Sending RequestBeltpackInformation (Message ID 0x0101), {requestTypeDesc}");

        // Create a TaskCompletionSource to wait for the complete reply
        var tcs = new TaskCompletionSource<ReplyBeltpackInformation?>();

        // Accumulate beltpack entries from all message fragments
        var allBeltpacks = new List<BeltpackInformationEntry>();
        int fragmentCount = 0;
        byte protocolSchema = 1;

        // Track ALL messages to see what we're getting
        int messageCount = 0;
        var receivedMessageIds = new HashSet<HCIMessageID>();

        EventHandler<HCIReply>? handler = null;
        handler = (sender, reply) =>
        {
            messageCount++;
            receivedMessageIds.Add(reply.MessageID);

            // Log every 10th message or important ones
            if (messageCount % 10 == 0 || reply.MessageID == HCIMessageID.ReplyBeltpackInformation || reply.MessageID == HCIMessageID.ReplyBeltpackStatus)
            {
                Console.WriteLine($"[HCIService] Message #{messageCount}: ID={reply.MessageID} (0x{(int)reply.MessageID:X4}), HasBeltpackInformation={reply.BeltpackInformation != null}");
            }

            if (reply.MessageID == HCIMessageID.ReplyBeltpackInformation)
            {
                fragmentCount++;
                Console.WriteLine($"[HCIService] Got ReplyBeltpackInformation fragment #{fragmentCount}! IsNull={reply.BeltpackInformation == null}, M flag={reply.Flags.M}");

                if (reply.BeltpackInformation != null)
                {
                    protocolSchema = reply.BeltpackInformation.ProtocolSchema;
                    int countInFragment = reply.BeltpackInformation.Beltpacks.Count;
                    allBeltpacks.AddRange(reply.BeltpackInformation.Beltpacks);

                    Console.WriteLine($"[HCIService] Fragment #{fragmentCount}: Schema={protocolSchema}, Count={reply.BeltpackInformation.Count}, Beltpacks in fragment={countInFragment}, Total accumulated={allBeltpacks.Count}");

                    // Check if more fragments are expected (M flag = More data)
                    if (!reply.Flags.M)
                    {
                        // Last fragment - create aggregated result
                        Console.WriteLine($"[HCIService] Last fragment received (M=false). Creating aggregated result with {allBeltpacks.Count} total beltpacks from {fragmentCount} fragments");

                        var aggregatedResult = new ReplyBeltpackInformation
                        {
                            ProtocolSchema = protocolSchema,
                            Beltpacks = allBeltpacks
                        };

                        _connection!.MessageReceived -= handler;
                        tcs.TrySetResult(aggregatedResult);
                    }
                    else
                    {
                        Console.WriteLine($"[HCIService] More fragments expected (M=true), continuing to listen...");
                    }
                }
            }
        };

        _connection.MessageReceived += handler;

        try
        {
            // Send the request
            _connection.RequestQueue?.Enqueue(request);
            Console.WriteLine($"[HCIService] Request enqueued");

            // Wait for response with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            cts.Token.Register(() => 
            {
                Console.WriteLine($"[HCIService] Timeout after {messageCount} messages. Received message types: {string.Join(", ", receivedMessageIds.Select(id => $"{id}(0x{(int)id:X4})"))}");
                Console.WriteLine($"[HCIService] Received {fragmentCount} ReplyBeltpackInformation fragments with {allBeltpacks.Count} total beltpacks");

                // If we got at least one fragment, return what we have
                if (allBeltpacks.Count > 0)
                {
                    Console.WriteLine($"[HCIService] Returning partial result due to timeout");
                    var partialResult = new ReplyBeltpackInformation
                    {
                        ProtocolSchema = protocolSchema,
                        Beltpacks = allBeltpacks
                    };
                    tcs.TrySetResult(partialResult);
                }
                else
                {
                    Console.WriteLine($"[HCIService] No ReplyBeltpackInformation (0x0102) received");
                    tcs.TrySetResult(null);
                }
            });

            var result = await tcs.Task;
            Console.WriteLine($"[HCIService] GetBeltpackInformationAsync returning: {(result == null ? "null" : $"Count={result.Count}")}");
            return result;
        }
        finally
        {
            _connection.MessageReceived -= handler;
        }
    }

    private void OnMessageReceived(object? sender, HCIReply reply)
    {
        Debug.WriteLine($"[HCIService] OnMessageReceived: MessageID=0x{(int)reply.MessageID:X4} ({reply.MessageID}), HasBeltpackStatus={reply.BeltpackStatus != null}, HasEvent={reply.Event != null}, Flags E={reply.Flags.E} M={reply.Flags.M}");
        MessageReceived?.Invoke(this, reply);
    }

    private void OnConnectionStateChanged(object? sender, bool connected)
    {
        ConnectionStateChanged?.Invoke(this, connected);
    }

    /// <summary>
    /// Adds a beltpack to the matrix.
    /// </summary>
    /// <param name="request">The beltpack add request.</param>
    /// <returns>The reply indicating success or failure, or null if the request failed.</returns>
    public async Task<ReplyBeltpackAdd?> AddBeltpackAsync(RequestBeltpackAddRequest request)
    {
        if (_connection == null || !_connection.IsConnected)
        {
            Console.WriteLine("[HCIService] Not connected");
            return null;
        }

        // Wait for connection to be ready
        if (_connectionReadyTcs != null)
        {
            await _connectionReadyTcs.Task;
        }

        Console.WriteLine($"[HCIService] Sending RequestBeltpackAdd (Message ID 0x0193) for serial {request.SerialNumber}");

        // Create a TaskCompletionSource to wait for the reply
        var tcs = new TaskCompletionSource<ReplyBeltpackAdd?>();

        EventHandler<HCIReply>? handler = null;
        handler = (sender, reply) =>
        {
            if (reply.MessageID == HCIMessageID.ReplyBeltpackAdd)
            {
                Console.WriteLine($"[HCIService] Got ReplyBeltpackAdd! IsNull={reply.BeltpackAdd == null}");

                if (reply.BeltpackAdd != null)
                {
                    Console.WriteLine($"[HCIService] BeltpackAdd result: Serial={reply.BeltpackAdd.SerialNumber:X8}, Result={reply.BeltpackAdd.Result}");
                }

                _connection!.MessageReceived -= handler;
                tcs.TrySetResult(reply.BeltpackAdd);
            }
        };

        _connection.MessageReceived += handler;

        try
        {
            // Send the request
            _connection.RequestQueue?.Enqueue(request);
            Console.WriteLine($"[HCIService] Request enqueued");

            // Wait for response with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            cts.Token.Register(() => 
            {
                Console.WriteLine($"[HCIService] Timeout waiting for ReplyBeltpackAdd");
                tcs.TrySetResult(null);
            });

            var result = await tcs.Task;
            Console.WriteLine($"[HCIService] AddBeltpackAsync returning: {(result == null ? "null" : $"Result={result.Result}")}");
            return result;
        }
        finally
        {
            _connection.MessageReceived -= handler;
        }
    }

    /// <summary>
    /// Deletes a beltpack from the matrix by PMID.
    /// </summary>
    /// <param name="pmid">The PMID of the beltpack to delete.</param>
    /// <returns>The reply indicating success or failure, or null if the request failed.</returns>
    public async Task<ReplyBeltpackDelete?> DeleteBeltpackAsync(uint pmid)
    {
        if (_connection == null || !_connection.IsConnected)
        {
            Console.WriteLine("[HCIService] Not connected");
            return null;
        }

        // Wait for connection to be ready
        if (_connectionReadyTcs != null)
        {
            await _connectionReadyTcs.Task;
        }

        var request = new RequestBeltpackDeleteRequest(pmid);
        Console.WriteLine($"[HCIService] Sending RequestBeltpackDelete (Message ID 0x0195) for PMID {pmid:X8}");

        // Create a TaskCompletionSource to wait for the reply
        var tcs = new TaskCompletionSource<ReplyBeltpackDelete?>();

        EventHandler<HCIReply>? handler = null;
        handler = (sender, reply) =>
        {
            if (reply.MessageID == HCIMessageID.ReplyBeltpackDelete)
            {
                Console.WriteLine($"[HCIService] Got ReplyBeltpackDelete! IsNull={reply.BeltpackDelete == null}");

                if (reply.BeltpackDelete != null)
                {
                    Console.WriteLine($"[HCIService] BeltpackDelete result: Success={reply.BeltpackDelete.Success}");
                }

                _connection!.MessageReceived -= handler;
                tcs.TrySetResult(reply.BeltpackDelete);
            }
        };

        _connection.MessageReceived += handler;

        try
        {
            // Send the request
            _connection.RequestQueue?.Enqueue(request);
            Console.WriteLine($"[HCIService] Request enqueued");

            // Wait for response with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            cts.Token.Register(() => 
            {
                Console.WriteLine($"[HCIService] Timeout waiting for ReplyBeltpackDelete");
                tcs.TrySetResult(null);
            });

            var result = await tcs.Task;
            Console.WriteLine($"[HCIService] DeleteBeltpackAsync returning: {(result == null ? "null" : $"Success={result.Success}")}");
            return result;
        }
        finally
        {
            _connection.MessageReceived -= handler;
        }
    }

    /// <summary>
    /// Requests system status (card information) from the matrix.
    /// </summary>
    /// <returns>The system card status reply, or null if the request failed.</returns>
    public async Task<ReplySystemCardStatus?> GetSystemStatusAsync()
    {
        if (_connection == null || !_connection.IsConnected)
        {
            Console.WriteLine("[HCIService] Not connected");
            return null;
        }

        // Wait for connection to be ready
        if (_connectionReadyTcs != null)
        {
            await _connectionReadyTcs.Task;
        }

        var request = new RequestSystemStatusRequest();
        Console.WriteLine($"[HCIService] Sending RequestSystemStatus (Message ID 0x0003)");

        // Create a TaskCompletionSource to wait for the reply
        var tcs = new TaskCompletionSource<ReplySystemCardStatus?>();

        EventHandler<HCIReply>? handler = null;
        handler = (sender, reply) =>
        {
            if (reply.MessageID == HCIMessageID.ReplySystemCardStatus)
            {
                Console.WriteLine($"[HCIService] Got ReplySystemCardStatus! IsNull={reply.SystemCardStatus == null}");

                if (reply.SystemCardStatus != null)
                {
                    Console.WriteLine($"[HCIService] SystemCardStatus: {reply.SystemCardStatus.Count} cards");
                }

                _connection!.MessageReceived -= handler;
                tcs.TrySetResult(reply.SystemCardStatus);
            }
        };

        _connection.MessageReceived += handler;

        try
        {
            // Send the request
            _connection.RequestQueue?.Enqueue(request);
            Console.WriteLine($"[HCIService] Request enqueued");

            // Wait for response with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            cts.Token.Register(() => 
            {
                Console.WriteLine($"[HCIService] Timeout waiting for ReplySystemCardStatus");
                tcs.TrySetResult(null);
            });

            var result = await tcs.Task;
            Console.WriteLine($"[HCIService] GetSystemStatusAsync returning: {(result == null ? "null" : $"Count={result.Count}")}");
            return result;
        }
        finally
        {
            _connection.MessageReceived -= handler;
        }
    }

    /// <summary>
    /// Requests peripheral info for a specific slot or wireless devices.
    /// </summary>
    /// <param name="slotId">The slot ID (0-254), or 0xFF for wireless devices.</param>
    /// <returns>The peripheral info reply, or null if the request failed.</returns>
    public async Task<ReplyPeripheralInfo?> GetPeripheralInfoAsync(byte slotId)
    {
        if (_connection == null || !_connection.IsConnected)
        {
            Console.WriteLine("[HCIService] Not connected");
            return null;
        }

        // Wait for connection to be ready
        if (_connectionReadyTcs != null)
        {
            await _connectionReadyTcs.Task;
        }

        var request = new RequestPeripheralInfoRequest(slotId);
        Console.WriteLine($"[HCIService] Sending RequestPeripheralInfo (Message ID 0x00F7) for slot {slotId} (0x{slotId:X2})");

        // Create a TaskCompletionSource to wait for the reply
        var tcs = new TaskCompletionSource<ReplyPeripheralInfo?>();

        EventHandler<HCIReply>? handler = null;
        handler = (sender, reply) =>
        {
            if (reply.MessageID == HCIMessageID.ReplyPeripheralInfo)
            {
                Console.WriteLine($"[HCIService] Got ReplyPeripheralInfo! IsNull={reply.PeripheralInfo == null}");

                if (reply.PeripheralInfo != null)
                {
                    Console.WriteLine($"[HCIService] PeripheralInfo: {reply.PeripheralInfo.Count} entries for slot {reply.PeripheralInfo.RequestedSlotNumber}");
                }

                _connection!.MessageReceived -= handler;
                tcs.TrySetResult(reply.PeripheralInfo);
            }
        };

        _connection.MessageReceived += handler;

        try
        {
            // Send the request
            _connection.RequestQueue?.Enqueue(request);
            Console.WriteLine($"[HCIService] Request enqueued");

            // Wait for response with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            cts.Token.Register(() => 
            {
                Console.WriteLine($"[HCIService] Timeout waiting for ReplyPeripheralInfo");
                tcs.TrySetResult(null);
            });

            var result = await tcs.Task;
            Console.WriteLine($"[HCIService] GetPeripheralInfoAsync returning: {(result == null ? "null" : $"Count={result.Count}")}");
            return result;
        }
        finally
        {
            _connection.MessageReceived -= handler;
        }
    }

    /// <summary>
    /// Requests peripheral info for wireless devices (beltpacks, antennas).
    /// </summary>
    /// <returns>The peripheral info reply for wireless devices, or null if the request failed.</returns>
    public async Task<ReplyPeripheralInfo?> GetWirelessPeripheralInfoAsync()
    {
        return await GetPeripheralInfoAsync(RequestPeripheralInfoRequest.WirelessDeviceSlot);
    }

    /// <summary>
    /// Requests role state information from the matrix.
    /// </summary>
    /// <returns>The role state reply containing all roles, or null if the request failed.</returns>
    public async Task<ReplyRoleState?> GetRolesAsync()
    {
        if (_connection == null || !_connection.IsConnected)
        {
            Console.WriteLine("[HCIService] Not connected");
            return null;
        }

        // Wait for connection to be ready
        if (_connectionReadyTcs != null)
        {
            await _connectionReadyTcs.Task;
        }

        var request = new RequestRoleStateRequest(); // Defaults to all roles (0xFFFF)
        Console.WriteLine($"[HCIService] Sending RequestRoleState (Message ID 0x0184) for all roles");

        // Create a TaskCompletionSource to wait for the complete reply
        var tcs = new TaskCompletionSource<ReplyRoleState?>();

        // Accumulate role entries from all message fragments
        var allRoles = new List<RoleStateEntry>();
        int fragmentCount = 0;
        byte protocolSchema = 1;

        EventHandler<HCIReply>? handler = null;
        handler = (sender, reply) =>
        {
            if (reply.MessageID == HCIMessageID.ReplyRoleState)
            {
                fragmentCount++;
                Console.WriteLine($"[HCIService] Got ReplyRoleState fragment #{fragmentCount}! IsNull={reply.RoleState == null}, M flag={reply.Flags.M}");

                if (reply.RoleState != null)
                {
                    protocolSchema = reply.RoleState.ProtocolSchema;
                    int countInFragment = reply.RoleState.Roles.Count;

                    // Convert physical ports from 0-indexed (protocol) to 1-indexed (UI)
                    // Note: Role numbers are NOT converted - they are actual role IDs (600, 601, etc.)
                    foreach (var role in reply.RoleState.Roles)
                    {
                        // Convert physical port from 0-based to 1-based (but NOT role number)
                        if (role.PhysicalPort != 0xFFFF)
                        {
                            role.PhysicalPort = (ushort)(role.PhysicalPort + 1);
                        }
                    }

                    allRoles.AddRange(reply.RoleState.Roles);

                    Console.WriteLine($"[HCIService] Fragment #{fragmentCount}: Schema={protocolSchema}, Roles in fragment={countInFragment}, Total accumulated={allRoles.Count}");

                    // Check if more fragments are expected (M flag = More data)
                    if (!reply.Flags.M)
                    {
                        // Last fragment - create aggregated result
                        Console.WriteLine($"[HCIService] Last fragment received (M=false). Creating aggregated result with {allRoles.Count} total roles from {fragmentCount} fragments");

                        var aggregatedResult = new ReplyRoleState
                        {
                            ProtocolSchema = protocolSchema,
                            Roles = allRoles
                        };

                        _connection!.MessageReceived -= handler;
                        tcs.TrySetResult(aggregatedResult);
                    }
                    else
                    {
                        Console.WriteLine($"[HCIService] More fragments expected (M=true), continuing to listen...");
                    }
                }
            }
        };

        _connection.MessageReceived += handler;

        try
        {
            // Send the request
            _connection.RequestQueue?.Enqueue(request);
            Console.WriteLine($"[HCIService] Request enqueued");

            // Wait for response with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            cts.Token.Register(() => 
            {
                Console.WriteLine($"[HCIService] Timeout waiting for ReplyRoleState");

                // If we got at least one fragment, return what we have
                if (allRoles.Count > 0)
                {
                    Console.WriteLine($"[HCIService] Returning partial result due to timeout");
                    var partialResult = new ReplyRoleState
                    {
                        ProtocolSchema = protocolSchema,
                        Roles = allRoles
                    };
                    tcs.TrySetResult(partialResult);
                }
                else
                {
                    tcs.TrySetResult(null);
                }
            });

            var result = await tcs.Task;
            Console.WriteLine($"[HCIService] GetRolesAsync returning: {(result == null ? "null" : $"Count={result.Roles.Count}")}");
            return result;
        }
        finally
        {
            _connection.MessageReceived -= handler;
        }
    }

    /// <summary>
    /// Updates the state of a role (allocation status and physical port).
    /// </summary>
    /// <param name="role">The role number to update (actual role ID, e.g., 600, 601).</param>
    /// <param name="newStatus">The new allocation status.</param>
    /// <param name="physicalPort">The physical port number (1-indexed, 0xFFFF if not applicable).</param>
    /// <returns>The reply indicating success or failure, or null if the request failed.</returns>
    public async Task<ReplyRoleStateSet?> UpdateRoleStateAsync(ushort role, RoleAllocationStatus newStatus, ushort physicalPort = 0xFFFF)
    {
        if (_connection == null || !_connection.IsConnected)
        {
            Console.WriteLine("[HCIService] Not connected");
            return null;
        }

        // Wait for connection to be ready
        if (_connectionReadyTcs != null)
        {
            await _connectionReadyTcs.Task;
        }

        // Convert physical port from 1-indexed (UI) to 0-indexed (protocol)
        // Note: Role numbers are NOT converted - they are actual role IDs (600, 601, etc.)
        ushort protocolPort = (physicalPort != 0xFFFF && physicalPort > 0) 
            ? (ushort)(physicalPort - 1) 
            : physicalPort;

        var request = new RequestRoleStateSetRequest(role, newStatus, protocolPort);
        Console.WriteLine($"[HCIService] Sending RequestRoleStateSet (Message ID 0x0186) for role {role}, status={newStatus}, port={physicalPort} (protocol: {protocolPort})");

        // Create a TaskCompletionSource to wait for the reply
        var tcs = new TaskCompletionSource<ReplyRoleStateSet?>();

        EventHandler<HCIReply>? handler = null;
        handler = (sender, reply) =>
        {
            if (reply.MessageID == HCIMessageID.ReplyRoleStateSet)
            {
                Console.WriteLine($"[HCIService] Got ReplyRoleStateSet! IsNull={reply.RoleStateSet == null}");

                if (reply.RoleStateSet != null)
                {
                    // Store the requested status for error message generation
                    reply.RoleStateSet.RequestedStatus = newStatus;

                    // Convert physical port from 0-indexed (protocol) to 1-indexed (UI)
                    // Note: Role number is NOT converted - it's the actual role ID
                    if (reply.RoleStateSet.PhysicalPort != 0xFFFF)
                    {
                        reply.RoleStateSet.PhysicalPort = (ushort)(reply.RoleStateSet.PhysicalPort + 1);
                    }

                    Console.WriteLine($"[HCIService] RoleStateSet result: Role={reply.RoleStateSet.Role}, Status={reply.RoleStateSet.CurrentStatus}, Port={reply.RoleStateSet.PhysicalPort}, Success={reply.RoleStateSet.Success}");
                }

                _connection!.MessageReceived -= handler;
                tcs.TrySetResult(reply.RoleStateSet);
            }
        };

        _connection.MessageReceived += handler;

        try
        {
            // Send the request
            _connection.RequestQueue?.Enqueue(request);
            Console.WriteLine($"[HCIService] Request enqueued");

            // Wait for response with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            cts.Token.Register(() => 
            {
                Console.WriteLine($"[HCIService] Timeout waiting for ReplyRoleStateSet");
                tcs.TrySetResult(null);
            });

            var result = await tcs.Task;
            Console.WriteLine($"[HCIService] UpdateRoleStateAsync returning: {(result == null ? "null" : $"Success={result.Success}")}");
            return result;
        }
        finally
        {
            _connection.MessageReceived -= handler;
        }
    }

    /// <summary>
    /// Requests current beltpack status from the matrix.
    /// Sends RequestPanelStatus (0x0005) which triggers ReplyBeltpackStatus (0x004C) responses
    /// containing the online/offline state of all beltpacks.
    /// </summary>
    /// <returns>The aggregated beltpack status reply, or null if the request failed.</returns>
    public async Task<ReplyBeltpackStatus?> GetBeltpackStatusAsync()
    {
        if (_connection == null || !_connection.IsConnected)
        {
            Console.WriteLine("[HCIService] Not connected");
            return null;
        }

        // Wait for connection to be ready
        if (_connectionReadyTcs != null)
        {
            await _connectionReadyTcs.Task;
        }

        var request = new RequestPanelStatusRequest();
        Console.WriteLine($"[HCIService] Sending RequestPanelStatus (Message ID 0x0005) to get beltpack status");

        // Create a TaskCompletionSource to wait for the complete reply
        var tcs = new TaskCompletionSource<ReplyBeltpackStatus?>();

        // Accumulate beltpack status entries from all message fragments
        var allEntries = new List<BeltpackStatusEntry>();
        int fragmentCount = 0;

        EventHandler<HCIReply>? handler = null;
        handler = (sender, reply) =>
        {
            if (reply.MessageID == HCIMessageID.ReplyBeltpackStatus && reply.BeltpackStatus != null)
            {
                fragmentCount++;
                Console.WriteLine($"[HCIService] Got ReplyBeltpackStatus fragment #{fragmentCount}! Entries={reply.BeltpackStatus.Entries.Count}, M flag={reply.Flags.M}");

                allEntries.AddRange(reply.BeltpackStatus.Entries);

                // Check if more fragments are expected (M flag = More data)
                if (!reply.Flags.M)
                {
                    Console.WriteLine($"[HCIService] Last beltpack status fragment (M=false). Total entries={allEntries.Count} from {fragmentCount} fragments");

                    var aggregatedResult = new ReplyBeltpackStatus
                    {
                        Schema = reply.BeltpackStatus.Schema,
                        Entries = allEntries
                    };

                    _connection!.MessageReceived -= handler;
                    tcs.TrySetResult(aggregatedResult);
                }
            }
        };

        _connection.MessageReceived += handler;

        try
        {
            // Send the request
            _connection.RequestQueue?.Enqueue(request);
            Console.WriteLine($"[HCIService] Request enqueued");

            // Wait for response with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            cts.Token.Register(() =>
            {
                Console.WriteLine($"[HCIService] Timeout waiting for ReplyBeltpackStatus. Got {fragmentCount} fragments with {allEntries.Count} entries");

                if (allEntries.Count > 0)
                {
                    var partialResult = new ReplyBeltpackStatus
                    {
                        Entries = allEntries
                    };
                    tcs.TrySetResult(partialResult);
                }
                else
                {
                    tcs.TrySetResult(null);
                }
            });

            var result = await tcs.Task;
            Console.WriteLine($"[HCIService] GetBeltpackStatusAsync returning: {(result == null ? "null" : $"Entries={result.Entries.Count}")}");
            return result;
        }
        finally
        {
            _connection.MessageReceived -= handler;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }
}
