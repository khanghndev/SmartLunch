using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MediaFiles;

namespace SmartLunch.Backend.Service.Application.Queries.MediaFiles.GetMediaFiles;

public class GetMediaFilesQuery : IRequest<GetMediaFilesResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    public GetMediaFilesQuery(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
    }
}
