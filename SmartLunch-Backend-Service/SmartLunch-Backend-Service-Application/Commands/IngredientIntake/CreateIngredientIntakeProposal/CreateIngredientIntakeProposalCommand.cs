using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;

namespace SmartLunch.Backend.Service.Application.Commands.IngredientIntake.CreateIngredientIntakeProposal;

public class CreateIngredientIntakeProposalCommand : IRequest<CreateIngredientIntakeProposalResponse>
{
    public CreateIngredientIntakeProposalRequest Request { get; }
    public Guid ActorUserId { get; }

    public CreateIngredientIntakeProposalCommand(CreateIngredientIntakeProposalRequest request, Guid actorUserId)
    {
        Request = request;
        ActorUserId = actorUserId;
    }
}
