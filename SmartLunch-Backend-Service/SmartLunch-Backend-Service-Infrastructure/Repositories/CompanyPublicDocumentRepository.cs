using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public sealed class CompanyPublicDocumentRepository : ICompanyPublicDocumentRepository
{
    private readonly SmartLunchDBContext _db;

    public CompanyPublicDocumentRepository(SmartLunchDBContext db) => _db = db;

    public Task<List<CompanyPublicDocument>> GetPublishedAsync(CancellationToken cancellationToken = default) =>
        _db.CompanyPublicDocuments
            .AsNoTracking()
            .Include(d => d.MediaFile)
            .Where(d => d.IsPublished)
            .OrderBy(d => d.SortOrder)
            .ThenByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<List<CompanyPublicDocument>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _db.CompanyPublicDocuments
            .AsNoTracking()
            .Include(d => d.MediaFile)
            .OrderBy(d => d.SortOrder)
            .ThenByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<CompanyPublicDocument?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.CompanyPublicDocuments
            .Include(d => d.MediaFile)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task<CompanyPublicDocument> CreateAsync(CompanyPublicDocument entity, CancellationToken cancellationToken = default)
    {
        _db.CompanyPublicDocuments.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(CompanyPublicDocument entity, CancellationToken cancellationToken = default)
    {
        _db.CompanyPublicDocuments.Update(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _db.CompanyPublicDocuments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (row != null)
        {
            _db.CompanyPublicDocuments.Remove(row);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
