using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientIntake.GetIngredientIntakeProposals;

public class GetIngredientIntakeProposalsQuery : IRequest<GetIngredientIntakeProposalsResponse>
{
    public GetIngredientIntakeProposalsRequest Request { get; }
    public int ActorUserId { get; }

    public GetIngredientIntakeProposalsQuery(GetIngredientIntakeProposalsRequest request, int actorUserId)
    {
        Request = request;
        ActorUserId = actorUserId;
    }
}
