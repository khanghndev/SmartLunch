using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SmartLunch.Backend.Service.Application.Integration.PayOS;

/// <summary>
/// Xác thực chữ ký webhook theo cùng thuật toán thư viện chính thức PayOS (<c>CreateSignatureFromObj</c> / <c>SortObjByKey</c> + HMAC-SHA256 hex).
/// </summary>
public static class PayOSWebhookSignature
{
    private static readonly JsonSerializerOptions CompactJson = new()
    {
        WriteIndented = false,
    };

    public static bool IsValid(JsonElement dataObject, string signature, string checksumKey)
    {
        if (dataObject.ValueKind != JsonValueKind.Object)
            return false;
        if (string.IsNullOrWhiteSpace(signature) || string.IsNullOrWhiteSpace(checksumKey))
            return false;

        var sorted = SortObjectByKey(dataObject);
        var keyBytes = Encoding.UTF8.GetBytes(checksumKey);
        var dataBytes = Encoding.UTF8.GetBytes(sorted);
        var hash = HMACSHA256.HashData(keyBytes, dataBytes);
        var computed = Convert.ToHexString(hash).ToLowerInvariant();
        var expected = signature.Trim();
        if (computed.Length != expected.Length)
            return false;

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computed),
            Encoding.UTF8.GetBytes(expected));
    }

    /// <summary>Chuỗi sort key + HMAC payload (giống payos-lib-golang SortObjByKey).</summary>
    public static string SortObjectByKey(JsonElement obj)
    {
        var keys = new List<string>();
        foreach (var p in obj.EnumerateObject())
            keys.Add(p.Name);
        keys.Sort(StringComparer.Ordinal);

        var parts = new List<string>(keys.Count);
        foreach (var key in keys)
        {
            var value = obj.GetProperty(key);
            var stringValue = ConvertValueToSignatureString(value);
            parts.Add($"{key}={stringValue}");
        }

        return string.Join("&", parts);
    }

    private static string ConvertValueToSignatureString(JsonElement el)
    {
        return el.ValueKind switch
        {
            JsonValueKind.String => el.GetString() ?? string.Empty,
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => string.Empty,
            JsonValueKind.Number => FormatNumber(el),
            JsonValueKind.Object or JsonValueKind.Array => JsonSerializer.Serialize(el, CompactJson),
            _ => el.GetRawText(),
        };
    }

    private static string FormatNumber(JsonElement el)
    {
        if (el.TryGetInt64(out var l))
            return l.ToString(CultureInfo.InvariantCulture);
        if (el.TryGetDecimal(out var d))
            return d.ToString(CultureInfo.InvariantCulture);
        return el.GetDouble().ToString("G17", CultureInfo.InvariantCulture);
    }
}
