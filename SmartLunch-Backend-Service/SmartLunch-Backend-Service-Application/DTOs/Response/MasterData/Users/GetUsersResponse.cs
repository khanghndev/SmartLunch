using SmartLunch.Backend.Service.Application.DTOs.Users;

namespace SmartLunch.Backend.Service.Application.DTOs.Users;

public class GetUsersResponse
{
    public List<UserDto> Users { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
