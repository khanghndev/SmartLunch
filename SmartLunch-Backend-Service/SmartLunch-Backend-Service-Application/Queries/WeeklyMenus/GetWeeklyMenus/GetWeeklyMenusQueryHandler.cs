using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenus;

public class GetWeeklyMenusQueryHandler : IRequestHandler<GetWeeklyMenusQuery, GetWeeklyMenusResponse>
{
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;
    private readonly ILogger<GetWeeklyMenusQueryHandler> _logger;

    public GetWeeklyMenusQueryHandler(IWeeklyMenuRepository weeklyMenuRepository, ILogger<GetWeeklyMenusQueryHandler> logger)
    {
        _weeklyMenuRepository = weeklyMenuRepository;
        _logger = logger;
    }

    public async Task<GetWeeklyMenusResponse> Handle(GetWeeklyMenusQuery request, CancellationToken cancellationToken)
    {
        var (weeklyMenus, totalCount) = await _weeklyMenuRepository.GetWeeklyMenusAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var weeklyMenuDtos = weeklyMenus.Select(weeklyMenu => new WeeklyMenuDto
        {
                Id = weeklyMenu.Id,
                StartDate = weeklyMenu.StartDate,
                EndDate = weeklyMenu.EndDate,
                Description = weeklyMenu.Description,
                CreatedBy = weeklyMenu.CreatedBy,
                CreatedAt = weeklyMenu.CreatedAt
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
}
