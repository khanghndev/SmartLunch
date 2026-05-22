using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class ContactInquiryRepository : IContactInquiryRepository
{
    private readonly SmartLunchDBContext _context;

    public ContactInquiryRepository(SmartLunchDBContext context) => _context = context;

    public Task<ContactInquiry?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.ContactInquiries
            .Include(c => c.RepliedByUser)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<ContactInquiry> CreateAsync(ContactInquiry entity, CancellationToken cancellationToken = default)
    {
        await _context.ContactInquiries.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(ContactInquiry entity, CancellationToken cancellationToken = default)
    {
        _context.ContactInquiries.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(List<ContactInquiry> Items, int TotalCount, int PendingCount)> GetManagerListAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ContactInquiries.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(c => c.Status == status);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(c =>
                c.FullName.Contains(term)
                || c.Phone.Contains(term)
                || c.Email.Contains(term)
                || (c.Code != null && c.Code.Contains(term))
                || c.InterestedService.Contains(term)
                || (c.Message != null && c.Message.Contains(term)));
        }

        var pendingCount = await _context.ContactInquiries.AsNoTracking()
            .CountAsync(c => c.Status == "pending", cancellationToken);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount, pendingCount);
    }
}
