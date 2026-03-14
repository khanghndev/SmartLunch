using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Transactions;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Transactions.GetTransaction;

public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, GetTransactionResponse>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<GetTransactionQueryHandler> _logger;

    public GetTransactionQueryHandler(ITransactionRepository transactionRepository, ILogger<GetTransactionQueryHandler> logger)
    {
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<GetTransactionResponse> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.TransactionId);

        if (transaction == null)
        {
            _logger.LogWarning("Transaction not found with ID: {TransactionId}", request.TransactionId);
            return new GetTransactionResponse { Transaction = new TransactionDto() };
        }

        return new GetTransactionResponse
        {
            Transaction = new TransactionDto
            {
                Id = transaction.Id,
                Date = transaction.Date,
                Description = transaction.Description,
                Amount = transaction.Amount,
                Category = transaction.Category,
                Method = transaction.Method,
                ReferenceId = transaction.ReferenceId,
                CreatedAt = transaction.CreatedAt
            }
        };
    }
}
