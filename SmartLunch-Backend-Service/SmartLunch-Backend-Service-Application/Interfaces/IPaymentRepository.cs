using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task<(List<Payment> Payments, int TotalCount)> GetPaymentsAsync(int page, int pageSize, string? searchTerm = null);
}
