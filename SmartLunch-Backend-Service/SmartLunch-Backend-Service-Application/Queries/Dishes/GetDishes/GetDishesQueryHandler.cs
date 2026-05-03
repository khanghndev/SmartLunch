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

namespace SmartLunch.Backend.Service.Application.Queries.Dishes.GetDishes;

public class GetDishesQueryHandler : IRequestHandler<GetDishesQuery, GetDishesResponse>
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
    private readonly ICacheService _cacheService;
    private readonly IDishRepository _dishRepository;
    private readonly ILogger<GetDishesQueryHandler> _logger;
    private readonly IStorageService _storage;
    private readonly IConfiguration _configuration;

    public GetDishesQueryHandler(
        ICacheService cacheService,
        IDishRepository dishRepository,
        ILogger<GetDishesQueryHandler> logger,
        IStorageService storage,
        IConfiguration configuration)
    {
        _cacheService = cacheService;
        _dishRepository = dishRepository;
        _logger = logger;
        _storage = storage;
        _configuration = configuration;
    }

    public async Task<GetDishesResponse> Handle(GetDishesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = MasterDataCacheKeys.Dishes(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.IsActive,
            request.Category);

        var cached = await _cacheService.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var (dishes, totalCount) = await _dishRepository.GetDishesAsync(
                    request.Page,
                    request.PageSize,
                    request.SearchTerm,
                    request.IsActive,
                    request.Category);

                var dishDtos = dishes.Select(DishDtoMapping.ToDto).ToList();

                _logger.LogInformation("Retrieved {Count} dishes (Page {Page}, PageSize {PageSize})",
                    dishDtos.Count, request.Page, request.PageSize);

                return new GetDishesResponse
                {
                    Data = dishDtos,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
            },
            CacheDuration,
            cancellationToken);

        // Never cache signed URLs (they expire). Cache stores objectName in Dish.ImageUrl.
        // Repo now includes DishImages + MediaFile, but mapping loses it; we only keep cover in ImageUrl.
        var signed = new List<DishDto>(cached.Data.Count);
        foreach (var dto in cached.Data)
        {
            signed.Add(await WithSignedImageAsync(dto));
        }

        return new GetDishesResponse
        {
            Data = signed,
            TotalCount = cached.TotalCount,
            Page = cached.Page,
            PageSize = cached.PageSize
        };
    }

    private async Task<DishDto> WithSignedImageAsync(DishDto dto)
    {
        if (dto == null) return new DishDto();
        var raw = dto.ImageUrl;
        if (string.IsNullOrWhiteSpace(raw)) return dto;

        if (raw.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return dto;
        }

        var expiresMinutes = int.TryParse(_configuration["Media:DownloadUrlExpireMinutes"], out var m) ? m : 15;
        var expiresIn = TimeSpan.FromMinutes(Math.Clamp(expiresMinutes, 1, 60));
        var signed = await _storage.CreateSignedUrlAsync(raw.Trim(), HttpMethod.Get, contentType: null, expiresIn: expiresIn);

        // If storage returned empty URL (file not found on Appwrite), keep original objectName.
        var resolvedUrl = string.IsNullOrWhiteSpace(signed.Url) ? raw : signed.Url;

        return new DishDto
        {
            Id = dto.Id,
            Code = dto.Code,
            Name = dto.Name,
            NameEnglish = dto.NameEnglish,
            Description = dto.Description,
            PrimarySlotKey = dto.PrimarySlotKey,
            DishSlotCategoryCodes = [..dto.DishSlotCategoryCodes],
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
            UpdatedAt = dto.UpdatedAt
        };
    }

    private static string ToFileId(string objectName)
    {
        var normalized = objectName.Trim();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        var hex = Convert.ToHexString(hash).ToLowerInvariant();
        return $"f_{hex[..34]}";
    }
}
