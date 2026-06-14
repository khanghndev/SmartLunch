using System.Net;
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
    private const int MaxRateLimitRetries = 3;

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

        HttpCallResult call;
        try
        {
            call = await SendWithRateLimitRetryAsync(
                () =>
                {
                    var req = new HttpRequestMessage(HttpMethod.Post, "v2/payment-requests");
                    ApplyAuthHeaders(req);
                    req.Content = JsonContent.Create(body, options: JsonWrite);
                    return req;
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PayOS HTTP request failed");
            return Fail(0, null, null, $"PayOS request failed: {ex.Message}");
        }

        var status = call.StatusCode;
        var raw = call.RawBody;
        if (!TryParseEnvelope(raw, status, out var envelope, out var parseError))
            return Fail(status, null, null, parseError, raw);

        var okCode = string.Equals(envelope!.Code, "00", StringComparison.Ordinal);
        var data = envelope.Data;

        if (!call.IsSuccessStatusCode || !okCode || data is null)
        {
            var msg = envelope.Desc ?? call.ReasonPhrase ?? "PayOS error";
            return new PayOSCreatePaymentResult
            {
                Success = false,
                StatusCode = status,
                Code = envelope.Code,
                Desc = envelope.Desc,
                Message = msg
            };
        }

        var paymentLinkId = data.PaymentLinkId ?? data.Id;
        return new PayOSCreatePaymentResult
        {
            Success = true,
            StatusCode = status,
            Code = envelope.Code,
            Desc = envelope.Desc,
            Status = data.Status,
            CheckoutUrl = ResolveCheckoutUrl(data),
            PaymentLinkId = paymentLinkId,
            QrCode = data.QrCode,
            Amount = data.Amount,
            Message = envelope.Desc ?? "success"
        };
    }

    public Task<PayOSPaymentRequestInfoResult> GetPaymentRequestAsync(
        long orderCode,
        CancellationToken cancellationToken = default)
        => SendPaymentRequestInfoAsync(HttpMethod.Get, $"v2/payment-requests/{orderCode}", null, cancellationToken);

    public Task<PayOSPaymentRequestInfoResult> CancelPaymentRequestAsync(
        long orderCode,
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
            return InfoFail(0, "PayOS is disabled (PayOS:Enabled = false).");

        if (string.IsNullOrWhiteSpace(_options.ClientId) ||
            string.IsNullOrWhiteSpace(_options.ApiKey))
            return InfoFail(0, "PayOS ClientId and ApiKey must be configured.");

        HttpCallResult call;
        try
        {
            call = await SendWithRateLimitRetryAsync(
                () =>
                {
                    var req = new HttpRequestMessage(method, relativePath);
                    ApplyAuthHeaders(req);
                    if (jsonBody != null)
                        req.Content = JsonContent.Create(jsonBody, options: JsonWrite);
                    return req;
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PayOS {Method} {Path} failed", method, relativePath);
            return InfoFail(0, $"PayOS request failed: {ex.Message}");
        }

        return MapPaymentRequestInfo(call);
    }

    private PayOSPaymentRequestInfoResult MapPaymentRequestInfo(HttpCallResult call)
    {
        var status = call.StatusCode;
        var raw = call.RawBody;
        if (!TryParseEnvelope(raw, status, out var envelope, out var parseError))
            return InfoFail(status, parseError);

        var okCode = string.Equals(envelope!.Code, "00", StringComparison.Ordinal);
        var data = envelope.Data;
        if (!call.IsSuccessStatusCode || !okCode || data is null)
        {
            var msg = envelope.Desc ?? call.ReasonPhrase ?? "PayOS error";
            return new PayOSPaymentRequestInfoResult
            {
                Success = false,
                StatusCode = status,
                Code = envelope.Code,
                Desc = envelope.Desc,
                Message = msg,
            };
        }

        return new PayOSPaymentRequestInfoResult
        {
            Success = true,
            StatusCode = status,
            Code = envelope.Code,
            Desc = envelope.Desc,
            Status = data.Status,
            CheckoutUrl = ResolveCheckoutUrl(data),
            QrCode = data.QrCode,
            Amount = data.Amount,
            Message = envelope.Desc ?? "success",
        };
    }

    /// <summary>
    /// POST create trả checkoutUrl; GET chỉ trả <c>id</c> (payment link id) — ghép URL theo tài liệu PayOS.
    /// </summary>
    private string? ResolveCheckoutUrl(PayOSApiData data)
    {
        if (!string.IsNullOrWhiteSpace(data.CheckoutUrl))
            return data.CheckoutUrl.Trim();

        var linkId = data.PaymentLinkId ?? data.Id;
        if (string.IsNullOrWhiteSpace(linkId))
            return null;

        var baseWeb = (_options.CheckoutWebBaseUrl ?? "").Trim().TrimEnd('/');
        if (string.IsNullOrEmpty(baseWeb))
            baseWeb = "https://pay.payos.vn/web";

        return $"{baseWeb}/{linkId.Trim()}";
    }

    private void ApplyAuthHeaders(HttpRequestMessage req)
    {
        req.Headers.TryAddWithoutValidation("x-client-id", _options.ClientId);
        req.Headers.TryAddWithoutValidation("x-api-key", _options.ApiKey);
        if (!string.IsNullOrWhiteSpace(_options.PartnerCode))
            req.Headers.TryAddWithoutValidation("x-partner-code", _options.PartnerCode);
    }

    private async Task<HttpCallResult> SendWithRateLimitRetryAsync(
        Func<HttpRequestMessage> buildRequest,
        CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= MaxRateLimitRetries; attempt++)
        {
            using var req = buildRequest();
            using var res = await _http.SendAsync(req, cancellationToken);
            var status = (int)res.StatusCode;
            var raw = await res.Content.ReadAsStringAsync(cancellationToken);

            if (!ShouldRetryRateLimit(status, raw) || attempt >= MaxRateLimitRetries)
            {
                return new HttpCallResult(
                    status,
                    raw,
                    res.IsSuccessStatusCode,
                    res.ReasonPhrase);
            }

            var delayMs = (int)Math.Pow(2, attempt - 1) * 1000;
            _logger.LogWarning(
                "PayOS rate limited (HTTP {Status}), retry {Attempt}/{Max} in {DelayMs}ms. Raw: {Raw}",
                status,
                attempt,
                MaxRateLimitRetries,
                delayMs,
                Truncate(raw));
            await Task.Delay(delayMs, cancellationToken);
        }

        throw new InvalidOperationException("PayOS retry loop exited unexpectedly.");
    }

    private static bool ShouldRetryRateLimit(int httpStatus, string raw) =>
        httpStatus == (int)HttpStatusCode.TooManyRequests || IsRateLimitMessage(raw);

    private static bool IsRateLimitMessage(string? text) =>
        !string.IsNullOrWhiteSpace(text) &&
        text.Contains("too many requests", StringComparison.OrdinalIgnoreCase);

    private bool TryParseEnvelope(
        string raw,
        int httpStatus,
        out PayOSApiEnvelope? envelope,
        out string errorMessage)
    {
        envelope = null;
        errorMessage = "";

        var trimmed = raw.Trim();
        if (trimmed.Length == 0)
        {
            errorMessage = "Empty PayOS response.";
            return false;
        }

        if (!LooksLikeJson(trimmed))
        {
            errorMessage = BuildPlainTextErrorMessage(trimmed, httpStatus);
            _logger.LogWarning("PayOS non-JSON response (HTTP {Status}): {Raw}", httpStatus, Truncate(trimmed));
            return false;
        }

        try
        {
            envelope = JsonSerializer.Deserialize<PayOSApiEnvelope>(trimmed, JsonRead);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "PayOS: invalid JSON. Raw: {Raw}", Truncate(trimmed));
            errorMessage = "Invalid JSON from PayOS.";
            return false;
        }

        if (envelope is null)
        {
            errorMessage = "Empty or unreadable PayOS response.";
            return false;
        }

        return true;
    }

    private static bool LooksLikeJson(string raw)
    {
        var span = raw.AsSpan().TrimStart();
        return span.Length > 0 && (span[0] == '{' || span[0] == '[');
    }

    private static string BuildPlainTextErrorMessage(string raw, int httpStatus)
    {
        if (IsRateLimitMessage(raw) || httpStatus == (int)HttpStatusCode.TooManyRequests)
        {
            return "PayOS tạm thời quá tải (quá nhiều yêu cầu). Vui lòng thử lại sau vài giây.";
        }

        return raw;
    }

    private static PayOSPaymentRequestInfoResult InfoFail(int statusCode, string message) =>
        new() { Success = false, StatusCode = statusCode, Message = message };

    private readonly record struct HttpCallResult(
        int StatusCode,
        string RawBody,
        bool IsSuccessStatusCode,
        string? ReasonPhrase);

    /// <summary>
    /// Cùng định dạng với @payos/node <c>createSignatureOfPaymentRequest</c>: chuỗi cố định, không URL-encode.
    /// </summary>
    internal static string CreatePaymentRequestSignature(
        int amount,
        string cancelUrl,
        string description,
        long orderCode,
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
        public long OrderCode { get; set; }
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
        /// <summary>Mã link thanh toán PayOS (GET); dùng ghép checkout URL khi không có checkoutUrl.</summary>
        public string? Id { get; set; }

        public string? Status { get; set; }
        public string? CheckoutUrl { get; set; }
        public string? PaymentLinkId { get; set; }
        public string? QrCode { get; set; }
        public int? Amount { get; set; }
    }
}
