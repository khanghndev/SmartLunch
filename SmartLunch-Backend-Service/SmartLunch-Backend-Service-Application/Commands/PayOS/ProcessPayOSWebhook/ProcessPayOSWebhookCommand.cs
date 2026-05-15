using MediatR;
using System.Text.Json;

namespace SmartLunch.Backend.Service.Application.Commands.PayOS.ProcessPayOSWebhook;

public sealed record ProcessPayOSWebhookCommand(JsonElement Body) : IRequest<ProcessPayOSWebhookResult>;

public enum PayOSWebhookProcessStatus
{
    Ok,
    AlreadyProcessed,
    /// <summary>Chữ ký / payload hợp lệ nhưng không ghi DB (ví dụ dữ liệu mẫu khi PayOS confirm URL, hoặc không khớp đơn). Vẫn trả HTTP 2xx để PayOS coi webhook hoạt động.</summary>
    AcknowledgedNoUpdate,
    InvalidPayload,
    InvalidSignature,
    PaymentNotFound,
    AmountMismatch,
    IgnoredNonSuccess,
}

public sealed class ProcessPayOSWebhookResult
{
    public PayOSWebhookProcessStatus Status { get; init; }
    public string Message { get; init; } = "";
}
