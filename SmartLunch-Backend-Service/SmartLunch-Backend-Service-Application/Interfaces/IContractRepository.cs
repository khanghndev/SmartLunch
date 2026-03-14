using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(Guid id);
    Task<(List<Contract> Contracts, int TotalCount)> GetContractsAsync(int page, int pageSize, string? searchTerm = null);
}
