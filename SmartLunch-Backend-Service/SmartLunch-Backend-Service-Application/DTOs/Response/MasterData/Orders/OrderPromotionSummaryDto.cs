namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

public class OrderPromotionSummaryDto
{
    public int PromotionId { get; set; }
    public string? PromotionCode { get; set; }
    public string PromotionName { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
}
