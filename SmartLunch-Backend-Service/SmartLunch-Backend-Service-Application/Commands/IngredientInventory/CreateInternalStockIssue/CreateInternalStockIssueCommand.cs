using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientInventory;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;

namespace SmartLunch.Backend.Service.Application.Commands.IngredientInventory.CreateInternalStockIssue;

public class CreateInternalStockIssueCommand : IRequest<CreateInternalStockIssueResponse>
{
    public CreateInternalStockIssueRequest Request { get; }
    public int? CreatedByUserId { get; }

    public CreateInternalStockIssueCommand(CreateInternalStockIssueRequest request, int? createdByUserId)
    {
        Request = request;
        CreatedByUserId = createdByUserId;
    }
}
