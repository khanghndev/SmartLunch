using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetInternalStockIssue;

public class GetInternalStockIssueQuery : IRequest<InternalStockIssueDetailDto?>
{
    public int IssueId { get; }

    public GetInternalStockIssueQuery(int issueId)
    {
        IssueId = issueId;
    }
}
