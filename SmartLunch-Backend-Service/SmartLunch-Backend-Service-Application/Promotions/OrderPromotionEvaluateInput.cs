using SmartLunch.Backend.Service.Application.Constants;

namespace SmartLunch.Backend.Service.Application.Promotions;

public sealed class OrderPromotionEvaluateInput
{
    public string Channel { get; set; } = PromotionConstants.ChannelB2C;

    public int? UserId { get; set; }
    public int? OrganizationId { get; set; }
    public int? ContractId { get; set; }
    public string? ContractType { get; set; }

    /// <summary>Mã KM khách nhập (tùy chọn).</summary>
    public string? PromotionCode { get; set; }

    public decimal Subtotal { get; set; }
    public int TotalQuantity { get; set; }
    public List<OrderPromotionLineInput> Lines { get; set; } = new();
    public DateTime? OrderPlacedAt { get; set; }
}

public sealed class OrderPromotionLineInput
{
    public int DishId { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

public sealed class OrderPromotionEvaluateResult
{
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAfter { get; set; }
    public bool Applied { get; set; }
    public int? PromotionId { get; set; }
    public string? PromotionCode { get; set; }
    public string? PromotionName { get; set; }
    public string? ScopeType { get; set; }
    public string? DiscountType { get; set; }
    public decimal? DiscountValue { get; set; }
    public string? Message { get; set; }
}
