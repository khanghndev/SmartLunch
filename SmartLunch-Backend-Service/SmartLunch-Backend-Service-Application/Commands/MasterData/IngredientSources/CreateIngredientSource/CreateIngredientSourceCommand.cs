using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.IngredientSources;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.IngredientSources;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.IngredientSources.CreateIngredientSource;

public class CreateIngredientSourceCommand : IRequest<GetIngredientSourceResponse>
{
    public CreateIngredientSourceRequest Request { get; }

    public CreateIngredientSourceCommand(CreateIngredientSourceRequest request)
    {
        Request = request;
    }
}
