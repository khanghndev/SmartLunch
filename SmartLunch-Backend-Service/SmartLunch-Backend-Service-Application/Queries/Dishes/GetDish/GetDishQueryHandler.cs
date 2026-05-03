using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Common.Caching;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace SmartLunch.Backend.Service.Application.Queries.Dishes.GetDish;

public class GetDishQueryHandler : IRequestHandler<GetDishQuery, GetDishResponse>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
    private readonly ICacheService _cacheService;
    private readonly IDishRepository _dishRepository;
    private readonly ILogger<GetDishQueryHandler> _logger;
    private readonly IStorageService _storage;
    private readonly IConfiguration _configuration;

    public GetDishQueryHandler(
        ICacheService cacheService,
        IDishRepository dishRepository,
        ILogger<GetDishQueryHandler> logger,
        IStorageService storage,
        IConfiguration configuration)
    {
        _cacheService = cacheService;
        _dishRepository = dishRepository;
        _logger = logger;
        _storage = storage;
        _configuration = configuration;
    }

    public async Task<GetDishResponse> Handle(GetDishQuery request, CancellationToken cancellationToken)
    {
        if (request.IncludeIngredientQuotas)
        {
            var dish = await _dishRepository.GetByIdWithIngredientsAsync(request.DishId);
            if (dish == null)
            {
                _logger.LogWarning("Dish not found with ID: {DishId}", request.DishId);
                return new GetDishResponse { Dish = new DishDto() };
            }

            var quotas = dish.DishIngredients
                .OrderBy(di => di.Ingredient?.Name)
                .Select(DishDtoMapping.ToQuotaDto)
                .ToList();

            var dto = DishDtoMapping.ToDto(dish);
            dto = await WithDishImagesAsync(dto, dish.DishImages);
            dto = await WithSignedImageAsync(dto);
            return new GetDishResponse
            {
                Dish = dto,
                IngredientQuotas = quotas
            };
        }

        var cacheKey = MasterDataCacheKeys.Dish(request.DishId);

        var cached = await _cacheService.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var dish = await _dishRepository.GetByIdAsync(request.DishId);

                if (dish == null)
                {
                    _logger.LogWarning("Dish not found with ID: {DishId}", request.DishId);
                    return new GetDishResponse { Dish = new DishDto() };
                }

                return new GetDishResponse
                {
                    Dish = DishDtoMapping.ToDto(dish)
                };
            },
            CacheDuration,
            cancellationToken);

        // Never cache signed URLs (they expire). Cache stores objectName in Dish.ImageUrl.
        // If not including quotas, still include images when possible via repo GetByIdWithIngredientsAsync is not called.
        // This path returns cover only (Dish.ImageUrl).
        return new GetDishResponse
        {
            Dish = await WithSignedImageAsync(cached.Dish),
            IngredientQuotas = cached.IngredientQuotas
        };
    }

    private Task<DishDto> WithDishImagesAsync(DishDto dto, ICollection<DishImage> dishImages)
    {
        if (dishImages == null || dishImages.Count == 0) return Task.FromResult(dto);

        // choose cover objectName for ImageUrl (for list/detail hero)
        var cover = dishImages
            .OrderByDescending(i => string.Equals(i.Role, "cover", StringComparison.OrdinalIgnoreCase))
            .ThenBy(i => i.SortOrder)
            .FirstOrDefault();

        if (cover?.MediaFile?.ObjectName != null)
        {
            dto.ImageUrl = cover.MediaFile.ObjectName;
        }

        var list = dishImages
            .OrderByDescending(i => string.Equals(i.Role, "cover", StringComparison.OrdinalIgnoreCase))
            .ThenBy(i => i.SortOrder)
            .Select(i => new DishImageDto
            {
                Id = i.Id,
                MediaFileId = i.MediaFileId,
                Role = i.Role,
                SortOrder = i.SortOrder,
                Url = ResolveMediaUrl(i.MediaFile)
            })
            .ToList();

        dto.Images = list;
        return Task.FromResult(dto);
    }

    private string ResolveMediaUrl(MediaFile? media)
    {
        if (media == null) return string.Empty;
        if (media.IsPublic)
        {
            var endpoint = (_configuration["Appwrite:Endpoint"] ?? "https://syd.cloud.appwrite.io/v1").TrimEnd('/');
            var bucketId = _configuration["Appwrite:BucketId"] ?? "";
            var projectId = _configuration["Appwrite:ProjectId"] ?? "";
            var fileId = ToFileId(media.ObjectName);
            return $"{endpoint}/storage/buckets/{bucketId}/files/{Uri.EscapeDataString(fileId)}/view?project={Uri.EscapeDataString(projectId)}";
        }

        // For private: return objectName; caller should request signed URL via separate endpoint.
        // In our UI flows we primarily store private docs, not dish images.
        return media.ObjectName;
    }

    private static string ToFileId(string objectName)
    {
        var normalized = objectName.Trim();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        var hex = Convert.ToHexString(hash).ToLowerInvariant();
        return $"f_{hex[..34]}";
    }

    private async Task<DishDto> WithSignedImageAsync(DishDto dto)
    {
        if (dto == null) return new DishDto();
        var raw = dto.ImageUrl;
        if (string.IsNullOrWhiteSpace(raw)) return dto;

        // If it's already an absolute URL, keep it.
        if (raw.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return dto;
        }

        var expiresMinutes = int.TryParse(_configuration["Media:DownloadUrlExpireMinutes"], out var m) ? m : 15;
        var expiresIn = TimeSpan.FromMinutes(Math.Clamp(expiresMinutes, 1, 60));
        var signed = await _storage.CreateSignedUrlAsync(raw.Trim(), HttpMethod.Get, contentType: null, expiresIn: expiresIn);

        // If storage returned empty URL (file not found), keep original objectName so dish data is still returned.
        var resolvedUrl = string.IsNullOrWhiteSpace(signed.Url) ? raw : signed.Url;

        // Copy to avoid mutating cached instance reference.
        return new DishDto
        {
            Id = dto.Id,
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            Price = dto.Price,
            DietaryLabel = dto.DietaryLabel,
            ImageUrl = resolvedUrl,
            Calories = dto.Calories,
            Protein = dto.Protein,
            Fat = dto.Fat,
            Carbs = dto.Carbs,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Images = dto.Images
        };
    }
}
