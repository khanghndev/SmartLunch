using System.Net;

namespace Khoa_Luan_KS_Web.Services;

/// <summary>
/// Gọi API backend kèm Bearer; tự refresh token một lần khi nhận 401.
/// </summary>
internal static class BackendApiAuthHelper
{
    public static async Task<T> SendWithRefreshAsync<T>(
        IApiTokenService? tokenService,
        string accessToken,
        Func<string, CancellationToken, Task<T>> send,
        CancellationToken cancellationToken)
    {
        try
        {
            return await send(accessToken, cancellationToken);
        }
        catch (BackendUnauthorizedException)
        {
            if (tokenService == null)
                throw;

            var newToken = await tokenService.ForceRefreshAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(newToken))
                throw;

            return await send(newToken, cancellationToken);
        }
    }

    public static async Task<T> HandleResponseAsync<T>(
        HttpResponseMessage res,
        CancellationToken cancellationToken,
        Func<string, string, Task<T>>? tryExtract = null)
    {
        var body = await res.Content.ReadAsStringAsync(cancellationToken);
        if (res.StatusCode == HttpStatusCode.Unauthorized)
            throw new BackendUnauthorizedException(TryExtractBackendMessage(body) ?? "Unauthorized");

        if (!res.IsSuccessStatusCode)
        {
            var msg = TryExtractBackendMessage(body) ?? $"Backend request failed ({(int)res.StatusCode})";
            throw new InvalidOperationException(msg);
        }

        return await DeserializeEnvelopeAsync<T>(body);
    }

    private static async Task<T> DeserializeEnvelopeAsync<T>(string body)
    {
        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var envelope = System.Text.Json.JsonSerializer.Deserialize<BaseApiResponse<T>>(body, options);
        if (envelope == null || envelope.Data == null)
        {
            if (typeof(T) == typeof(object))
                return (T)(object)new { };
            throw new InvalidOperationException("Invalid response from backend");
        }

        return envelope.Data;
    }

    private static string? TryExtractBackendMessage(string body)
    {
        try
        {
            var doc = System.Text.Json.JsonDocument.Parse(body);
            string? finalMsg = null;
            if (doc.RootElement.TryGetProperty("message", out var msg) && msg.ValueKind == System.Text.Json.JsonValueKind.String)
                finalMsg = msg.GetString();

            if (doc.RootElement.TryGetProperty("errors", out var errors) &&
                errors.ValueKind == System.Text.Json.JsonValueKind.Array &&
                errors.GetArrayLength() > 0)
            {
                var errorList = errors.EnumerateArray()
                    .Select(e => e.GetString())
                    .Where(e => !string.IsNullOrEmpty(e));
                var errorStr = string.Join(" | ", errorList);
                if (!string.IsNullOrEmpty(errorStr))
                    finalMsg = finalMsg == null ? errorStr : $"{finalMsg} ({errorStr})";
            }

            return finalMsg;
        }
        catch
        {
            return null;
        }
    }
}

internal sealed class BackendUnauthorizedException : Exception
{
    public BackendUnauthorizedException(string message) : base(message) { }
}
