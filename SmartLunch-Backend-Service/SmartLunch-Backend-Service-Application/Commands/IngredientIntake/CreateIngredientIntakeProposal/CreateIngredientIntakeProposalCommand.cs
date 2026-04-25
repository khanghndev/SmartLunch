using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;

namespace SmartLunch.Backend.Service.Application.Commands.IngredientIntake.CreateIngredientIntakeProposal;

public class CreateIngredientIntakeProposalCommand : IRequest<CreateIngredientIntakeProposalResponse>
{
    public CreateIngredientIntakeProposalRequest Request { get; }
    public int ActorUserId { get; }

    public CreateIngredientIntakeProposalCommand(CreateIngredientIntakeProposalRequest request, int actorUserId)
    {
        Request = request;
        ActorUserId = actorUserId;
    }
}
