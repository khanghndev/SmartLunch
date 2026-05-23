using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IOrganizationLegalDocumentRepository
{
    Task<List<OrganizationLegalDocument>> GetByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default);
    Task<OrganizationLegalDocument?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OrganizationLegalDocument> CreateAsync(OrganizationLegalDocument entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
