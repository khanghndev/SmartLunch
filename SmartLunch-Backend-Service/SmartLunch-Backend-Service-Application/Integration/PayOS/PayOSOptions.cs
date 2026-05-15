namespace SmartLunch.Backend.Service.Application.Integration.PayOS;

/// <summary>
/// Cấu hình kênh thanh toán PayOS (merchant API). Giá trị thật nên đặt qua User Secrets / biến môi trường / Key Vault — không commit bí mật.
/// </summary>
public sealed class PayOSOptions
{
    public const string SectionKey = "PayOS";

    /// <summary>
    /// Bật để cho phép gọi API PayOS. False: <see cref="IPayOSClient.CreatePaymentRequestAsync"/> trả lỗi rõ ràng.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Ví dụ production: https://api-merchant.payos.vn
    /// </summary>
    public string BaseUrl { get; set; } = "https://api-merchant.payos.vn";

    public string ClientId { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string ChecksumKey { get; set; } = "";

    /// <summary>
    /// Tùy chọn; chỉ gửi header khi không rỗng.
    /// </summary>
    public string PartnerCode { get; set; } = "";

    /// <summary>
    /// URL mặc định sau khi thanh toán thành công (frontend). Có thể ghi đè mỗi lần tạo link.
    /// </summary>
    public string DefaultReturnUrl { get; set; } = "";

    /// <summary>
    /// URL mặc định khi hủy. Có thể ghi đè mỗi lần tạo link.
    /// </summary>
    public string DefaultCancelUrl { get; set; } = "";

    /// <summary>
    /// Chỉ dùng môi trường dev: bỏ qua verify chữ ký webhook. Production phải false.
    /// </summary>
    public bool WebhookSkipSignatureVerification { get; set; }
}
