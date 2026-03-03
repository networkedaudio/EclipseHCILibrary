using System.Net;
using System.Net.Sockets;
using EclipseHXSNMP.Models;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;

namespace EclipseHXSNMP;

/// <summary>
/// Lightweight SNMPv2c agent that serves Eclipse HX matrix status data.
/// Listens on a UDP port and responds to GET and GETNEXT requests.
/// </summary>
public class EclipseHxSnmpAgent : IDisposable
{
    private readonly EclipseHxSnmpObjectStore _store;
    private readonly OctetString _community;
    private readonly int _port;
    private UdpClient? _listener;
    private CancellationTokenSource? _cts;
    private Task? _listenTask;
    private bool _disposed;

    /// <summary>
    /// Event raised when a request is received.
    /// </summary>
    public event EventHandler<string>? RequestReceived;

    /// <summary>
    /// Event raised when an error occurs.
    /// </summary>
    public event EventHandler<Exception>? ErrorOccurred;

    /// <summary>
    /// Creates a new Eclipse HX SNMP agent.
    /// </summary>
    /// <param name="matrixStatus">The matrix status data source.</param>
    /// <param name="community">SNMP community string (default: "public").</param>
    /// <param name="port">UDP port to listen on (default: 161).</param>
    public EclipseHxSnmpAgent(EclipseHxMatrixStatus matrixStatus, string community = "public", int port = 161)
    {
        _store = new EclipseHxSnmpObjectStore(matrixStatus);
        _community = new OctetString(community);
        _port = port;
    }

    /// <summary>
    /// Gets the underlying object store for direct access.
    /// </summary>
    public EclipseHxSnmpObjectStore ObjectStore => _store;

    /// <summary>
    /// Starts listening for SNMP requests.
    /// </summary>
    public void Start()
    {
        if (_listenTask != null && !_listenTask.IsCompleted)
            return;

        _store.Refresh();

        _cts = new CancellationTokenSource();
        _listener = new UdpClient(new IPEndPoint(IPAddress.Any, _port));
        _listenTask = Task.Run(() => ListenLoopAsync(_cts.Token));
    }

    /// <summary>
    /// Stops the SNMP agent.
    /// </summary>
    public async Task StopAsync()
    {
        _cts?.Cancel();
        _listener?.Close();

        if (_listenTask != null)
        {
            try { await _listenTask; } catch (OperationCanceledException) { }
        }
    }

    /// <summary>
    /// Refreshes the SNMP object store from the current matrix status.
    /// Call this after updating the <see cref="EclipseHxMatrixStatus"/>.
    /// </summary>
    public void RefreshStore() => _store.Refresh();

    private async Task ListenLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var result = await _listener!.ReceiveAsync(ct);
                _ = Task.Run(() => HandleRequest(result.Buffer, result.RemoteEndPoint), ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex);
            }
        }
    }

    private void HandleRequest(byte[] data, IPEndPoint remote)
    {
        try
        {
            var request = MessageFactory.ParseMessages(data, 0, data.Length, new UserRegistry())[0];

            RequestReceived?.Invoke(this, $"{request.TypeCode()} from {remote}");

            if (request is GetRequestMessage getRequest)
            {
                var responseVars = ProcessGet(getRequest.Variables());
                var response = new ResponseMessage(
                    getRequest.RequestId(),
                    getRequest.Version,
                    _community,
                    ErrorCode.NoError,
                    0,
                    responseVars);

                var responseBytes = response.ToBytes();
                _listener?.Send(responseBytes, responseBytes.Length, remote);
            }
            else if (request is GetNextRequestMessage getNextRequest)
            {
                var responseVars = ProcessGetNext(getNextRequest.Variables());
                var response = new ResponseMessage(
                    getNextRequest.RequestId(),
                    getNextRequest.Version,
                    _community,
                    ErrorCode.NoError,
                    0,
                    responseVars);

                var responseBytes = response.ToBytes();
                _listener?.Send(responseBytes, responseBytes.Length, remote);
            }
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex);
        }
    }

    private List<Variable> ProcessGet(IList<Variable> requestVars)
    {
        var result = new List<Variable>();
        foreach (var reqVar in requestVars)
        {
            var found = _store.GetVariable(reqVar.Id);
            result.Add(found ?? new Variable(reqVar.Id, new NoSuchInstance()));
        }
        return result;
    }

    private List<Variable> ProcessGetNext(IList<Variable> requestVars)
    {
        var result = new List<Variable>();
        foreach (var reqVar in requestVars)
        {
            var found = _store.GetNextVariable(reqVar.Id);
            result.Add(found ?? new Variable(reqVar.Id, new EndOfMibView()));
        }
        return result;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _cts?.Cancel();
        _listener?.Dispose();
        _cts?.Dispose();
    }
}
