using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Users;

namespace SmartLunch.Backend.Service.Application.Queries.Users.GetUsers;

/// <summary>
/// Query to get list of users with pagination
/// </summary>
public class GetUsersQuery : IRequest<GetUsersResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }

    public GetUsersQuery(int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchTerm = searchTerm;
        IsActive = isActive;
    }
}
