using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;
using SmartLunch.Backend.Service.Infrastructure.Data;

namespace SmartLunch.Backend.Service.Infrastructure.Repositories;

public class ComplaintRepository : IComplaintRepository
{
    private readonly SmartLunchDBContext _context;

    public ComplaintRepository(SmartLunchDBContext context)
    {
        _context = context;
    }

    public async Task<Complaint?> GetByIdAsync(int id)
    {
        return await _context.Complaints
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<(List<Complaint> Complaints, int TotalCount)> GetComplaintsAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Complaints.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.Title.Contains(searchTerm) ||
                e.Description.Contains(searchTerm) ||
                e.Status.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();

        var complaints = await query
            .OrderBy(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (complaints, totalCount);
    }
}
