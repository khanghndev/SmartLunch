using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserRoles;

namespace SmartLunch.Backend.Service.Application.Queries.UserRoles.GetUserRole;

public class GetUserRoleQuery : IRequest<GetUserRoleResponse>
{
    public int Id { get; set; }

    public GetUserRoleQuery(int id)
    {
        Id = id;
    }
}
