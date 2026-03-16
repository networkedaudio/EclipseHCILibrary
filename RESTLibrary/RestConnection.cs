using RESTLibrary.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RESTLibrary;

/// <summary>
/// Base class for REST API connections to Clear-Com devices.
/// Manages authentication (JWT bearer token), HTTPS certificate handling,
/// and provides typed helper methods for GET / POST / PUT / DELETE.
/// </summary>
public abstract class RestConnection : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _username;
    private readonly string _password;
    private string? _token;
    private bool _disposed;

    /// <summary>
    /// JSON serializer options shared across all requests.
    /// </summary>
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// The base URI of the device (e.g. https://192.168.1.100).
    /// </summary>
    public Uri BaseAddress => _httpClient.BaseAddress!;

    /// <summary>
    /// Whether the connection has a valid token.
    /// This does not guarantee the token hasn't expired.
    /// </summary>
    public bool IsAuthenticated => !string.IsNullOrEmpty(_token);

    /// <summary>
    /// Event raised when authentication succeeds.
    /// </summary>
    public event EventHandler? Authenticated;

    /// <summary>
    /// Event raised when an API error occurs.
    /// </summary>
    public event EventHandler<string>? ErrorOccurred;

    /// <summary>
    /// Creates a new REST connection.
    /// </summary>
    /// <param name="host">IP address or hostname of the device.</param>
    /// <param name="username">Login username.</param>
    /// <param name="password">Login password.</param>
    /// <param name="port">HTTPS port (default 443).</param>
    /// <param name="useSsl">Whether to use HTTPS (default true).</param>
    protected RestConnection(string host, string username, string password, int port = 443, bool useSsl = true)
    {
        _username = username;
        _password = password;

        var handler = new HttpClientHandler
        {
            // Clear-Com devices typically use self-signed certificates
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };

        string scheme = useSsl ? "https" : "http";
        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri($"{scheme}://{host}:{port}"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// The API base path (e.g. "/api/1" for LQ, "/api" for Arcadia).
    /// Subclasses override this to set the correct prefix.
    /// </summary>
    protected abstract string ApiBasePath { get; }

    /// <summary>
    /// The login endpoint path relative to the device root (e.g. "/api/login").
    /// </summary>
    protected virtual string LoginPath => "/api/login";

    /// <summary>
    /// Authenticates with the device and stores the JWT bearer token.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if authentication succeeded.</returns>
    public async Task<bool> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var loginBody = new { username = _username, password = _password };
            var content = new StringContent(
                JsonSerializer.Serialize(loginBody, JsonOptions),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(LoginPath, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                OnError($"Authentication failed ({(int)response.StatusCode}): {errorBody}");
                return false;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            // Try to parse { "token": "..." } first, fall back to raw string
            try
            {
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(json, JsonOptions);
                _token = authResponse?.Token;
            }
            catch
            {
                // Some firmware versions return the token as a plain quoted string
                _token = json.Trim('"', ' ', '\n', '\r');
            }

            if (string.IsNullOrEmpty(_token))
            {
                OnError("Authentication returned empty token.");
                return false;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            Authenticated?.Invoke(this, EventArgs.Empty);
            return true;
        }
        catch (Exception ex)
        {
            OnError($"Authentication exception: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Builds the full API path from a relative endpoint path.
    /// </summary>
    /// <param name="relativePath">Path relative to the API base (e.g. "/devices").</param>
    protected string BuildPath(string relativePath)
    {
        // If the relative path already starts with the base, don't double it
        if (relativePath.StartsWith(ApiBasePath, StringComparison.OrdinalIgnoreCase))
            return relativePath;

        return $"{ApiBasePath}{relativePath}";
    }

    // ── HTTP helpers ──

    /// <summary>
    /// Sends a GET request and deserializes the response.
    /// </summary>
    protected async Task<RestResult<T>> GetAsync<T>(string path, CancellationToken ct = default)
    {
        return await SendAsync<T>(HttpMethod.Get, path, null, ct);
    }

    /// <summary>
    /// Sends a POST request with a JSON body and deserializes the response.
    /// </summary>
    protected async Task<RestResult<T>> PostAsync<T>(string path, object? body = null, CancellationToken ct = default)
    {
        return await SendAsync<T>(HttpMethod.Post, path, body, ct);
    }

    /// <summary>
    /// Sends a POST request with a JSON body, expecting no meaningful response body.
    /// </summary>
    protected async Task<RestResult> PostAsync(string path, object? body = null, CancellationToken ct = default)
    {
        return await SendAsync(HttpMethod.Post, path, body, ct);
    }

    /// <summary>
    /// Sends a PUT request with a JSON body and deserializes the response.
    /// </summary>
    protected async Task<RestResult<T>> PutAsync<T>(string path, object? body = null, CancellationToken ct = default)
    {
        return await SendAsync<T>(HttpMethod.Put, path, body, ct);
    }

    /// <summary>
    /// Sends a PUT request with a JSON body, expecting no meaningful response body.
    /// </summary>
    protected async Task<RestResult> PutAsync(string path, object? body = null, CancellationToken ct = default)
    {
        return await SendAsync(HttpMethod.Put, path, body, ct);
    }

    /// <summary>
    /// Sends a DELETE request and deserializes the response.
    /// </summary>
    protected async Task<RestResult<T>> DeleteAsync<T>(string path, CancellationToken ct = default)
    {
        return await SendAsync<T>(HttpMethod.Delete, path, null, ct);
    }

    /// <summary>
    /// Sends a DELETE request, expecting no meaningful response body.
    /// </summary>
    protected async Task<RestResult> DeleteAsync(string path, CancellationToken ct = default)
    {
        return await SendAsync(HttpMethod.Delete, path, null, ct);
    }

    /// <summary>
    /// Sends a PATCH request with a JSON body and deserializes the response.
    /// </summary>
    protected async Task<RestResult<T>> PatchAsync<T>(string path, object? body = null, CancellationToken ct = default)
    {
        return await SendAsync<T>(HttpMethod.Patch, path, body, ct);
    }

    /// <summary>
    /// Core send method that returns a typed result.
    /// </summary>
    private async Task<RestResult<T>> SendAsync<T>(HttpMethod method, string path, object? body, CancellationToken ct)
    {
        try
        {
            using var request = new HttpRequestMessage(method, BuildPath(path));

            if (body != null)
            {
                request.Content = new StringContent(
                    JsonSerializer.Serialize(body, JsonOptions),
                    Encoding.UTF8,
                    "application/json");
            }

            var response = await _httpClient.SendAsync(request, ct);
            var rawJson = await response.Content.ReadAsStringAsync(ct);
            int statusCode = (int)response.StatusCode;

            if (!response.IsSuccessStatusCode)
            {
                return RestResult<T>.Failure(statusCode, rawJson, rawJson);
            }

            if (string.IsNullOrWhiteSpace(rawJson))
            {
                return RestResult<T>.Success(default, statusCode);
            }

            var data = JsonSerializer.Deserialize<T>(rawJson, JsonOptions);
            return RestResult<T>.Success(data, statusCode, rawJson);
        }
        catch (Exception ex)
        {
            OnError($"{method} {path}: {ex.Message}");
            return RestResult<T>.Failure(0, ex.Message);
        }
    }

    /// <summary>
    /// Core send method that returns a non-generic result (no deserialized body).
    /// </summary>
    private async Task<RestResult> SendAsync(HttpMethod method, string path, object? body, CancellationToken ct)
    {
        try
        {
            using var request = new HttpRequestMessage(method, BuildPath(path));

            if (body != null)
            {
                request.Content = new StringContent(
                    JsonSerializer.Serialize(body, JsonOptions),
                    Encoding.UTF8,
                    "application/json");
            }

            var response = await _httpClient.SendAsync(request, ct);
            int statusCode = (int)response.StatusCode;

            if (!response.IsSuccessStatusCode)
            {
                var rawJson = await response.Content.ReadAsStringAsync(ct);
                return RestResult.Failure(statusCode, rawJson, rawJson);
            }

            return RestResult.Ok(statusCode);
        }
        catch (Exception ex)
        {
            OnError($"{method} {path}: {ex.Message}");
            return RestResult.Failure(0, ex.Message);
        }
    }

    /// <summary>
    /// Raises the <see cref="ErrorOccurred"/> event.
    /// </summary>
    protected void OnError(string message)
    {
        ErrorOccurred?.Invoke(this, message);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
