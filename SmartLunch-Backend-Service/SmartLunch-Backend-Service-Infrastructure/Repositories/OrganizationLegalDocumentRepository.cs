using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class OrganizationLegalDocumentRepository : IOrganizationLegalDocumentRepository
{
    private readonly SmartLunchDBContext _db;

    public OrganizationLegalDocumentRepository(SmartLunchDBContext db) => _db = db;

    public Task<List<OrganizationLegalDocument>> GetByOrganizationIdAsync(int organizationId, CancellationToken cancellationToken = default) =>
        _db.OrganizationLegalDocuments
            .AsNoTracking()
            .Include(d => d.MediaFile)
            .Where(d => d.OrganizationId == organizationId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<OrganizationLegalDocument?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.OrganizationLegalDocuments
            .Include(d => d.MediaFile)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task<OrganizationLegalDocument> CreateAsync(OrganizationLegalDocument entity, CancellationToken cancellationToken = default)
    {
        _db.OrganizationLegalDocuments.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _db.OrganizationLegalDocuments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (row != null)
        {
            _db.OrganizationLegalDocuments.Remove(row);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
