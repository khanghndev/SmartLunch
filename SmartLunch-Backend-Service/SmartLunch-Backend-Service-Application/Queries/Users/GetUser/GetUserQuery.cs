using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Users;

namespace SmartLunch.Backend.Service.Application.Queries.Users;

/// <summary>
/// Query to get a user by ID
/// </summary>
public class GetUserQuery : IRequest<UserDto?>
{
    public Guid UserId { get; set; }

    public GetUserQuery(Guid userId)
    {
        UserId = userId;
    }
}
