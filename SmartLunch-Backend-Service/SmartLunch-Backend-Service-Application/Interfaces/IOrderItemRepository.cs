using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IOrderItemRepository
{
    Task<OrderItem?> GetByIdAsync(int id);
    Task<(List<OrderItem> OrderItems, int TotalCount)> GetOrderItemsAsync(int page, int pageSize, string? searchTerm = null);
}
