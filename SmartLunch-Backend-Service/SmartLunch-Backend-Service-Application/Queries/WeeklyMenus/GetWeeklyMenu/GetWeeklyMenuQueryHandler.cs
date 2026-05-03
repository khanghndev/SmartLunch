using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenu;

public class GetWeeklyMenuQueryHandler : IRequestHandler<GetWeeklyMenuQuery, GetWeeklyMenuResponse>
{
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;
    private readonly ILogger<GetWeeklyMenuQueryHandler> _logger;
    private readonly IConfiguration _configuration;

    public GetWeeklyMenuQueryHandler(IWeeklyMenuRepository weeklyMenuRepository, ILogger<GetWeeklyMenuQueryHandler> logger, IConfiguration configuration)
    {
        _weeklyMenuRepository = weeklyMenuRepository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<GetWeeklyMenuResponse> Handle(GetWeeklyMenuQuery request, CancellationToken cancellationToken)
    {
        var weeklyMenu = await _weeklyMenuRepository.GetByIdAsync(request.WeeklyMenuId);

        if (weeklyMenu == null)
        {
            _logger.LogWarning("WeeklyMenu not found with ID: {WeeklyMenuId}", request.WeeklyMenuId);
            return new GetWeeklyMenuResponse { WeeklyMenu = new WeeklyMenuDto() };
        }

        var dto = new WeeklyMenuDto
        {
            Id = weeklyMenu.Id,
            StartDate = weeklyMenu.StartDate,
            EndDate = weeklyMenu.EndDate,
            Description = weeklyMenu.Description,
            CreatedBy = weeklyMenu.CreatedBy,
            CreatedAt = weeklyMenu.CreatedAt
        };

        dto = WithMenuImages(dto, weeklyMenu.WeeklyMenuImages);

        return new GetWeeklyMenuResponse { WeeklyMenu = dto };
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
