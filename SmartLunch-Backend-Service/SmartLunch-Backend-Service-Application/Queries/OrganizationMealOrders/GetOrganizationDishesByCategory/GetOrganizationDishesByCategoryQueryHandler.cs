using MediatR;
using Microsoft.Extensions.Configuration;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealOrders.GetOrganizationDishesByCategory;

public sealed class GetOrganizationDishesByCategoryQueryHandler
    : IRequestHandler<GetOrganizationDishesByCategoryQuery, GetOrganizationDishesByCategoryResponse>
{
    private readonly IDishRepository _dishRepository;
    private readonly IStorageService _storage;
    private readonly IConfiguration _configuration;

    public GetOrganizationDishesByCategoryQueryHandler(
        IDishRepository dishRepository,
        IStorageService storage,
        IConfiguration configuration)
    {
        _dishRepository = dishRepository;
        _storage = storage;
        _configuration = configuration;
    }

    public async Task<GetOrganizationDishesByCategoryResponse> Handle(
        GetOrganizationDishesByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        if (request.DishCategoryId <= 0)
            throw new ArgumentException("categoryId (dish category) is required.");

        var (dishes, total) = await _dishRepository.GetByDishCategoryIdAsync(
            request.DishCategoryId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var items = new List<OrganizationDishListItemDto>(dishes.Count);
        foreach (var dish in dishes)
        {
            var dto = DishDtoMapping.ToDto(dish);
            dto = await WithSignedImageAsync(dto, cancellationToken);
            items.Add(new OrganizationDishListItemDto
            {
                Id = dto.Id,
                Name = dto.Name,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                SlotKeys = dish.DishDishCategories
                    .Where(x => x.DishCategory != null)
                    .Select(x => x.DishCategory!.SlotKey)
                    .Distinct()
                    .ToList(),
            });
        }

        return new GetOrganizationDishesByCategoryResponse
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total,
            Dishes = items,
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
