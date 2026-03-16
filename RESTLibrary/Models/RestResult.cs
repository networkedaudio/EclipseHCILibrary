using System.Text.Json.Serialization;

namespace RESTLibrary.Models;

/// <summary>
/// Result wrapper for all REST API calls.
/// </summary>
/// <typeparam name="T">The expected response payload type.</typeparam>
public class RestResult<T>
{
    /// <summary>
    /// Whether the request completed with a success status code.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// The HTTP status code returned by the device.
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// The deserialized response body, or default if the request failed or had no body.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// The raw JSON response body. Useful for debugging or when <typeparamref name="T"/>
    /// cannot capture the full shape.
    /// </summary>
    public string? RawJson { get; init; }

    /// <summary>
    /// An error message when <see cref="IsSuccess"/> is false.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static RestResult<T> Success(T? data, int statusCode, string? rawJson = null) => new()
    {
        IsSuccess = true,
        StatusCode = statusCode,
        Data = data,
        RawJson = rawJson
    };

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static RestResult<T> Failure(int statusCode, string? error, string? rawJson = null) => new()
    {
        IsSuccess = false,
        StatusCode = statusCode,
        Error = error,
        RawJson = rawJson
    };
}

/// <summary>
/// Non-generic result for endpoints that return no meaningful body.
/// </summary>
public class RestResult : RestResult<object>
{
    /// <summary>
    /// Creates a successful result with no data.
    /// </summary>
    public static RestResult Ok(int statusCode) => new()
    {
        IsSuccess = true,
        StatusCode = statusCode
    };

    /// <summary>
    /// Creates a failed result with no data.
    /// </summary>
    public new static RestResult Failure(int statusCode, string? error, string? rawJson = null) => new()
    {
        IsSuccess = false,
        StatusCode = statusCode,
        Error = error,
        RawJson = rawJson
    };
}

/// <summary>
/// JWT authentication response from the device login endpoint.
/// </summary>
public class AuthResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}
