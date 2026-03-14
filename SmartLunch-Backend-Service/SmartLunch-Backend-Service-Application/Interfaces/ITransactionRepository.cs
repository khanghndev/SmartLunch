using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<(List<Transaction> Transactions, int TotalCount)> GetTransactionsAsync(int page, int pageSize, string? searchTerm = null);
}
