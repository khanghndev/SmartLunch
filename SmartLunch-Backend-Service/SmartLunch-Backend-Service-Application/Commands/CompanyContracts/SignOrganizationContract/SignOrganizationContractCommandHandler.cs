using MediatR;
using SmartLunch.Backend.Service.Application.Commands.MasterData.Contracts.SignContract;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Contracts;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Commands.CompanyContracts.SignOrganizationContract;

public class SignOrganizationContractCommandHandler : IRequestHandler<SignOrganizationContractCommand, GetContractResponse>
{
    private readonly IContractRepository _contractRepository;
    private readonly IUserOrganizationRepository _userOrganizationRepository;
    private readonly IMediator _mediator;

    public SignOrganizationContractCommandHandler(
        IContractRepository contractRepository,
        IUserOrganizationRepository userOrganizationRepository,
        IMediator mediator)
    {
        _contractRepository = contractRepository;
        _userOrganizationRepository = userOrganizationRepository;
        _mediator = mediator;
    }

    public async Task<GetContractResponse> Handle(SignOrganizationContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByIdAsync(request.ContractId);
        if (contract == null)
            throw new KeyNotFoundException($"Contract with ID {request.ContractId} was not found.");

        if (!contract.OrganizationId.HasValue)
            throw new UnauthorizedAccessException("This contract is not available for organization self-service.");

        var membership = await _userOrganizationRepository.GetByUserAndOrganizationAsync(
            request.UserId,
            contract.OrganizationId.Value);
        if (membership == null || !membership.IsActive)
            throw new UnauthorizedAccessException("You do not have access to this contract.");

        return await _mediator.Send(
            new SignContractCommand(request.ContractId, request.UserId, request.Request),
            cancellationToken);
    }
}
