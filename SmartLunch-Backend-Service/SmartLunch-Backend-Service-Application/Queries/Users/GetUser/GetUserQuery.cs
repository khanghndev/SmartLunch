using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Users;

namespace SmartLunch.Backend.Service.Application.Queries.Users;

/// <summary>
/// Query to get a user by ID
/// </summary>
public class GetUserQuery : IRequest<GetUserResponse>
{
    public int UserId { get; set; }

    public GetUserQuery(int userId)
    {
        UserId = userId;
    }
}
