using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Reviews;

namespace SmartLunch.Backend.Service.Application.Queries.Reviews.GetReviews;

public class GetReviewsQuery : IRequest<GetReviewsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    public GetReviewsQuery(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
