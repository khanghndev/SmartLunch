using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenu;

public class GetWeeklyMenuQueryHandler : IRequestHandler<GetWeeklyMenuQuery, GetWeeklyMenuResponse>
{
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;
    private readonly ILogger<GetWeeklyMenuQueryHandler> _logger;

    public GetWeeklyMenuQueryHandler(IWeeklyMenuRepository weeklyMenuRepository, ILogger<GetWeeklyMenuQueryHandler> logger)
    {
        _weeklyMenuRepository = weeklyMenuRepository;
        _logger = logger;
    }

    public async Task<GetWeeklyMenuResponse> Handle(GetWeeklyMenuQuery request, CancellationToken cancellationToken)
    {
        var weeklyMenu = await _weeklyMenuRepository.GetByIdAsync(request.WeeklyMenuId);

        if (weeklyMenu == null)
        {
            _logger.LogWarning("WeeklyMenu not found with ID: {WeeklyMenuId}", request.WeeklyMenuId);
            return new GetWeeklyMenuResponse { WeeklyMenu = new WeeklyMenuDto() };
        }

        return new GetWeeklyMenuResponse
        {
            WeeklyMenu = new WeeklyMenuDto
            {
                Id = weeklyMenu.Id,
                StartDate = weeklyMenu.StartDate,
                EndDate = weeklyMenu.EndDate,
                Description = weeklyMenu.Description,
                CreatedBy = weeklyMenu.CreatedBy,
                CreatedAt = weeklyMenu.CreatedAt
            }
        };
    }
}
