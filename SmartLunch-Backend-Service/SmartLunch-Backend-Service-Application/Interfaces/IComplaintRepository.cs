using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IComplaintRepository
{
    Task<Complaint?> GetByIdAsync(Guid id);
    Task<(List<Complaint> Complaints, int TotalCount)> GetComplaintsAsync(int page, int pageSize, string? searchTerm = null);
}
