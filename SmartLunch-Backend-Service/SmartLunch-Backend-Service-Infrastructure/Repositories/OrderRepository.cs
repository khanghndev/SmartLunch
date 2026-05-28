using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly SmartLunchDBContext _context;

    public OrderRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Order?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Contract).ThenInclude(c => c!.Partner)
            .Include(o => o.Contract).ThenInclude(c => c!.Organization)
            .Include(o => o.User).ThenInclude(u => u!.UserOrganizations).ThenInclude(uo => uo.Organization)
            .Include(o => o.CreatedBySalesUser)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Dish)
                    .ThenInclude(d => d.DishIngredients)
                        .ThenInclude(di => di.Ingredient)
            .Include(o => o.Payments)
            .Include(o => o.Deliveries)
            .Include(o => o.PromotionApplications)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<Order> Orders, int TotalCount)> GetOrdersAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        DateOnly? scheduledOn = null,
        string? status = null,
        int? restrictToUserId = null)
    {
        var query = _context.Orders
            .Include(o => o.Contract).ThenInclude(c => c!.Partner)
            .Include(o => o.Contract).ThenInclude(c => c!.Organization)
            .Include(o => o.OrderItems).ThenInclude(i => i.Dish)
            .AsQueryable();

        if (restrictToUserId.HasValue)
            query = query.Where(o => o.UserId == restrictToUserId.Value);

        if (scheduledOn.HasValue)
        {
            var start = scheduledOn.Value.ToDateTime(TimeOnly.MinValue);
            var end = start.AddDays(1);
            query = query.Where(o => o.ScheduledDate >= start && o.ScheduledDate < end);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            var s = status.Trim().ToLowerInvariant();
            query = query.Where(o => o.Status == s);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(e =>
                e.Status.Contains(term) ||
                e.PaymentStatus.Contains(term) ||
                (e.InvoiceCode != null && e.InvoiceCode.Contains(term)) ||
                (e.Code != null && e.Code.Contains(term)) ||
                e.Id.ToString() == term ||
                (e.Contract != null && e.Contract.Organization != null && e.Contract.Organization.Name.Contains(term)));
        }

        var totalCount = await query.CountAsync();

        var orders = await query
            .OrderBy(e => e.ScheduledDate)
            .ThenBy(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalCount);
    }

    public async Task<List<MealStatisticItemDto>> GetMealStatisticsAsync(
        DateTime? startDate,
        DateTime? endDate,
        int? organizationId)
    {
        var query = _context.Orders
            .Include(o => o.Contract).ThenInclude(c => c.Organization)
            .Include(o => o.OrderItems)
            .Where(o => o.Status != "cancelled")
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(o => o.ScheduledDate >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(o => o.ScheduledDate <= endDate.Value);

        if (organizationId.HasValue)
            query = query.Where(o => o.Contract!.OrganizationId == organizationId.Value);

        var orders = await query.ToListAsync();

        var result = orders
            .SelectMany(o => o.OrderItems.Select(i => new { Order = o, Item = i }))
            .GroupBy(x => new 
            { 
                Date = DateOnly.FromDateTime(x.Order.ScheduledDate),
                MealSlot = x.Order.ScheduledDate.TimeOfDay.Hours < 15 ? "Lunch" : "Dinner",
                OrganizationId = x.Order.Contract?.OrganizationId,
                OrganizationName = x.Order.Contract?.Organization?.Name ?? "Unknown"
            })
            .Select(g => new MealStatisticItemDto
            {
                Date = g.Key.Date,
                MealSlot = g.Key.MealSlot,
                OrganizationId = g.Key.OrganizationId,
                OrganizationName = g.Key.OrganizationName,
                TotalMeals = g.Sum(x => x.Item.Quantity),
                TotalAmount = g.Sum(x => x.Item.TotalPrice)
            })
            .OrderByDescending(x => x.Date)
            .ThenBy(x => x.OrganizationName)
            .ToList();

        return result;
    }

    public async Task<List<DetailedMealItemDto>> GetDetailedMealStatisticsAsync(
        DateTime? startDate,
        DateTime? endDate,
        int? organizationId)
    {
        var query = _context.Orders
            .Include(o => o.Contract).ThenInclude(c => c.Organization)
            .Include(o => o.OrderItems).ThenInclude(i => i.Dish)
            .Where(o => o.Status != "cancelled")
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(o => o.ScheduledDate >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(o => o.ScheduledDate <= endDate.Value);

        if (organizationId.HasValue)
            query = query.Where(o => o.Contract!.OrganizationId == organizationId.Value);

        var orders = await query.ToListAsync();

        // Get all menu schedules for the date range to match with orders
        var start = startDate ?? orders.Min(o => (DateTime?)o.ScheduledDate) ?? DateTime.MinValue;
        var end = endDate ?? orders.Max(o => (DateTime?)o.ScheduledDate) ?? DateTime.MaxValue;

        var menuSchedules = await _context.MenuSchedules
            .Include(ms => ms.Menu)
            .Where(ms => ms.Date >= start.Date && ms.Date <= end.Date)
            .ToListAsync();

        var result = orders
            .SelectMany(o => o.OrderItems.Select(i => new { Order = o, Item = i }))
            .Select(x => 
            {
                var date = DateOnly.FromDateTime(x.Order.ScheduledDate);
                var slot = x.Order.ScheduledDate.TimeOfDay.Hours < 15 ? "lunch" : "dinner";
                
                // Try to find the matching menu schedule
                var schedule = menuSchedules.FirstOrDefault(ms => 
                    ms.Date.Date == x.Order.ScheduledDate.Date && 
                    ms.DishId == x.Item.DishId && 
                    ms.MealSlot.ToLower() == slot);

                return new 
                { 
                    Date = date,
                    MealSlot = slot == "lunch" ? "Lunch" : "Dinner",
                    OrganizationId = x.Order.Contract?.OrganizationId,
                    OrganizationName = x.Order.Contract?.Organization?.Name ?? "Unknown",
                    MenuId = schedule?.MenuId,
                    MenuName = schedule?.Menu?.Description ?? "General Menu",
                    DishId = x.Item.DishId,
                    DishName = x.Item.Dish?.Name ?? "Unknown",
                    Quantity = x.Item.Quantity,
                    TotalPrice = x.Item.TotalPrice
                };
            })
            .GroupBy(x => new 
            { 
                x.Date,
                x.MealSlot,
                x.OrganizationId,
                x.OrganizationName,
                x.MenuId,
                x.MenuName,
                x.DishId,
                x.DishName
            })
            .Select(g => new DetailedMealItemDto
            {
                Date = g.Key.Date,
                MealSlot = g.Key.MealSlot,
                OrganizationId = g.Key.OrganizationId,
                OrganizationName = g.Key.OrganizationName,
                MenuId = g.Key.MenuId,
                MenuName = g.Key.MenuName,
                DishId = g.Key.DishId,
                DishName = g.Key.DishName,
                Quantity = g.Sum(x => x.Quantity),
                TotalAmount = g.Sum(x => x.TotalPrice)
            })
            .OrderByDescending(x => x.Date)
            .ThenBy(x => x.OrganizationName)
            .ThenBy(x => x.DishName)
            .ToList();

        return result;
    }

    public async Task<List<(DateOnly ServiceDate, int DishId, int QuantityMeals, int? ContractId)>> GetUpcomingDishDemandAsync(
        DateOnly startDate,
        DateOnly endDateInclusive,
        CancellationToken cancellationToken = default)
    {
        var start = startDate.ToDateTime(TimeOnly.MinValue);
        var endExclusive = endDateInclusive.AddDays(1).ToDateTime(TimeOnly.MinValue);

        // Pull order items within range; service date wins if present.
        var rows = await _context.OrderItems
            .AsNoTracking()
            .Where(oi =>
                oi.Order.Status != "cancelled" &&
                (
                    (oi.ServiceDate.HasValue && oi.ServiceDate.Value >= startDate && oi.ServiceDate.Value <= endDateInclusive)
                    || (!oi.ServiceDate.HasValue && oi.Order.ScheduledDate >= start && oi.Order.ScheduledDate < endExclusive)
                )
            )
            .Select(oi => new
            {
                ServiceDate = oi.ServiceDate ?? DateOnly.FromDateTime(oi.Order.ScheduledDate),
                oi.DishId,
                oi.Quantity,
                oi.Order.ContractId
            })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(x => new { x.ServiceDate, x.DishId, x.ContractId })
            .Select(g => (g.Key.ServiceDate, g.Key.DishId, g.Sum(x => x.Quantity), g.Key.ContractId))
            .OrderBy(x => x.ServiceDate)
            .ThenBy(x => x.DishId)
            .ToList();
    }

    public Task<bool> InvoiceCodeExistsAsync(string invoiceCode, CancellationToken cancellationToken = default)
    {
        return _context.Orders.AnyAsync(o => o.InvoiceCode == invoiceCode, cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public Task CommitAsync() => _context.SaveChangesAsync();

    public async Task<List<Order>> GetOrdersPendingPaymentReminderAsync(
        DateTime annexSignedBefore,
        int maxCount,
        CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Contract).ThenInclude(c => c!.Organization)
            .Include(o => o.Payments)
            .Where(o =>
                o.RecipientEmail != null &&
                o.RecipientEmail != "" &&
                o.PaymentReminderSentAt == null &&
                o.AnnexSignedAt != null &&
                o.AnnexSignedAt <= annexSignedBefore &&
                o.PaymentStatus == "awaiting_payment")
            .OrderBy(o => o.AnnexSignedAt)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetContractWeekOrderAsync(
        int contractId,
        DateOnly weekMonday,
        CancellationToken cancellationToken = default)
    {
        var scheduledUtc = VietnamTime.CalendarDateMidnight(weekMonday);
        return await _context.Orders
            .Include(o => o.OrderItems).ThenInclude(i => i.Dish).ThenInclude(d => d.DishDishCategories).ThenInclude(ddc => ddc.DishCategory)
            .FirstOrDefaultAsync(
                o => o.ContractId == contractId && o.ScheduledDate == scheduledUtc,
                cancellationToken);
    }

    public async Task<bool> ContractWeekHasMainItemsAsync(
        int contractId,
        DateOnly weekMonday,
        CancellationToken cancellationToken = default)
    {
        var scheduledUtc = VietnamTime.CalendarDateMidnight(weekMonday);
        return await _context.OrderItems
            .AnyAsync(
                i => i.Order != null &&
                     i.Order.ContractId == contractId &&
                     i.Order.ScheduledDate == scheduledUtc &&
                     i.ServiceDate != null &&
                     i.Dish.DishDishCategories.Any(ddc =>
                         ddc.DishCategory != null &&
                         string.Equals(ddc.DishCategory.SlotKey, "main", StringComparison.OrdinalIgnoreCase)),
                cancellationToken);
    }
}
