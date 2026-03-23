using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Mappings;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientIntake.GetIngredientIntakeProposals;

public class GetIngredientIntakeProposalsQueryHandler
    : IRequestHandler<GetIngredientIntakeProposalsQuery, GetIngredientIntakeProposalsResponse>
{
    private readonly IIngredientIntakeProposalRepository _repository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<GetIngredientIntakeProposalsQueryHandler> _logger;

    public GetIngredientIntakeProposalsQueryHandler(
        IIngredientIntakeProposalRepository repository,
        IUserRoleRepository userRoleRepository,
        ILogger<GetIngredientIntakeProposalsQueryHandler> logger)
    {
        _repository = repository;
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    public async Task<GetIngredientIntakeProposalsResponse> Handle(
        GetIngredientIntakeProposalsQuery request,
        CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (req.Page < 1 || req.PageSize < 1 || req.PageSize > 200)
            throw new ArgumentException("Invalid pagination.");

        var userRoles = await _userRoleRepository.GetActiveByUserIdAsync(request.ActorUserId);
        var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();
        if (!IntakeProposalAccessHelper.CanCreateIntakeProposal(roleNames))
            throw new UnauthorizedAccessException("You are not allowed to view intake proposals.");

        var seeAll = IntakeProposalAccessHelper.IsElevatedReviewer(roleNames);
        Guid? filter = seeAll ? null : request.ActorUserId;

        var (items, total) = await _repository.GetPagedAsync(req.Page, req.PageSize, filter, cancellationToken);

        _logger.LogInformation(
            "Listed intake proposals for user {UserId}, seeAll={SeeAll}, count={Count}",
            request.ActorUserId,
            seeAll,
            items.Count);

        return new GetIngredientIntakeProposalsResponse
        {
            Data = items.Select(IngredientIntakeProposalMapping.ToSummary).ToList(),
            TotalCount = total,
            Page = req.Page,
            PageSize = req.PageSize
        };
    }
}
