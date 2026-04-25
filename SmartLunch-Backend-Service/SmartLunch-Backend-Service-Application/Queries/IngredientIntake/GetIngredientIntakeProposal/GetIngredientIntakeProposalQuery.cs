using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientIntake.GetIngredientIntakeProposal;

public class GetIngredientIntakeProposalQuery : IRequest<IngredientIntakeProposalDetailDto?>
{
    public int ProposalId { get; }
    public int ActorUserId { get; }

    public GetIngredientIntakeProposalQuery(int proposalId, int actorUserId)
    {
        ProposalId = proposalId;
        ActorUserId = actorUserId;
    }
}
