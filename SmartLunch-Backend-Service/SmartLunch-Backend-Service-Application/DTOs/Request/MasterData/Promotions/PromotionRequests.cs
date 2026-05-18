namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Promotions;

public class GetPromotionsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public string? ScopeType { get; set; }
}

public class PromotionTargetRequest
{
    public string TargetType { get; set; } = string.Empty;
    public int? TargetId { get; set; }
    public string? TargetKey { get; set; }
}

public class UpsertPromotionRequest
{
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ScopeType { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public int Priority { get; set; }
    public string SelectionMode { get; set; } = "best_discount";
    public string Channel { get; set; } = "all";
    public int? MinOrderQuantity { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public TimeOnly? BookingTimeStart { get; set; }
    public TimeOnly? BookingTimeEnd { get; set; }
    public int? MaxTotalUses { get; set; }
    public int? MaxUsesPerUser { get; set; }
    public bool IsActive { get; set; } = true;
    public List<PromotionTargetRequest> Targets { get; set; } = new();
}

public class PreviewPromotionRequest
{
    public string Channel { get; set; } = "b2c";
    public int? OrganizationId { get; set; }
    public int? ContractId { get; set; }
    public string? ContractType { get; set; }
    public string? PromotionCode { get; set; }
    public decimal Subtotal { get; set; }
    public int TotalQuantity { get; set; }
    public List<PreviewPromotionLineRequest> Lines { get; set; } = new();
}

public class PreviewPromotionLineRequest
{
    public int DishId { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
