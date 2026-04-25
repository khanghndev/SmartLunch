using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.IngredientIntake;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientIntake.GetIntakeProposalReviewHistory;

public class GetIntakeProposalReviewHistoryQuery : IRequest<GetIntakeProposalReviewHistoryResponse>
{
    public GetIntakeProposalReviewHistoryRequest Request { get; }
    public int ActorUserId { get; }

    public GetIntakeProposalReviewHistoryQuery(GetIntakeProposalReviewHistoryRequest request, int actorUserId)
    {
        Request = request;
        ActorUserId = actorUserId;
    }
}
