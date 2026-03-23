using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Mappings;

namespace SmartLunch.Backend.Service.Application.Commands.IngredientIntake.ReviewIngredientIntakeProposal;

public class ReviewIngredientIntakeProposalCommandHandler
    : IRequestHandler<ReviewIngredientIntakeProposalCommand, ReviewIngredientIntakeProposalResponse>
{
    private readonly IIngredientIntakeProposalRepository _proposalRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<ReviewIngredientIntakeProposalCommandHandler> _logger;

    public ReviewIngredientIntakeProposalCommandHandler(
        IIngredientIntakeProposalRepository proposalRepository,
        IUserRoleRepository userRoleRepository,
        ILogger<ReviewIngredientIntakeProposalCommandHandler> logger)
    {
        _proposalRepository = proposalRepository;
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    public async Task<ReviewIngredientIntakeProposalResponse> Handle(
        ReviewIngredientIntakeProposalCommand request,
        CancellationToken cancellationToken)
    {
        var userRoles = await _userRoleRepository.GetActiveByUserIdAsync(request.ReviewerUserId);
        var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();
        if (!IntakeProposalAccessHelper.IsElevatedReviewer(roleNames))
            throw new UnauthorizedAccessException("Only administrators can review intake proposals.");

        await _proposalRepository.ReviewProposalAsync(
            request.ProposalId,
            request.ReviewerUserId,
            request.Request.Approve,
            request.Request.ReviewNote,
            cancellationToken);

        var reloaded = await _proposalRepository.GetByIdWithDetailsAsync(request.ProposalId, cancellationToken);
        if (reloaded == null)
            throw new InvalidOperationException("Failed to reload proposal after review.");

        _logger.LogInformation(
            "Proposal {ProposalId} reviewed: approve={Approve}",
            request.ProposalId,
            request.Request.Approve);

        return new ReviewIngredientIntakeProposalResponse
        {
            Proposal = IngredientIntakeProposalMapping.ToDetail(reloaded)
        };
    }
}
