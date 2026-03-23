using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;

namespace SmartLunch.Backend.Service.Application.Commands.IngredientIntake.CreateActualIntakeFromProposal;

public class CreateActualIntakeFromProposalCommand : IRequest<CreateActualIntakeFromProposalResponse>
{
    public Guid ProposalId { get; }
    public CreateActualIntakeFromProposalRequest Request { get; }
    public Guid ActorUserId { get; }

    public CreateActualIntakeFromProposalCommand(
        Guid proposalId,
        CreateActualIntakeFromProposalRequest request,
        Guid actorUserId)
    {
        ProposalId = proposalId;
        Request = request;
        ActorUserId = actorUserId;
    }
}
