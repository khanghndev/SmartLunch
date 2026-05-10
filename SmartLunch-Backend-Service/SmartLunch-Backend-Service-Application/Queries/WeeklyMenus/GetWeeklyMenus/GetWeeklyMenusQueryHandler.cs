using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenus;

public class GetWeeklyMenusQueryHandler : IRequestHandler<GetWeeklyMenusQuery, GetWeeklyMenusResponse>
{
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;
    private readonly ILogger<GetWeeklyMenusQueryHandler> _logger;
    private readonly IConfiguration _configuration;

    public GetWeeklyMenusQueryHandler(IWeeklyMenuRepository weeklyMenuRepository, ILogger<GetWeeklyMenusQueryHandler> logger, IConfiguration configuration)
    {
        _weeklyMenuRepository = weeklyMenuRepository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<GetWeeklyMenusResponse> Handle(GetWeeklyMenusQuery request, CancellationToken cancellationToken)
    {
        var (weeklyMenus, totalCount) = await _weeklyMenuRepository.GetWeeklyMenusAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.CustomerTypeId);

        var weeklyMenuDtos = weeklyMenus.Select(weeklyMenu =>
        {
            var dto = new WeeklyMenuDto
            {
                Id = weeklyMenu.Id,
                StartDate = weeklyMenu.StartDate,
                EndDate = weeklyMenu.EndDate,
                Description = weeklyMenu.Description,
                CreatedBy = weeklyMenu.CreatedBy,
                CreatedAt = weeklyMenu.CreatedAt
            };

            var cover = weeklyMenu.WeeklyMenuImages?
                .OrderByDescending(i => string.Equals(i.Role, "cover", StringComparison.OrdinalIgnoreCase))
                .ThenBy(i => i.SortOrder)
                .FirstOrDefault();

            if (cover?.MediaFile != null)
                dto.ImageUrl = ResolveUrl(cover.MediaFile);

            return dto;
        }).ToList();

        _logger.LogInformation("Retrieved {Count} weeklymenus (Page {Page}, PageSize {PageSize})",
            weeklyMenuDtos.Count, request.Page, request.PageSize);

        return new GetWeeklyMenusResponse
        {
            Data = weeklyMenuDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private string ResolveUrl(MediaFile media)
    {
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
