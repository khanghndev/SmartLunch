using MediatR;

namespace SmartLunch.Backend.Service.Application.Queries.Auth;

public class GetUserRolesQuery : IRequest<List<string>>
{
    public int UserId { get; set; }

    public GetUserRolesQuery(int userId)
    {
        UserId = userId;
    }
}
