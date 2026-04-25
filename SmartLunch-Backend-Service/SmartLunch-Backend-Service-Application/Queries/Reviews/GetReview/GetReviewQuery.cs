using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Reviews;

namespace SmartLunch.Backend.Service.Application.Queries.Reviews.GetReview;

public class GetReviewQuery : IRequest<GetReviewResponse>
{
    public int ReviewId { get; set; }

    public GetReviewQuery(int reviewId)
    {
        ReviewId = reviewId;
    }
}
