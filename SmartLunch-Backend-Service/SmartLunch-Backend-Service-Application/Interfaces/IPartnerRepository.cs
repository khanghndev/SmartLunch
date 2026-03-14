using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IPartnerRepository
{
    Task<Partner?> GetByIdAsync(Guid id);
    Task<(List<Partner> Partners, int TotalCount)> GetPartnersAsync(int page, int pageSize, string? searchTerm = null, bool? isActive = null);
}