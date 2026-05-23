namespace SmartLunch.Backend.Service.Application.Integration.PayOS;

public interface IPayOSClient
{
    /// <summary>
    /// Tạo phiên thanh toán và trả về checkout URL / QR code.
    /// </summary>
    Task<PayOSCreatePaymentResult> CreatePaymentRequestAsync(
        PayOSCreatePaymentInput input,
        CancellationToken cancellationToken = default);

    /// <summary>Tra cứu phiên thanh toán theo orderCode (thường = Payment.Id).</summary>
    Task<PayOSPaymentRequestInfoResult> GetPaymentRequestAsync(
        int orderCode,
        CancellationToken cancellationToken = default);

    /// <summary>Hủy phiên thanh toán để có thể tạo lại cùng orderCode (nếu PayOS cho phép).</summary>
    Task<PayOSPaymentRequestInfoResult> CancelPaymentRequestAsync(
        int orderCode,
        string? cancellationReason = null,
        CancellationToken cancellationToken = default);
}
