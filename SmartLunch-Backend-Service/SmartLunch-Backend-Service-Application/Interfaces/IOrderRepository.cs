using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;
namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> GetByIdWithDetailsAsync(int id);
    Task<(List<Order> Orders, int TotalCount)> GetOrdersAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        DateOnly? scheduledOn = null,
        string? status = null,
        string? paymentStatus = null,
        int? restrictToUserId = null);

    Task<List<MealStatisticItemDto>> GetMealStatisticsAsync(
        DateTime? startDate,
        DateTime? endDate,
        int? organizationId);

    Task<List<DetailedMealItemDto>> GetDetailedMealStatisticsAsync(
        DateTime? startDate,
        DateTime? endDate,
        int? organizationId);

    Task<bool> InvoiceCodeExistsAsync(string invoiceCode, CancellationToken cancellationToken = default);

    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>Persist changes to tracked orders (and related entities on the same context).</summary>
    Task CommitAsync();

    /// <summary>Đơn đã ký phụ lục, chưa gửi nhắc thanh toán, chờ thanh toán.</summary>
    Task<List<Order>> GetOrdersPendingPaymentReminderAsync(
        DateTime annexSignedBefore,
        int maxCount,
        CancellationToken cancellationToken = default);

    /// <summary>Đơn tuần gắn HĐ (ScheduledDate = Thứ 2 tuần đó).</summary>
    Task<Order?> GetContractWeekOrderAsync(
        int contractId,
        DateOnly weekMonday,
        CancellationToken cancellationToken = default);

    Task<bool> ContractWeekHasMainItemsAsync(
        int contractId,
        DateOnly weekMonday,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Upcoming dish demand aggregated by service date.
    /// Uses OrderItem.ServiceDate if present; otherwise uses Order.ScheduledDate (date part).
    /// Excludes cancelled orders.
    /// </summary>
    Task<List<(DateOnly ServiceDate, int DishId, int QuantityMeals, int? ContractId)>> GetUpcomingDishDemandAsync(
        DateOnly startDate,
        DateOnly endDateInclusive,
        CancellationToken cancellationToken = default);
}
