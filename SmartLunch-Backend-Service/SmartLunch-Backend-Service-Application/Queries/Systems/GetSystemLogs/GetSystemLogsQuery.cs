using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

namespace SmartLunch.Backend.Service.Application.Queries.Systems.GetSystemLogs;

public class GetSystemLogsQuery : IRequest<GetSystemLogsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetSystemLogsQuery(int page = 1, int pageSize = 10)
    {
        Page = page;
        PageSize = pageSize;
    }
}

