using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;

namespace SmartLunch.Backend.Service.Application.Commands.IngredientIntake.ReviewIngredientIntakeProposal;

public class ReviewIngredientIntakeProposalCommand : IRequest<ReviewIngredientIntakeProposalResponse>
{
    public Guid ProposalId { get; }
    public ReviewIngredientIntakeProposalRequest Request { get; }
    public Guid ReviewerUserId { get; }

    public ReviewIngredientIntakeProposalCommand(
        Guid proposalId,
        ReviewIngredientIntakeProposalRequest request,
        Guid reviewerUserId)
    {
        ProposalId = proposalId;
        Request = request;
        ReviewerUserId = reviewerUserId;
    }
}
