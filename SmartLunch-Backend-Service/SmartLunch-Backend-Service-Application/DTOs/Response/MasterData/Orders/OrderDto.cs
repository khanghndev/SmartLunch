namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

public class OrderDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int? ContractId { get; set; }
    public OrderContractSummaryDto? ContractSummary { get; set; }
    public int? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal? SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public OrderPromotionSummaryDto? AppliedPromotion { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? InvoiceCode { get; set; }
    public int? CreatedBySalesUserId { get; set; }
    public string? CreatedBySalesDisplayName { get; set; }

    /// <summary>PDF phụ lục / biên bản đặt hàng đã ký (lưu object storage).</summary>
    public string? AnnexPdfUrl { get; set; }

    public DateTime? AnnexSignedAt { get; set; }

    public List<OrderItemLineDto> Items { get; set; } = new();
}
