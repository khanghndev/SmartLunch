using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface IContractWeeklySelectionRepository
{
    Task<ContractWeeklySelection?> GetByContractAndWeekAsync(
        int contractId,
        DateOnly weekMonday,
        bool includeItems = false,
        CancellationToken cancellationToken = default);

    Task<List<ContractWeeklySelection>> GetAllByContractIdAsync(
        int contractId,
        bool includeItems = false,
        CancellationToken cancellationToken = default);

    Task<(int TotalWeeks, int FilledWeeks)> GetProgressAsync(
        int contractId,
        CancellationToken cancellationToken = default);

    Task AddAsync(ContractWeeklySelection selection, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
