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

    /// <summary>Chọn KM theo Id (ưu tiên hơn mã khi cả hai có).</summary>
    public int? PromotionId { get; set; }

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

public sealed class OrderPromotionEligibleItem
{
    public int PromotionId { get; set; }
    public string? PromotionCode { get; set; }
    public string PromotionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAfter { get; set; }
    public bool IsRecommended { get; set; }
}

public sealed class ListEligiblePromotionsResult
{
    public decimal Subtotal { get; set; }
    public List<OrderPromotionEligibleItem> Items { get; set; } = new();
    public int? RecommendedPromotionId { get; set; }
    public string? Message { get; set; }
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
