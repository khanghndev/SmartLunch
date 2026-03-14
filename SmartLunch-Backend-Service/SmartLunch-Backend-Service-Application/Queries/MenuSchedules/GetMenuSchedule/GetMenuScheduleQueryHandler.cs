using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSchedules;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.MenuSchedules.GetMenuSchedule;

public class GetMenuScheduleQueryHandler : IRequestHandler<GetMenuScheduleQuery, GetMenuScheduleResponse>
{
    private readonly IMenuScheduleRepository _menuScheduleRepository;
    private readonly ILogger<GetMenuScheduleQueryHandler> _logger;

    public GetMenuScheduleQueryHandler(IMenuScheduleRepository menuScheduleRepository, ILogger<GetMenuScheduleQueryHandler> logger)
    {
        _menuScheduleRepository = menuScheduleRepository;
        _logger = logger;
    }

    public async Task<GetMenuScheduleResponse> Handle(GetMenuScheduleQuery request, CancellationToken cancellationToken)
    {
        var menuSchedule = await _menuScheduleRepository.GetByIdAsync(request.MenuScheduleId);

        if (menuSchedule == null)
        {
            _logger.LogWarning("MenuSchedule not found with ID: {MenuScheduleId}", request.MenuScheduleId);
            return new GetMenuScheduleResponse { MenuSchedule = new MenuScheduleDto() };
        }

        return new GetMenuScheduleResponse
        {
            MenuSchedule = new MenuScheduleDto
            {
                Id = menuSchedule.Id,
                MenuId = menuSchedule.MenuId,
                Date = menuSchedule.Date,
                MealSlot = menuSchedule.MealSlot,
                DishId = menuSchedule.DishId,
                CreatedAt = menuSchedule.CreatedAt
            }
        };
    }
}
