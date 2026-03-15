using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;

namespace SmartLunch.Backend.Service.Application.Queries.UserRoles.GetUserRole;

public class GetUserRoleQuery : IRequest<GetUserRoleResponse>
{
    public Guid Id { get; set; }

    public GetUserRoleQuery(Guid id)
    {
        Id = id;
    }
}
