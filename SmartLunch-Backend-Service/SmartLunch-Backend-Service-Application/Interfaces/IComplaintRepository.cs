using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IComplaintRepository
{
    Task<Complaint?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Complaint?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<ComplaintEvidence?> GetEvidenceAsync(int evidenceId, int complaintId, CancellationToken cancellationToken = default);
    Task<(List<Complaint> Complaints, int TotalCount)> GetComplaintsAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        string? status = null,
        int? userId = null,
        CancellationToken cancellationToken = default);
    Task<Complaint> CreateAsync(Complaint complaint, CancellationToken cancellationToken = default);
    Task UpdateAsync(Complaint complaint, CancellationToken cancellationToken = default);
    Task AddEvidenceAsync(ComplaintEvidence evidence, CancellationToken cancellationToken = default);
    Task DeleteEvidenceAsync(ComplaintEvidence evidence, CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task<int> CountByOrderAndUserAsync(int orderId, int userId, CancellationToken cancellationToken = default);
}
