using SmartLunch.Backend.Service.Application.DTOs;
using SmartLunch.Backend.Service.Application.Promotions;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;

public class PromotionTargetDto
{
    public int Id { get; set; }
    public string TargetType { get; set; } = string.Empty;
    public int? TargetId { get; set; }
    public string? TargetKey { get; set; }
}

public class PromotionDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ScopeType { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public int Priority { get; set; }
    public string SelectionMode { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public int? MinOrderQuantity { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public TimeOnly? BookingTimeStart { get; set; }
    public TimeOnly? BookingTimeEnd { get; set; }
    public int? MaxTotalUses { get; set; }
    public int? MaxUsesPerUser { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<PromotionTargetDto> Targets { get; set; } = new();
}

public class GetPromotionResponse
{
    public PromotionDto Promotion { get; set; } = new();
}

public class GetPromotionsResponse : PaginationResponse<PromotionDto>
{
}

public class PreviewPromotionResponse
{
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAfter { get; set; }
    public bool Applied { get; set; }
    public int? PromotionId { get; set; }
    public string? PromotionCode { get; set; }
    public string? PromotionName { get; set; }
    public string? Message { get; set; }

    public static PreviewPromotionResponse From(OrderPromotionEvaluateResult r) => new()
    {
        Subtotal = r.Subtotal,
        DiscountAmount = r.DiscountAmount,
        TotalAfter = r.TotalAfter,
        Applied = r.Applied,
        PromotionId = r.PromotionId,
        PromotionCode = r.PromotionCode,
        PromotionName = r.PromotionName,
        Message = r.Message,
    };
}
