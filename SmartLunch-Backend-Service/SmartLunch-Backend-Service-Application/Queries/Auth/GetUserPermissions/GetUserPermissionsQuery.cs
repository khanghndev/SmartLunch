using MediatR;

namespace SmartLunch.Backend.Service.Application.Queries.Auth;

public class GetUserPermissionsQuery : IRequest<List<string>>
{
    public int UserId { get; set; }

    public GetUserPermissionsQuery(int userId)
    {
        UserId = userId;
    }
}
