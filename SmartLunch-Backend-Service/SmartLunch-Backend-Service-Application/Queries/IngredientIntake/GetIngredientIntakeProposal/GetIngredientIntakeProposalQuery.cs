using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientIntake.GetIngredientIntakeProposal;

public class GetIngredientIntakeProposalQuery : IRequest<IngredientIntakeProposalDetailDto?>
{
    public Guid ProposalId { get; }
    public Guid ActorUserId { get; }

    public GetIngredientIntakeProposalQuery(Guid proposalId, Guid actorUserId)
    {
        ProposalId = proposalId;
        ActorUserId = actorUserId;
    }
}
