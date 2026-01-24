using MediatR;

namespace SmartLunch.Backend.Service.Application.Queries.Auth;

public class GetUserPermissionsQuery : IRequest<List<string>>
{
    public Guid UserId { get; set; }

    public GetUserPermissionsQuery(Guid userId)
    {
        UserId = userId;
    }
}
