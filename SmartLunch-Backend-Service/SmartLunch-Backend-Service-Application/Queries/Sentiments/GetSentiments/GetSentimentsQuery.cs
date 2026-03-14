using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Sentiments;

namespace SmartLunch.Backend.Service.Application.Queries.Sentiments.GetSentiments;

public class GetSentimentsQuery : IRequest<GetSentimentsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    public GetSentimentsQuery(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
