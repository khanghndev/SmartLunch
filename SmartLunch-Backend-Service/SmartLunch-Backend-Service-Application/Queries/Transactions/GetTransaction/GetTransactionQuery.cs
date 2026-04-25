using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Transactions;

namespace SmartLunch.Backend.Service.Application.Queries.Transactions.GetTransaction;

public class GetTransactionQuery : IRequest<GetTransactionResponse>
{
    public int TransactionId { get; set; }

    public GetTransactionQuery(int transactionId)
    {
        TransactionId = transactionId;
    }
}
