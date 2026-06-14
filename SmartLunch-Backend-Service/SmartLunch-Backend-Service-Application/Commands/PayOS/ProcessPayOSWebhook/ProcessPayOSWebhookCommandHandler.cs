using System.Globalization;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Integration.PayOS;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.PayOS.ProcessPayOSWebhook;

public sealed class ProcessPayOSWebhookCommandHandler
    : IRequestHandler<ProcessPayOSWebhookCommand, ProcessPayOSWebhookResult>
{
    private readonly IOptions<PayOSOptions> _payOptions;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessPayOSWebhookCommandHandler> _logger;

    public ProcessPayOSWebhookCommandHandler(
        IOptions<PayOSOptions> payOptions,
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        ILogger<ProcessPayOSWebhookCommandHandler> logger)
    {
        _payOptions = payOptions;
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ProcessPayOSWebhookResult> Handle(
        ProcessPayOSWebhookCommand request,
        CancellationToken cancellationToken)
    {
        var root = request.Body;
        if (root.ValueKind != JsonValueKind.Object)
            return Fail(PayOSWebhookProcessStatus.InvalidPayload, "Body must be a JSON object.");

        if (!root.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Object)
            return Fail(PayOSWebhookProcessStatus.InvalidPayload, "Missing data object.");

        if (!TryGetStringPropertyIgnoreCase(root, "signature", out var signature) || string.IsNullOrWhiteSpace(signature))
            return Fail(PayOSWebhookProcessStatus.InvalidPayload, "Missing signature.");

        var opts = _payOptions.Value;
        if (!opts.WebhookSkipSignatureVerification)
        {
            if (string.IsNullOrWhiteSpace(opts.ChecksumKey))
                return Fail(PayOSWebhookProcessStatus.InvalidSignature, "ChecksumKey is not configured.");

            if (!PayOSWebhookSignature.IsValid(data, signature.Trim(), opts.ChecksumKey))
            {
                _logger.LogWarning("PayOS webhook: invalid signature for orderCode in data.");
                return Fail(PayOSWebhookProcessStatus.InvalidSignature, "Invalid signature.");
            }
        }
        else
            _logger.LogWarning("PayOS webhook: signature verification SKIPPED (PayOS:WebhookSkipSignatureVerification).");

        if (!IsOuterSuccess(root))
            return new ProcessPayOSWebhookResult
            {
                Status = PayOSWebhookProcessStatus.IgnoredNonSuccess,
                Message = "Outer code/success indicates non-success; ignored.",
            };

        if (!IsDataPaymentSuccess(data))
            return new ProcessPayOSWebhookResult
            {
                Status = PayOSWebhookProcessStatus.IgnoredNonSuccess,
                Message = "data.code is not success; ignored.",
            };

        if (!data.TryGetProperty("orderCode", out var orderCodeEl))
            return Fail(PayOSWebhookProcessStatus.InvalidPayload, "Missing data.orderCode.");

        if (!TryParsePayOSOrderCode(orderCodeEl, out var orderCode))
            return Fail(PayOSWebhookProcessStatus.InvalidPayload, "Invalid data.orderCode.");

        if (!data.TryGetProperty("amount", out var amountEl))
            return Fail(PayOSWebhookProcessStatus.InvalidPayload, "Missing data.amount.");

        if (!TryParsePayOSAmount(amountEl, out var webhookAmount))
            return Fail(PayOSWebhookProcessStatus.InvalidPayload, "Invalid data.amount.");

        int paymentId = (int)(orderCode % 100000);

        var payment = await _paymentRepository.GetByIdWithOrderAndPaymentsAsync(paymentId, cancellationToken);
        if (payment == null)
        {
            _logger.LogWarning(
                "PayOS webhook: no local payment for orderCode {OrderCode} (paymentId {PaymentId}) (dữ liệu mẫu khi confirm URL hoặc giao dịch ngoài hệ thống).",
                orderCode, paymentId);
            return Ack(PayOSWebhookProcessStatus.AcknowledgedNoUpdate,
                "No matching payment; webhook received (e.g. PayOS URL test payload).");
        }

        var isMatch = GeneratePayOsOrderCode(payment) == orderCode || payment.Id == orderCode;

        if (!isMatch)
        {
            _logger.LogWarning(
                "PayOS webhook: payment mismatch for orderCode {OrderCode}. Expected {ExpectedCode}, ID: {DbId}",
                orderCode, GeneratePayOsOrderCode(payment), payment.Id);
            return Ack(PayOSWebhookProcessStatus.AcknowledgedNoUpdate,
                "Payment code does not match; ignored.");
        }

        if (!string.Equals(payment.Method, "payos", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("PayOS webhook: payment {PaymentId} is not PayOS method ({Method}).", payment.Id, payment.Method);
            return Ack(PayOSWebhookProcessStatus.AcknowledgedNoUpdate, "Payment method is not payos; ignored.");
        }

        var expectedAmount = (int)Math.Round(payment.Amount, MidpointRounding.AwayFromZero);
        var webhookAmountInt = (int)Math.Round(webhookAmount, MidpointRounding.AwayFromZero);
        if (expectedAmount != webhookAmountInt)
        {
            _logger.LogWarning(
                "PayOS webhook: amount mismatch payment {PaymentId}. Expected {Expected}, got {Actual}.",
                payment.Id,
                expectedAmount,
                webhookAmountInt);
            return Ack(PayOSWebhookProcessStatus.AcknowledgedNoUpdate, "Amount does not match payment record; ignored.");
        }

        if (string.Equals(payment.Status, "paid", StringComparison.OrdinalIgnoreCase))
        {
            return new ProcessPayOSWebhookResult
            {
                Status = PayOSWebhookProcessStatus.AlreadyProcessed,
                Message = "Payment already marked paid.",
            };
        }

        var order = payment.Order;
        if (order == null)
        {
            _logger.LogWarning("PayOS webhook: payment {PaymentId} has no order navigation.", payment.Id);
            return Ack(PayOSWebhookProcessStatus.AcknowledgedNoUpdate, "Payment has no order; ignored.");
        }

        payment.Status = "paid";
        payment.PaymentDate = VietnamTime.Now;

        decimal paidSum = 0;
        foreach (var p in order.Payments)
        {
            var status = p.Id == payment.Id ? payment.Status : p.Status;
            if (string.Equals(status, "paid", StringComparison.OrdinalIgnoreCase))
                paidSum += p.Amount;
        }

        var derived = PaymentReconciliationDerivation.DerivePaymentStatus(order.TotalAmount, paidSum);
        if (string.Equals(derived, PaymentReconciliationDerivation.DerivedOverpaid, StringComparison.Ordinal))
            order.PaymentStatus = OrderPaymentStatus.Paid;
        else if (string.Equals(derived, OrderPaymentStatus.Partial, StringComparison.Ordinal) && paidSum > 0)
            order.PaymentStatus = OrderPaymentStatus.DepositPaid;
        else
            order.PaymentStatus = derived;
        order.UpdatedAt = VietnamTime.Now;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("PayOS webhook: payment {PaymentId} marked paid; order {OrderId} payment status {Status}.",
            payment.Id, order.Id, order.PaymentStatus);

        return new ProcessPayOSWebhookResult { Status = PayOSWebhookProcessStatus.Ok, Message = "Processed." };
    }

    private static ProcessPayOSWebhookResult Fail(PayOSWebhookProcessStatus status, string message) =>
        new() { Status = status, Message = message };

    private static ProcessPayOSWebhookResult Ack(PayOSWebhookProcessStatus status, string message) =>
        new() { Status = status, Message = message };

    private static bool TryGetStringPropertyIgnoreCase(JsonElement root, string name, out string value)
    {
        foreach (var p in root.EnumerateObject())
        {
            if (!string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))
                continue;
            if (p.Value.ValueKind != JsonValueKind.String)
                break;
            value = p.Value.GetString() ?? string.Empty;
            return true;
        }

        value = string.Empty;
        return false;
    }

    private static bool TryParsePayOSOrderCode(JsonElement el, out long orderCode)
    {
        orderCode = 0;
        switch (el.ValueKind)
        {
            case JsonValueKind.Number:
                if (!el.TryGetInt64(out orderCode) || orderCode <= 0)
                    return false;
                return true;
            case JsonValueKind.String:
                return long.TryParse(
                    el.GetString(),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out orderCode) && orderCode > 0;
            default:
                return false;
        }
    }

    private static long GeneratePayOsOrderCode(Payment payment)
    {
        var dt = payment.CreatedAt;
        if (dt.Kind == DateTimeKind.Unspecified)
        {
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
        }
        var unixSecs = ((DateTimeOffset)dt).ToUnixTimeSeconds();
        return unixSecs * 100000L + (payment.Id % 100000);
    }

    private static bool TryParsePayOSAmount(JsonElement el, out decimal amount)
    {
        amount = 0;
        switch (el.ValueKind)
        {
            case JsonValueKind.Number:
                if (el.TryGetDecimal(out amount))
                    return amount >= 0;
                if (el.TryGetDouble(out var d))
                {
                    amount = (decimal)d;
                    return amount >= 0;
                }

                return false;
            case JsonValueKind.String:
                return decimal.TryParse(
                    el.GetString(),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out amount) && amount >= 0;
            default:
                return false;
        }
    }

    private static bool IsOuterSuccess(JsonElement root)
    {
        if (root.TryGetProperty("code", out var code) && code.ValueKind == JsonValueKind.String)
        {
            if (!string.Equals(code.GetString(), "00", StringComparison.Ordinal))
                return false;
        }

        if (root.TryGetProperty("success", out var ok) && ok.ValueKind is JsonValueKind.True or JsonValueKind.False)
            return ok.GetBoolean();

        return true;
    }

    private static bool IsDataPaymentSuccess(JsonElement data)
    {
        if (data.TryGetProperty("code", out var code) && code.ValueKind == JsonValueKind.String)
            return string.Equals(code.GetString(), "00", StringComparison.Ordinal);
        return true;
    }
}
