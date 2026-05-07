using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IDeliveryRepository
{
    Task<Delivery?> GetByIdAsync(int id);
    Task<Delivery?> GetByIdWithOrderAsync(int id, CancellationToken cancellationToken = default);
    Task<(List<Delivery> Deliveries, int TotalCount)> GetDeliveriesAsync(int page, int pageSize, string? searchTerm = null);

    Task<(List<Delivery> Deliveries, int TotalCount)> GetForShipperAsync(
        int shipperUserId,
        int page,
        int pageSize,
        string? status = null,
        DateOnly? scheduledOn = null,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Delivery delivery, CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
}
