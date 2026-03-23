using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Mappings;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientIntake.GetIngredientIntakeProposal;

public class GetIngredientIntakeProposalQueryHandler
    : IRequestHandler<GetIngredientIntakeProposalQuery, IngredientIntakeProposalDetailDto?>
{
    private readonly IIngredientIntakeProposalRepository _repository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<GetIngredientIntakeProposalQueryHandler> _logger;

    public GetIngredientIntakeProposalQueryHandler(
        IIngredientIntakeProposalRepository repository,
        IUserRoleRepository userRoleRepository,
        ILogger<GetIngredientIntakeProposalQueryHandler> logger)
    {
        _repository = repository;
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    public async Task<IngredientIntakeProposalDetailDto?> Handle(
        GetIngredientIntakeProposalQuery request,
        CancellationToken cancellationToken)
    {
        var userRoles = await _userRoleRepository.GetActiveByUserIdAsync(request.ActorUserId);
        var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();
        if (!IntakeProposalAccessHelper.CanCreateIntakeProposal(roleNames))
            throw new UnauthorizedAccessException("You are not allowed to view intake proposals.");

        var entity = await _repository.GetByIdWithDetailsAsync(request.ProposalId, cancellationToken);
        if (entity == null)
            return null;

        var seeAll = IntakeProposalAccessHelper.IsElevatedReviewer(roleNames);
        if (!seeAll && entity.CreatedByUserId != request.ActorUserId)
        {
            _logger.LogWarning(
                "User {UserId} denied access to proposal {ProposalId} (not owner)",
                request.ActorUserId,
                request.ProposalId);
            return null;
        }

        return IngredientIntakeProposalMapping.ToDetail(entity);
    }
}
