using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserOrganizations;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserOrganizations;

namespace SmartLunch.Backend.Service.Application.Commands.UserOrganizations.CreateUserOrganization;

public class CreateUserOrganizationCommand : IRequest<CreateUserOrganizationResponse>
{
    public CreateUserOrganizationRequest Request { get; set; }

    public CreateUserOrganizationCommand(CreateUserOrganizationRequest request)
    {
        Request = request;
    }
}
