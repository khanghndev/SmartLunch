using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserOrganizations;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserOrganizations;

namespace SmartLunch.Backend.Service.Application.Commands.UserOrganizations.UpdateUserOrganization;

public class UpdateUserOrganizationCommand : IRequest<GetUserOrganizationResponse>
{
    public UpdateUserOrganizationRequest Request { get; set; }

    public UpdateUserOrganizationCommand(UpdateUserOrganizationRequest request)
    {
        Request = request;
    }
}
