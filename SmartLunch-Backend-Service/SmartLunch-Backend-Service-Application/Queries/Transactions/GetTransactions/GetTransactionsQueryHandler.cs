using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Transactions;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Transactions.GetTransactions;

public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, GetTransactionsResponse>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<GetTransactionsQueryHandler> _logger;

    public GetTransactionsQueryHandler(ITransactionRepository transactionRepository, ILogger<GetTransactionsQueryHandler> logger)
    {
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<GetTransactionsResponse> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var (transactions, totalCount) = await _transactionRepository.GetTransactionsAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var transactionDtos = transactions.Select(transaction => new TransactionDto
        {
                Id = transaction.Id,
                Date = transaction.Date,
                Description = transaction.Description,
                Amount = transaction.Amount,
                Category = transaction.Category,
                Method = transaction.Method,
                ReferenceId = transaction.ReferenceId,
                CreatedAt = transaction.CreatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} transactions (Page {Page}, PageSize {PageSize})",
            transactionDtos.Count, request.Page, request.PageSize);

        return new GetTransactionsResponse
        {
            Data = transactionDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
