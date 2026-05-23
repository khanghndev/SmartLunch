using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartLunch.Backend.Service.Application.Integration.PayOS;

namespace SmartLunch.Backend.Service.Infrastructure.ExternalServices;

public sealed class PayOSClient : IPayOSClient
{
    private static readonly JsonSerializerOptions JsonWrite = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonSerializerOptions JsonRead = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _http;
    private readonly PayOSOptions _options;
    private readonly ILogger<PayOSClient> _logger;

    public PayOSClient(
        HttpClient httpClient,
        IOptions<PayOSOptions> options,
        ILogger<PayOSClient> logger)
    {
        _http = httpClient;
        _options = options.Value;
        _logger = logger;

        var baseUrl = (_options.BaseUrl ?? "").TrimEnd('/');
        if (!string.IsNullOrEmpty(baseUrl))
            _http.BaseAddress = new Uri(baseUrl + "/");
    }

    public async Task<PayOSCreatePaymentResult> CreatePaymentRequestAsync(
        PayOSCreatePaymentInput input,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return Fail(0, null, null, "PayOS is disabled (PayOS:Enabled = false).");
        }

        if (string.IsNullOrWhiteSpace(_options.ClientId) ||
            string.IsNullOrWhiteSpace(_options.ApiKey) ||
            string.IsNullOrWhiteSpace(_options.ChecksumKey))
        {
            return Fail(0, null, null, "PayOS ClientId, ApiKey, and ChecksumKey must be configured.");
        }

        var returnUrl = input.ReturnUrl ?? _options.DefaultReturnUrl;
        var cancelUrl = input.CancelUrl ?? _options.DefaultCancelUrl;
        if (string.IsNullOrWhiteSpace(returnUrl) || string.IsNullOrWhiteSpace(cancelUrl))
        {
            return Fail(0, null, null, "returnUrl and cancelUrl are required (set on request or PayOS:DefaultReturnUrl / DefaultCancelUrl).");
        }

        if (string.IsNullOrWhiteSpace(input.Description))
            return Fail(0, null, null, "Description is required.");

        if (input.Amount <= 0)
            return Fail(0, null, null, "Amount must be a positive integer (VND).");

        var signature = CreatePaymentRequestSignature(
            input.Amount,
            cancelUrl,
            input.Description,
            input.OrderCode,
            returnUrl,
            _options.ChecksumKey);

        var body = new PayOSPaymentRequestDto
        {
            OrderCode = input.OrderCode,
            Amount = input.Amount,
            Description = input.Description,
            ReturnUrl = returnUrl,
            CancelUrl = cancelUrl,
            Signature = signature,
            BuyerName = input.BuyerName,
            BuyerEmail = input.BuyerEmail,
            BuyerPhone = input.BuyerPhone,
            ExpiredAt = input.ExpiredAt,
            Items = input.Items?.Select(i => new PayOSPaymentItemDto
            {
                Name = i.Name,
                Quantity = i.Quantity,
                Price = i.Price,
                Unit = i.Unit
            }).ToList()
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, "v2/payment-requests");
        req.Headers.TryAddWithoutValidation("x-client-id", _options.ClientId);
        req.Headers.TryAddWithoutValidation("x-api-key", _options.ApiKey);
        if (!string.IsNullOrWhiteSpace(_options.PartnerCode))
            req.Headers.TryAddWithoutValidation("x-partner-code", _options.PartnerCode);

        req.Content = JsonContent.Create(body, options: JsonWrite);

        HttpResponseMessage res;
        try
        {
            res = await _http.SendAsync(req, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PayOS HTTP request failed");
            return Fail(0, null, null, $"PayOS request failed: {ex.Message}");
        }

        var status = (int)res.StatusCode;
        string raw;
        try
        {
            raw = await res.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PayOS: failed to read response body");
            return Fail(status, null, null, "Failed to read PayOS response body.");
        }

        PayOSApiEnvelope? envelope;
        try
        {
            envelope = JsonSerializer.Deserialize<PayOSApiEnvelope>(raw, JsonRead);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "PayOS: invalid JSON. Raw: {Raw}", Truncate(raw));
            return Fail(status, null, null, "Invalid JSON from PayOS.", raw);
        }

        if (envelope is null)
            return Fail(status, null, null, "Empty or unreadable PayOS response.", raw);

        var okCode = string.Equals(envelope.Code, "00", StringComparison.Ordinal);
        var data = envelope.Data;

        if (!res.IsSuccessStatusCode || !okCode || data is null)
        {
            var msg = envelope.Desc ?? res.ReasonPhrase ?? "PayOS error";
            return new PayOSCreatePaymentResult
            {
                Success = false,
                StatusCode = status,
                Code = envelope.Code,
                Desc = envelope.Desc,
                Message = msg
            };
        }

        return new PayOSCreatePaymentResult
        {
            Success = true,
            StatusCode = status,
            Code = envelope.Code,
            Desc = envelope.Desc,
            Status = data.Status,
            CheckoutUrl = data.CheckoutUrl,
            PaymentLinkId = data.PaymentLinkId,
            QrCode = data.QrCode,
            Amount = data.Amount,
            Message = envelope.Desc ?? "success"
        };
    }

    public Task<PayOSPaymentRequestInfoResult> GetPaymentRequestAsync(
        int orderCode,
        CancellationToken cancellationToken = default)
        => SendPaymentRequestInfoAsync(HttpMethod.Get, $"v2/payment-requests/{orderCode}", null, cancellationToken);

    public Task<PayOSPaymentRequestInfoResult> CancelPaymentRequestAsync(
        int orderCode,
        string? cancellationReason = null,
        CancellationToken cancellationToken = default)
    {
        object? body = string.IsNullOrWhiteSpace(cancellationReason)
            ? null
            : new { cancellationReason = cancellationReason.Trim() };
        return SendPaymentRequestInfoAsync(
            HttpMethod.Post,
            $"v2/payment-requests/{orderCode}/cancel",
            body,
            cancellationToken);
    }

    private async Task<PayOSPaymentRequestInfoResult> SendPaymentRequestInfoAsync(
        HttpMethod method,
        string relativePath,
        object? jsonBody,
        CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
            return InfoFail("PayOS is disabled (PayOS:Enabled = false).");

        if (string.IsNullOrWhiteSpace(_options.ClientId) ||
            string.IsNullOrWhiteSpace(_options.ApiKey))
            return InfoFail("PayOS ClientId and ApiKey must be configured.");

        using var req = new HttpRequestMessage(method, relativePath);
        req.Headers.TryAddWithoutValidation("x-client-id", _options.ClientId);
        req.Headers.TryAddWithoutValidation("x-api-key", _options.ApiKey);
        if (!string.IsNullOrWhiteSpace(_options.PartnerCode))
            req.Headers.TryAddWithoutValidation("x-partner-code", _options.PartnerCode);

        if (jsonBody != null)
            req.Content = JsonContent.Create(jsonBody, options: JsonWrite);

        HttpResponseMessage res;
        try
        {
            res = await _http.SendAsync(req, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PayOS {Method} {Path} failed", method, relativePath);
            return InfoFail($"PayOS request failed: {ex.Message}");
        }

        var status = (int)res.StatusCode;
        string raw;
        try
        {
            raw = await res.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return InfoFail($"Failed to read PayOS response: {ex.Message}");
        }

        PayOSApiEnvelope? envelope;
        try
        {
            envelope = JsonSerializer.Deserialize<PayOSApiEnvelope>(raw, JsonRead);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "PayOS: invalid JSON. Raw: {Raw}", Truncate(raw));
            return InfoFail("Invalid JSON from PayOS.");
        }

        if (envelope is null)
            return InfoFail("Empty PayOS response.");

        var okCode = string.Equals(envelope.Code, "00", StringComparison.Ordinal);
        var data = envelope.Data;
        if (!res.IsSuccessStatusCode || !okCode || data is null)
        {
            var msg = envelope.Desc ?? res.ReasonPhrase ?? "PayOS error";
            return new PayOSPaymentRequestInfoResult
            {
                Success = false,
                Code = envelope.Code,
                Desc = envelope.Desc,
                Message = msg,
            };
        }

        return new PayOSPaymentRequestInfoResult
        {
            Success = true,
            Code = envelope.Code,
            Desc = envelope.Desc,
            Status = data.Status,
            CheckoutUrl = data.CheckoutUrl,
            QrCode = data.QrCode,
            Amount = data.Amount,
            Message = envelope.Desc ?? "success",
        };
    }

    private static PayOSPaymentRequestInfoResult InfoFail(string message) =>
        new() { Success = false, Message = message };

    /// <summary>
    /// Cùng định dạng với @payos/node <c>createSignatureOfPaymentRequest</c>: chuỗi cố định, không URL-encode.
    /// </summary>
    internal static string CreatePaymentRequestSignature(
        int amount,
        string cancelUrl,
        string description,
        int orderCode,
        string returnUrl,
        string checksumKey)
    {
        var dataStr =
            $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";
        var keyBytes = Encoding.UTF8.GetBytes(checksumKey);
        var dataBytes = Encoding.UTF8.GetBytes(dataStr);
        var hash = HMACSHA256.HashData(keyBytes, dataBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static PayOSCreatePaymentResult Fail(
        int statusCode,
        string? code,
        string? desc,
        string message,
        string? rawDebug = null)
    {
        var fullMessage = rawDebug is null ? message : $"{message} {Truncate(rawDebug)}";
        return new PayOSCreatePaymentResult
        {
            Success = false,
            StatusCode = statusCode,
            Code = code,
            Desc = desc,
            Message = fullMessage
        };
    }

    private static string Truncate(string s, int max = 500) =>
        s.Length <= max ? s : s[..max] + "…";

    private sealed class PayOSPaymentRequestDto
    {
        public int OrderCode { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; } = "";
        public string ReturnUrl { get; set; } = "";
        public string CancelUrl { get; set; } = "";
        public string Signature { get; set; } = "";
        public string? BuyerName { get; set; }
        public string? BuyerEmail { get; set; }
        public string? BuyerPhone { get; set; }
        public long? ExpiredAt { get; set; }
        public List<PayOSPaymentItemDto>? Items { get; set; }
    }

    private sealed class PayOSPaymentItemDto
    {
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string? Unit { get; set; }
    }

    private sealed class PayOSApiEnvelope
    {
        public string? Code { get; set; }
        public string? Desc { get; set; }
        public PayOSApiData? Data { get; set; }
    }

    private sealed class PayOSApiData
    {
        public string? Status { get; set; }
        public string? CheckoutUrl { get; set; }
        public string? PaymentLinkId { get; set; }
        public string? QrCode { get; set; }
        public int? Amount { get; set; }
    }
}
