using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IDeliveryRepository
{
    Task<Delivery?> GetByIdAsync(Guid id);
    Task<(List<Delivery> Deliveries, int TotalCount)> GetDeliveriesAsync(int page, int pageSize, string? searchTerm = null);
}
