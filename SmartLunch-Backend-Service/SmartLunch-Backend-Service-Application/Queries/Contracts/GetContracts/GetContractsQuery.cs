using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;

namespace SmartLunch.Backend.Service.Application.Queries.Contracts.GetContracts;

public class GetContractsQuery : IRequest<GetContractsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    public GetContractsQuery(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
