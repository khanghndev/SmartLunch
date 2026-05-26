using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenuDetail;

public class GetWeeklyMenuDetailQueryHandler : IRequestHandler<GetWeeklyMenuDetailQuery, GetWeeklyMenuDetailResponse>
{
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;
    private readonly ILogger<GetWeeklyMenuDetailQueryHandler> _logger;
    private readonly IConfiguration _configuration;
    private readonly IStorageService _storage;

    public GetWeeklyMenuDetailQueryHandler(
        IWeeklyMenuRepository weeklyMenuRepository,
        ILogger<GetWeeklyMenuDetailQueryHandler> logger,
        IConfiguration configuration,
        IStorageService storage)
    {
        _weeklyMenuRepository = weeklyMenuRepository;
        _logger = logger;
        _configuration = configuration;
        _storage = storage;
    }

    public async Task<GetWeeklyMenuDetailResponse> Handle(GetWeeklyMenuDetailQuery request, CancellationToken cancellationToken)
    {
        var weeklyMenu = await _weeklyMenuRepository.GetByIdWithSchedulesAndImagesAsync(request.WeeklyMenuId, cancellationToken);

        if (weeklyMenu == null)
        {
            _logger.LogWarning("WeeklyMenu not found with ID: {WeeklyMenuId}", request.WeeklyMenuId);
            return new GetWeeklyMenuDetailResponse();
        }

        var header = new WeeklyMenuDto
        {
            Id = weeklyMenu.Id,
            StartDate = weeklyMenu.StartDate,
            EndDate = weeklyMenu.EndDate,
            Description = weeklyMenu.Description,
            CreatedBy = weeklyMenu.CreatedBy,
            CreatedAt = weeklyMenu.CreatedAt
        };

        header = WithMenuImages(header, weeklyMenu.WeeklyMenuImages);

        var schedules = new List<WeeklyMenuScheduleDetailDto>();
        foreach (var ms in weeklyMenu.MenuSchedules
                     .OrderBy(ms => ms.Date)
                     .ThenBy(ms => ms.MealSlot)
                     .ThenBy(ms => ms.Id))
        {
            var dish = ms.Dish;
            WeeklyMenuScheduleDishSummaryDto dishSummary;
            if (dish == null)
            {
                dishSummary = new WeeklyMenuScheduleDishSummaryDto();
            }
            else
            {
                var rawImageUrl = ResolveDishCoverObjectName(dish);
                dishSummary = new WeeklyMenuScheduleDishSummaryDto
                {
                    Id = dish.Id,
                    Code = dish.Code,
                    Name = dish.Name,
                    Category = DishDtoMapping.FormatMealSlotNamesDisplay(dish),
                    Price = dish.Price,
                    ImageUrl = await ResolveSignedImageUrlAsync(rawImageUrl, cancellationToken)
                };
            }

            schedules.Add(new WeeklyMenuScheduleDetailDto
            {
                Id = ms.Id,
                Code = ms.Code,
                MenuId = ms.MenuId,
                Date = ms.Date,
                MealSlot = ms.MealSlot,
                DishId = ms.DishId,
                CreatedAt = ms.CreatedAt,
                Dish = dishSummary
            });
        }

        return new GetWeeklyMenuDetailResponse
        {
            WeeklyMenu = header,
            Schedules = schedules
        };
    }

    private static string? ResolveDishCoverObjectName(Dish dish)
    {
        if (dish.DishImages != null && dish.DishImages.Count > 0)
        {
            var cover = dish.DishImages
                .OrderByDescending(i => string.Equals(i.Role, "cover", StringComparison.OrdinalIgnoreCase))
                .ThenBy(i => i.SortOrder)
                .Select(i => i.MediaFile?.ObjectName)
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

            if (!string.IsNullOrWhiteSpace(cover))
                return cover.Trim();
        }

        return string.IsNullOrWhiteSpace(dish.ImageUrl) ? null : dish.ImageUrl.Trim();
    }

    private async Task<string?> ResolveSignedImageUrlAsync(string? raw, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        if (raw.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return raw;
        }

        var expiresMinutes = int.TryParse(_configuration["Media:DownloadUrlExpireMinutes"], out var m) ? m : 15;
        var expiresIn = TimeSpan.FromMinutes(Math.Clamp(expiresMinutes, 1, 60));
        var signed = await _storage.CreateSignedUrlAsync(raw.Trim(), HttpMethod.Get, contentType: null, expiresIn: expiresIn);

        return string.IsNullOrWhiteSpace(signed.Url) ? raw : signed.Url;
    }

    private WeeklyMenuDto WithMenuImages(WeeklyMenuDto dto, ICollection<WeeklyMenuImage> images)
    {
        if (images == null || images.Count == 0) return dto;

        var cover = images
            .OrderByDescending(i => string.Equals(i.Role, "cover", StringComparison.OrdinalIgnoreCase))
            .ThenBy(i => i.SortOrder)
            .FirstOrDefault();

        if (cover?.MediaFile?.ObjectName != null)
            dto.ImageUrl = ResolveUrl(cover.MediaFile);

        dto.Images = images
            .OrderByDescending(i => string.Equals(i.Role, "cover", StringComparison.OrdinalIgnoreCase))
            .ThenBy(i => i.SortOrder)
            .Select(i => new WeeklyMenuImageDto
            {
                Id = i.Id,
                MediaFileId = i.MediaFileId,
                Role = i.Role,
                SortOrder = i.SortOrder,
                Url = ResolveUrl(i.MediaFile)
            })
            .ToList();

        return dto;
    }

    private string ResolveUrl(MediaFile? media)
    {
        if (media == null) return string.Empty;
        if (!media.IsPublic) return media.ObjectName;

        var endpoint = (_configuration["Appwrite:Endpoint"] ?? "https://syd.cloud.appwrite.io/v1").TrimEnd('/');
        var bucketId = _configuration["Appwrite:BucketId"] ?? "";
        var projectId = _configuration["Appwrite:ProjectId"] ?? "";
        var fileId = ToFileId(media.ObjectName);
        return $"{endpoint}/storage/buckets/{bucketId}/files/{Uri.EscapeDataString(fileId)}/view?project={Uri.EscapeDataString(projectId)}";
    }

    private static string ToFileId(string objectName)
    {
        var normalized = objectName.Trim();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        var hex = Convert.ToHexString(hash).ToLowerInvariant();
        return $"f_{hex[..34]}";
    }
}
