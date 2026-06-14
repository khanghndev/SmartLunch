using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.WeeklyMenus;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Queries.WeeklyMenus.GetWeeklyMenuDetail;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.WeeklyMenus.CreateWeeklyMenu;

public class CreateWeeklyMenuCommandHandler : IRequestHandler<CreateWeeklyMenuCommand, GetWeeklyMenuDetailResponse>
{
    private readonly IWeeklyMenuRepository _weeklyMenuRepository;
    private readonly IDishRepository _dishRepository;
    private readonly ICustomerTypeRepository _customerTypeRepository;
    private readonly IMediator _mediator;

    public CreateWeeklyMenuCommandHandler(
        IWeeklyMenuRepository weeklyMenuRepository,
        IDishRepository dishRepository,
        ICustomerTypeRepository customerTypeRepository,
        IMediator mediator)
    {
        _weeklyMenuRepository = weeklyMenuRepository;
        _dishRepository = dishRepository;
        _customerTypeRepository = customerTypeRepository;
        _mediator = mediator;
    }

    public async Task<GetWeeklyMenuDetailResponse> Handle(CreateWeeklyMenuCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;
        var start = req.StartDate.Date;
        var end = req.EndDate.Date;

        if (end < start)
            throw new ArgumentException("Ngày kết thúc phải sau hoặc bằng ngày bắt đầu.");

        var menuType = string.IsNullOrWhiteSpace(req.MenuType) ? "General" : req.MenuType.Trim();

        if (req.CustomerTypeId.HasValue)
        {
            var types = await _customerTypeRepository.GetAllAsync(cancellationToken);
            if (types.All(t => t.Id != req.CustomerTypeId.Value))
                throw new ArgumentException("Loại khách hàng không hợp lệ.");
        }

        // Removed ExistsByPeriodAsync check because one period can have multiple menus (e.g. from AI plans)

        var rawSchedules = req.Schedules ?? new List<CreateMenuScheduleItemRequest>();
        if (rawSchedules.Count == 0)
            throw new ArgumentException("Vui lòng thêm ít nhất một món vào lịch thực đơn.");

        var normalized = rawSchedules
            .Select(s => new
            {
                Date = s.Date.Date,
                MealSlot = string.IsNullOrWhiteSpace(s.MealSlot) ? "lunch" : s.MealSlot.Trim().ToLowerInvariant(),
                DishId = s.DishId
            })
            .Where(s => s.DishId > 0)
            .GroupBy(s => (s.Date, s.MealSlot, s.DishId))
            .Select(g => g.First())
            .ToList();

        if (normalized.Count == 0)
            throw new ArgumentException("Vui lòng chọn món ăn hợp lệ.");

        foreach (var s in normalized)
        {
            if (s.Date < start || s.Date > end)
                throw new ArgumentException($"Ngày {s.Date:dd/MM/yyyy} nằm ngoài khoảng thực đơn.");
        }

        var dishIds = normalized.Select(s => s.DishId).Distinct().ToList();
        var dishes = await _dishRepository.GetByIdsAsync(dishIds, cancellationToken);
        if (dishes.Count != dishIds.Count)
            throw new ArgumentException("Một hoặc nhiều món ăn không tồn tại.");

        var inactive = dishes.Where(d => !d.IsActive).Select(d => d.Name).ToList();
        if (inactive.Count > 0)
            throw new ArgumentException($"Các món không còn hoạt động: {string.Join(", ", inactive)}.");

        var menu = new WeeklyMenu
        {
            StartDate = start,
            EndDate = end,
            MenuType = menuType,
            CustomerTypeId = req.CustomerTypeId,
            Description = string.IsNullOrWhiteSpace(req.Description) ? null : req.Description.Trim(),
            CreatedBy = command.CreatedByUserId,
            CreatedAt = VietnamTime.Now
        };

        var schedules = normalized.Select(s => new MenuSchedule
        {
            Date = s.Date,
            MealSlot = s.MealSlot,
            DishId = s.DishId,
            CreatedAt = VietnamTime.Now
        }).ToList();

        var created = await _weeklyMenuRepository.CreateWithSchedulesAsync(menu, schedules, cancellationToken);

        return await _mediator.Send(new GetWeeklyMenuDetailQuery(created.Id), cancellationToken);
    }
}
