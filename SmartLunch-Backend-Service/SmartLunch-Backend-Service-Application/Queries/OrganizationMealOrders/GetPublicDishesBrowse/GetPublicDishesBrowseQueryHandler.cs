using MediatR;
using Microsoft.Extensions.Configuration;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealOrders.GetPublicDishesBrowse;

public sealed class GetPublicDishesBrowseQueryHandler
    : IRequestHandler<GetPublicDishesBrowseQuery, GetPublicDishesBrowseResponse>
{
    private readonly IDishRepository _dishRepository;
    private readonly IStorageService _storage;
    private readonly IConfiguration _configuration;

    public GetPublicDishesBrowseQueryHandler(
        IDishRepository dishRepository,
        IStorageService storage,
        IConfiguration configuration)
    {
        _dishRepository = dishRepository;
        _storage = storage;
        _configuration = configuration;
    }

    public async Task<GetPublicDishesBrowseResponse> Handle(
        GetPublicDishesBrowseQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 60);

        List<Dish> dishes;
        int totalCount;

        if (request.CategoryId is > 0)
        {
            (dishes, totalCount) = await _dishRepository.GetByDishCategoryIdAsync(
                request.CategoryId.Value, page, pageSize, cancellationToken);
        }
        else
        {
            (dishes, totalCount) = await _dishRepository.GetDishesAsync(
                page, pageSize, request.Search, isActive: true, category: null);
        }

        var items = new List<PublicDishBrowseItemDto>(dishes.Count);
        foreach (var dish in dishes)
        {
            var dto = DishDtoMapping.ToDto(dish);
            dto = await WithSignedImageAsync(dto, cancellationToken);
            items.Add(new PublicDishBrowseItemDto
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                PrimarySlotKey = dto.PrimarySlotKey,
                CategoryLabel = DishDtoMapping.FormatMealSlotNamesDisplay(dish),
                DietaryLabel = dto.DietaryLabel,
            });
        }

        return new GetPublicDishesBrowseResponse
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items,
        };
    }

    private async Task<DishDto> WithSignedImageAsync(DishDto dto, CancellationToken cancellationToken)
    {
        var raw = dto.ImageUrl;
        if (string.IsNullOrWhiteSpace(raw))
            return dto;

        if (raw.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return dto;

        var expiresMinutes = int.TryParse(_configuration["Media:DownloadUrlExpireMinutes"], out var m) ? m : 15;
        var expiresIn = TimeSpan.FromMinutes(Math.Clamp(expiresMinutes, 1, 60));
        var signed = await _storage.CreateSignedUrlAsync(
            raw.Trim(), HttpMethod.Get, contentType: null, expiresIn: expiresIn);

        var resolvedUrl = string.IsNullOrWhiteSpace(signed.Url) ? raw : signed.Url;
        return new DishDto
        {
            Id = dto.Id,
            Code = dto.Code,
            Name = dto.Name,
            NameEnglish = dto.NameEnglish,
            Description = dto.Description,
            PrimarySlotKey = dto.PrimarySlotKey,
            DishSlotCategoryCodes = [.. dto.DishSlotCategoryCodes],
            CookingMethod = dto.CookingMethod,
            Price = dto.Price,
            DietaryLabel = dto.DietaryLabel,
            ImageUrl = resolvedUrl,
            Images = dto.Images.ToList(),
            Calories = dto.Calories,
            Protein = dto.Protein,
            Fat = dto.Fat,
            Carbs = dto.Carbs,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
        };
    }
}
