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
}
