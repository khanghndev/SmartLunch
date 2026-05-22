using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IContactInquiryRepository
{
    Task<ContactInquiry?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ContactInquiry> CreateAsync(ContactInquiry entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(ContactInquiry entity, CancellationToken cancellationToken = default);
    Task<(List<ContactInquiry> Items, int TotalCount, int PendingCount)> GetManagerListAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        string? status = null,
        CancellationToken cancellationToken = default);
}
