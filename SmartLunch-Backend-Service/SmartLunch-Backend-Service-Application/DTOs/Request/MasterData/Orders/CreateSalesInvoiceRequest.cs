namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;

/// <summary>
/// Tạo đơn / hóa đơn bởi nhân viên bán (POS).
/// </summary>
public class CreateSalesInvoiceRequest
{
    public int OrganizationId { get; set; }

    /// <summary>Khách hàng (đặt cơm) — tùy chọn.</summary>
    public int? UserId { get; set; }

    /// <summary>Ngày giao / ăn. Nếu không gửi (default), dùng ngày hiện tại (UTC).</summary>
    public DateOnly ScheduledDate { get; set; }

    public List<CreateSalesInvoiceLineRequest> Lines { get; set; } = new();
}

public class CreateSalesInvoiceLineRequest
{
    public int DishId { get; set; }
    public int Quantity { get; set; }
}
