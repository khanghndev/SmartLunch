using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSchedules;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.MenuSchedules.GetMenuSchedules;

public class GetMenuSchedulesQueryHandler : IRequestHandler<GetMenuSchedulesQuery, GetMenuSchedulesResponse>
{
    private readonly IMenuScheduleRepository _menuScheduleRepository;
    private readonly ILogger<GetMenuSchedulesQueryHandler> _logger;

    public GetMenuSchedulesQueryHandler(IMenuScheduleRepository menuScheduleRepository, ILogger<GetMenuSchedulesQueryHandler> logger)
    {
        _menuScheduleRepository = menuScheduleRepository;
        _logger = logger;
    }

    public async Task<GetMenuSchedulesResponse> Handle(GetMenuSchedulesQuery request, CancellationToken cancellationToken)
    {
        var (menuSchedules, totalCount) = await _menuScheduleRepository.GetMenuSchedulesAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var menuScheduleDtos = menuSchedules.Select(menuSchedule => new MenuScheduleDto
        {
                Id = menuSchedule.Id,
                MenuId = menuSchedule.MenuId,
                Date = menuSchedule.Date,
                MealSlot = menuSchedule.MealSlot,
                DishId = menuSchedule.DishId,
                CreatedAt = menuSchedule.CreatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} menuschedules (Page {Page}, PageSize {PageSize})",
            menuScheduleDtos.Count, request.Page, request.PageSize);

        return new GetMenuSchedulesResponse
        {
            Data = menuScheduleDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
