using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Systems;

namespace SmartLunch.Backend.Service.Application.Queries.Systems.GetSystemBackups;

public class GetSystemBackupsQuery : IRequest<GetSystemBackupsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IncludeDeleted { get; set; }

    public GetSystemBackupsQuery(int page = 1, int pageSize = 10, bool includeDeleted = false)
    {
        Page = page;
        PageSize = pageSize;
        IncludeDeleted = includeDeleted;
    }
}

