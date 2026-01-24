using MediatR;

namespace SmartLunch.Backend.Service.Application.Queries.Auth;

public class GetUserRolesQuery : IRequest<List<string>>
{
    public Guid UserId { get; set; }

    public GetUserRolesQuery(Guid userId)
    {
        UserId = userId;
    }
}
