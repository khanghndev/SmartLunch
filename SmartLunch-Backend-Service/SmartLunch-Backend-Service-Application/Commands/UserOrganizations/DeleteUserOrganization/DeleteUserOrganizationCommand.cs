using MediatR;

namespace SmartLunch.Backend.Service.Application.Commands.UserOrganizations.DeleteUserOrganization;

public class DeleteUserOrganizationCommand : IRequest<bool>
{
    public int Id { get; set; }

    public DeleteUserOrganizationCommand(int id)
    {
        Id = id;
    }
}
