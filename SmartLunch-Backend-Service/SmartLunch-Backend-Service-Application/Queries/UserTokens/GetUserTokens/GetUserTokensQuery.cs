using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserTokens;

namespace SmartLunch.Backend.Service.Application.Queries.UserTokens.GetUserTokens;

public class GetUserTokensQuery : IRequest<GetUserTokensResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }

    public GetUserTokensQuery(int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        IsActive = isActive;
    }
}
