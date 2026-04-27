using MediatR;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.UserOrganizations.DeleteUserOrganization;

public class DeleteUserOrganizationCommandHandler : IRequestHandler<DeleteUserOrganizationCommand, bool>
{
    private readonly IUserOrganizationRepository _userOrganizationRepository;

    public DeleteUserOrganizationCommandHandler(IUserOrganizationRepository userOrganizationRepository)
    {
        _userOrganizationRepository = userOrganizationRepository;
    }

    public async Task<bool> Handle(DeleteUserOrganizationCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _userOrganizationRepository.DeleteAsync(request.Id);
        if (!deleted)
            throw new KeyNotFoundException($"UserOrganization with ID {request.Id} not found");
        return true;
    }
}
