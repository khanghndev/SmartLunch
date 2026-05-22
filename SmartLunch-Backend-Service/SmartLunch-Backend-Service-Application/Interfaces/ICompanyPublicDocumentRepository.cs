using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface ICompanyPublicDocumentRepository
{
    Task<List<CompanyPublicDocument>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<List<CompanyPublicDocument>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CompanyPublicDocument?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CompanyPublicDocument> CreateAsync(CompanyPublicDocument entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(CompanyPublicDocument entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
