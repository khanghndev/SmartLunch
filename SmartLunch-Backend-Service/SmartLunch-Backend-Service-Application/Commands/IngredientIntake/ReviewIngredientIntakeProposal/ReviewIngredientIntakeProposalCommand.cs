using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;

namespace SmartLunch.Backend.Service.Application.Commands.IngredientIntake.ReviewIngredientIntakeProposal;

public class ReviewIngredientIntakeProposalCommand : IRequest<ReviewIngredientIntakeProposalResponse>
{
    public int ProposalId { get; }
    public ReviewIngredientIntakeProposalRequest Request { get; }
    public int ReviewerUserId { get; }

    public ReviewIngredientIntakeProposalCommand(
        int proposalId,
        ReviewIngredientIntakeProposalRequest request,
        int reviewerUserId)
    {
        ProposalId = proposalId;
        Request = request;
        ReviewerUserId = reviewerUserId;
    }
}
