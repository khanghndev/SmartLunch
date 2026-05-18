namespace SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

public sealed class OrganizationMealDraftLineSummaryDto
{
    public DateOnly ServiceDate { get; set; }

    /// <summary>main | side | soup</summary>
    public string Slot { get; set; } = string.Empty;

    public int DishId { get; set; }
    public string DishName { get; set; } = string.Empty;
    public int Quantity { get; set; }

    /// <summary>Đơn giá ghi nhận: bằng giá thỏa thuận / suất nếu main; 0 nếu side/soup.</summary>
    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}

public sealed class PrepareOrganizationMealContractResponse
{
    /// <summary>Mã nháy — gửi lại khi POST checkout.</summary>
    public string DraftId { get; set; } = string.Empty;

    public DateOnly AllowedFirstServiceDate { get; set; }
    public DateOnly AllowedLastServiceDate { get; set; }

    public decimal PricePerPortion { get; set; }
    public int TotalMainQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public int? AppliedPromotionId { get; set; }
    public string? AppliedPromotionName { get; set; }
    public string? PromotionCode { get; set; }
    public List<OrganizationMealDraftLineSummaryDto> Lines { get; set; } = new();

    /// <summary>Gợi ý hiển thị: dữ liệu chỉ lưu cache, chưa ghi CSDL.</summary>
    public string PersistenceNotice { get; set; } =
        "Dữ liệu đang ở bước xem/ký hợp đồng — chưa lưu database. Gọi POST checkout để xác nhận đơn và thanh toán.";
}
