using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Reviews;

namespace SmartLunch.Backend.Service.Application.Queries.Reviews.GetReview;

public class GetReviewQuery : IRequest<GetReviewResponse>
{
    public Guid ReviewId { get; set; }

    public GetReviewQuery(Guid reviewId)
    {
        ReviewId = reviewId;
    }
}
