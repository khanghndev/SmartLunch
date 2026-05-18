using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Promotions;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Helpers;

public static class PromotionDtoMapping
{
    public static PromotionDto ToDto(Promotion entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Name = entity.Name,
        Description = entity.Description,
        ScopeType = entity.ScopeType,
        DiscountType = entity.DiscountType,
        DiscountValue = entity.DiscountValue,
        Priority = entity.Priority,
        SelectionMode = entity.SelectionMode,
        Channel = entity.Channel,
        MinOrderQuantity = entity.MinOrderQuantity,
        MinOrderAmount = entity.MinOrderAmount,
        ValidFrom = entity.ValidFrom,
        ValidTo = entity.ValidTo,
        BookingTimeStart = entity.BookingTimeStart,
        BookingTimeEnd = entity.BookingTimeEnd,
        MaxTotalUses = entity.MaxTotalUses,
        MaxUsesPerUser = entity.MaxUsesPerUser,
        IsActive = entity.IsActive,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        Targets = entity.Targets.Select(t => new PromotionTargetDto
        {
            Id = t.Id,
            TargetType = t.TargetType,
            TargetId = t.TargetId,
            TargetKey = t.TargetKey,
        }).ToList(),
    };
}
