using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IPartnerPaymentRepository
{
    Task<PartnerPayment?> GetByIdAsync(Guid id);
    Task<(List<PartnerPayment> PartnerPayments, int TotalCount)> GetPartnerPaymentsAsync(int page, int pageSize, string? searchTerm = null);
}
