using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Inventories;

namespace SmartLunch.Backend.Service.Application.Queries.Inventories.GetInventory;

public class GetInventoryQuery : IRequest<GetInventoryResponse>
{
    public Guid InventoryId { get; set; }

    public GetInventoryQuery(Guid inventoryId)
    {
        InventoryId = inventoryId;
    }
}
