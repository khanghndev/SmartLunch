using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.Reviews;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationReviews;

namespace SmartLunch.Backend.Service.Application.Queries.CustomerReviews.GetPublicReviews;

public sealed class GetPublicReviewsQueryHandler : IRequestHandler<GetPublicReviewsQuery, GetPublicReviewsResponse>
{
    private readonly IReviewRepository _reviews;

    public GetPublicReviewsQueryHandler(IReviewRepository reviews) => _reviews = reviews;

    public async Task<GetPublicReviewsResponse> Handle(GetPublicReviewsQuery request, CancellationToken cancellationToken)
    {
        var (items, total, avg) = await _reviews.GetPublicReviewsAsync(request.Page, request.PageSize, cancellationToken);
        return new GetPublicReviewsResponse
        {
            Reviews = items.Select(ReviewDisplayMapper.ToPublicDto).ToList(),
            TotalCount = total,
            AverageRating = avg,
        };
    }
}
