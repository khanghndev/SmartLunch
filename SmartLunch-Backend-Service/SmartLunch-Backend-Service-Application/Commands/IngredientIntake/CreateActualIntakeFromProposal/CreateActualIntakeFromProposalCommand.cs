using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;

namespace SmartLunch.Backend.Service.Application.Commands.IngredientIntake.CreateActualIntakeFromProposal;

public class CreateActualIntakeFromProposalCommand : IRequest<CreateActualIntakeFromProposalResponse>
{
    public int ProposalId { get; }
    public CreateActualIntakeFromProposalRequest Request { get; }
    public int ActorUserId { get; }

    public CreateActualIntakeFromProposalCommand(
        int proposalId,
        CreateActualIntakeFromProposalRequest request,
        int actorUserId)
    {
        ProposalId = proposalId;
        Request = request;
        ActorUserId = actorUserId;
    }
}
