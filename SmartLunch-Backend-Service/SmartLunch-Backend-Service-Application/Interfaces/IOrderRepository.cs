using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<(List<Order> Orders, int TotalCount)> GetOrdersAsync(int page, int pageSize, string? searchTerm = null);
}
