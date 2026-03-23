using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Mappings;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientIntake.GetIntakeProposalReviewHistory;

public class GetIntakeProposalReviewHistoryQueryHandler
    : IRequestHandler<GetIntakeProposalReviewHistoryQuery, GetIntakeProposalReviewHistoryResponse>
{
    private readonly IIngredientIntakeProposalRepository _repository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<GetIntakeProposalReviewHistoryQueryHandler> _logger;

    public GetIntakeProposalReviewHistoryQueryHandler(
        IIngredientIntakeProposalRepository repository,
        IUserRoleRepository userRoleRepository,
        ILogger<GetIntakeProposalReviewHistoryQueryHandler> logger)
    {
        _repository = repository;
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    public async Task<GetIntakeProposalReviewHistoryResponse> Handle(
        GetIntakeProposalReviewHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (req.Page < 1 || req.PageSize < 1 || req.PageSize > 200)
            throw new ArgumentException("Invalid pagination.");

        var userRoles = await _userRoleRepository.GetActiveByUserIdAsync(request.ActorUserId);
        var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();
        if (!IntakeProposalAccessHelper.CanCreateIntakeProposal(roleNames))
            throw new UnauthorizedAccessException("You are not allowed to view intake proposal review history.");

        var seeAll = IntakeProposalAccessHelper.IsElevatedReviewer(roleNames);
        Guid? filter = seeAll ? null : request.ActorUserId;

        var (items, total) = await _repository.GetReviewHistoryPagedAsync(
            req.Page,
            req.PageSize,
            filter,
            cancellationToken);

        _logger.LogInformation(
            "Review history for user {UserId}, seeAll={SeeAll}, rows={Count}",
            request.ActorUserId,
            seeAll,
            items.Count);

        return new GetIntakeProposalReviewHistoryResponse
        {
            Data = items.Select(IngredientIntakeProposalMapping.ToReviewHistoryEntry).ToList(),
            TotalCount = total,
            Page = req.Page,
            PageSize = req.PageSize
        };
    }
}
