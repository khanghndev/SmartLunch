using Microsoft.EntityFrameworkCore;
using SmartLunch.Backend.Service.Application.Constants;
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

    public Task<Complaint?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Complaints.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<Complaint?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Complaints
            .Include(c => c.Evidence.OrderBy(e => e.SortOrder).ThenBy(e => e.Id))
            .Include(c => c.User)
            .Include(c => c.Order!).ThenInclude(o => o.Contract!).ThenInclude(ct => ct.Organization)
            .Include(c => c.Order!).ThenInclude(o => o.OrderItems).ThenInclude(i => i.Dish)
            .Include(c => c.Order!).ThenInclude(o => o.Deliveries).ThenInclude(d => d.AssignedStaff)
            .Include(c => c.ResolvedByUser)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<ComplaintEvidence?> GetEvidenceAsync(int evidenceId, int complaintId, CancellationToken cancellationToken = default) =>
        _context.ComplaintEvidence.FirstOrDefaultAsync(
            e => e.Id == evidenceId && e.ComplaintId == complaintId, cancellationToken);

    public async Task<(List<Complaint> Complaints, int TotalCount)> GetComplaintsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        string? status = null,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Complaints
            .Include(c => c.Order)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(e => e.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var s = status.Trim().ToLowerInvariant();
            if (s == ComplaintStatus.PendingReview)
            {
                query = query.Where(e =>
                    e.Status == ComplaintStatus.PendingReview
                    || e.Status == ComplaintStatus.LegacyNew
                    || e.Status == ComplaintStatus.LegacyInProgress);
            }
            else
            {
                query = query.Where(e => e.Status == s);
            }
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.Title.Contains(searchTerm) ||
                e.Description.Contains(searchTerm) ||
                (e.Code != null && e.Code.Contains(searchTerm)) ||
                e.Status.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var complaints = await query
            .OrderByDescending(e => e.SubmittedAt ?? e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (complaints, totalCount);
    }

    public async Task<Complaint> CreateAsync(Complaint complaint, CancellationToken cancellationToken = default)
    {
        _context.Complaints.Add(complaint);
        await _context.SaveChangesAsync(cancellationToken);
        return complaint;
    }

    public Task UpdateAsync(Complaint complaint, CancellationToken cancellationToken = default)
    {
        _context.Complaints.Update(complaint);
        return Task.CompletedTask;
    }

    public async Task AddEvidenceAsync(ComplaintEvidence evidence, CancellationToken cancellationToken = default)
    {
        _context.ComplaintEvidence.Add(evidence);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteEvidenceAsync(ComplaintEvidence evidence, CancellationToken cancellationToken = default)
    {
        _context.ComplaintEvidence.Remove(evidence);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    public Task<int> CountByOrderAndUserAsync(int orderId, int userId, CancellationToken cancellationToken = default) =>
        _context.Complaints.CountAsync(c => c.OrderId == orderId && c.UserId == userId, cancellationToken);
}
