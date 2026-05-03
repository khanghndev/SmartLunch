using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.WeeklyMenu;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.CustomerFeatures.WeeklyMenu;

public class GetCustomerWeeklyMenuQueryHandler : IRequestHandler<GetCustomerWeeklyMenuQuery, GetCustomerWeeklyMenuResponse>
{
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;
    private readonly ILogger<GetCustomerWeeklyMenuQueryHandler> _logger;

    public GetCustomerWeeklyMenuQueryHandler(IWeeklyMenuRepository weeklyMenuRepository, ILogger<GetCustomerWeeklyMenuQueryHandler> logger)
    {
        _weeklyMenuRepository = weeklyMenuRepository;
        _logger = logger;
    }

    public async Task<GetCustomerWeeklyMenuResponse> Handle(GetCustomerWeeklyMenuQuery request, CancellationToken cancellationToken)
    {
        var targetDate = request.Date ?? DateTime.UtcNow;

        var weeklyMenu = await _weeklyMenuRepository.GetWeeklyMenuWithSchedulesByDateAsync(targetDate);

        if (weeklyMenu == null)
        {
            _logger.LogInformation("No WeeklyMenu found for date: {Date}", targetDate);
            return new GetCustomerWeeklyMenuResponse(); // Returns null WeeklyMenu property
        }

        var dto = new CustomerWeeklyMenuDto
        {
            Id = weeklyMenu.Id,
            StartDate = weeklyMenu.StartDate,
            EndDate = weeklyMenu.EndDate,
            Description = weeklyMenu.Description,
            Schedules = weeklyMenu.MenuSchedules.Select(ms => new CustomerMenuScheduleDto
            {
                Id = ms.Id,
                Date = ms.Date,
                MealSlot = ms.MealSlot,
                Dish = new CustomerDishDto
                {
                    Id = ms.Dish.Id,
                    Name = ms.Dish.Name,
                    Description = ms.Dish.Description,
                    Category = DishDtoMapping.FormatMealSlotNamesDisplay(ms.Dish),
                    Price = ms.Dish.Price,
                    DietaryLabel = ms.Dish.DietaryLabel,
                    ImageUrl = ms.Dish.ImageUrl,
                    Calories = ms.Dish.Calories
                }
            }).OrderBy(s => s.Date).ThenBy(s => s.MealSlot).ToList()
        };

        return new GetCustomerWeeklyMenuResponse
        {
            WeeklyMenu = dto
        };
    }
}
