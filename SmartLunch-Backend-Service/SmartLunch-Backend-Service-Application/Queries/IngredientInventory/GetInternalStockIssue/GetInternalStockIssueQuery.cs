using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetInternalStockIssue;

public class GetInternalStockIssueQuery : IRequest<InternalStockIssueDetailDto?>
{
    public Guid IssueId { get; }

    public GetInternalStockIssueQuery(Guid issueId)
    {
        IssueId = issueId;
    }
}
