using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(int id);
    Task<(List<Transaction> Transactions, int TotalCount)> GetTransactionsAsync(int page, int pageSize, string? searchTerm = null);
    Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default);
}
