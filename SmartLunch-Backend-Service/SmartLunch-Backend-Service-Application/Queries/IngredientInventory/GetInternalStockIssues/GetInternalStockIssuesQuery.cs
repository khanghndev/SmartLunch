using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientInventory;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetInternalStockIssues;

public class GetInternalStockIssuesQuery : IRequest<GetInternalStockIssuesResponse>
{
    public GetInternalStockIssuesRequest Request { get; }

    public GetInternalStockIssuesQuery(GetInternalStockIssuesRequest request)
    {
        Request = request;
    }
}
