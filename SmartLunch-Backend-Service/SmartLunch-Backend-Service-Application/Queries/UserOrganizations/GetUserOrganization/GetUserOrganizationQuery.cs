using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserOrganizations;

namespace SmartLunch.Backend.Service.Application.Queries.UserOrganizations.GetUserOrganization;

public class GetUserOrganizationQuery : IRequest<GetUserOrganizationResponse>
{
    public int Id { get; set; }

    public GetUserOrganizationQuery(int id)
    {
        Id = id;
    }
}
