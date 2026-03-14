using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Transactions;

namespace SmartLunch.Backend.Service.Application.Queries.Transactions.GetTransactions;

public class GetTransactionsQuery : IRequest<GetTransactionsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    public GetTransactionsQuery(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
