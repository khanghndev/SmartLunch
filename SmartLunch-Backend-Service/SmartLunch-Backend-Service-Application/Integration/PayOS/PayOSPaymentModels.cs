using System.Text.Json.Serialization;

namespace SmartLunch.Backend.Service.Application.Integration.PayOS;

/// <summary>
/// Thông tin gửi lên PayOS khi tạo yêu cầu thanh toán (payment link).
/// </summary>
public sealed class PayOSCreatePaymentInput
{
    /// <summary>
    /// Mã đơn thanh toán trên PayOS — kiểu int theo API. Phải unique trong kênh; thường map từ hợp đồng/phiên thanh toán (lưu ý giới hạn int).
    /// </summary>
    public int OrderCode { get; set; }

    /// <summary>
    /// Số tiền VND (số nguyên, không xu).
    /// </summary>
    public int Amount { get; set; }

    public string Description { get; set; } = "";

    /// <summary>
    /// Nếu null, dùng <see cref="PayOSOptions.DefaultReturnUrl"/>.
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Nếu null, dùng <see cref="PayOSOptions.DefaultCancelUrl"/>.
    /// </summary>
    public string? CancelUrl { get; set; }

    public string? BuyerName { get; set; }
    public string? BuyerEmail { get; set; }
    public string? BuyerPhone { get; set; }

    /// <summary>
    /// Epoch seconds (unix); tùy chọn theo tài liệu PayOS.
    /// </summary>
    public long? ExpiredAt { get; set; }

    public IReadOnlyList<PayOSPaymentItemInput>? Items { get; set; }
}

public sealed class PayOSPaymentItemInput
{
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public int Price { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Unit { get; set; }
}

/// <summary>
/// Kết quả tạo link thanh toán (trích từ <c>data</c> của body phản hồi PayOS).
/// </summary>
public sealed class PayOSCreatePaymentResult
{
    public bool Success { get; init; }

    /// <summary>
    /// Theo API PayOS (<c>PENDING</c>, …) khi tạo thành công.
    /// </summary>
    public string? Status { get; init; }

    public string? CheckoutUrl { get; init; }

    /// <summary>
    /// Theo tài liệu; dùng cho tra cứu / webhook.
    /// </summary>
    public string? PaymentLinkId { get; init; }

    public string? QrCode { get; init; }
    public int? Amount { get; init; }

    /// <summary>
    /// HTTP status hoặc mã logic khi không gọi được API như mong đợi.
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// Mã lớp outer (<c>code</c>). Thường "00" khi OK.
    /// </summary>
    public string? Code { get; init; }

    public string? Desc { get; init; }

    public string Message { get; init; } = "";
}
