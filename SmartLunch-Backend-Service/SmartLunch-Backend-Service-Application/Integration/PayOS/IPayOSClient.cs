namespace SmartLunch.Backend.Service.Application.Integration.PayOS;

public interface IPayOSClient
{
    /// <summary>
    /// Tạo phiên thanh toán và trả về checkout URL / QR code.
    /// </summary>
    Task<PayOSCreatePaymentResult> CreatePaymentRequestAsync(
        PayOSCreatePaymentInput input,
        CancellationToken cancellationToken = default);
}
