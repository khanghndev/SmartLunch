using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientIntake;
using SmartLunch.Backend.Service.Application.Helpers;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.Mappings;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.IngredientIntake.CreateIngredientIntakeProposal;

public class CreateIngredientIntakeProposalCommandHandler
    : IRequestHandler<CreateIngredientIntakeProposalCommand, CreateIngredientIntakeProposalResponse>
{
    private readonly IIngredientIntakeProposalRepository _proposalRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<CreateIngredientIntakeProposalCommandHandler> _logger;

    public CreateIngredientIntakeProposalCommandHandler(
        IIngredientIntakeProposalRepository proposalRepository,
        IIngredientRepository ingredientRepository,
        IUserRoleRepository userRoleRepository,
        ILogger<CreateIngredientIntakeProposalCommandHandler> logger)
    {
        _proposalRepository = proposalRepository;
        _ingredientRepository = ingredientRepository;
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    public async Task<CreateIngredientIntakeProposalResponse> Handle(
        CreateIngredientIntakeProposalCommand request,
        CancellationToken cancellationToken)
    {
        var userRoles = await _userRoleRepository.GetActiveByUserIdAsync(request.ActorUserId);
        var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();
        if (!IntakeProposalAccessHelper.CanCreateIntakeProposal(roleNames))
            throw new UnauthorizedAccessException("You are not allowed to create intake proposals.");

        var req = request.Request;
        if (req.Lines == null || req.Lines.Count == 0)
            throw new ArgumentException("At least one line is required.");

        var merged = req.Lines
            .GroupBy(l => l.IngredientId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var proposalId = Guid.NewGuid();
        var lineEntities = new List<IngredientIntakeProposalLine>();
        foreach (var (ingredientId, rows) in merged)
        {
            var qty = rows.Sum(r => r.Quantity);
            if (qty <= 0)
                throw new ArgumentException($"Quantity must be positive for ingredient {ingredientId}.");

            var ing = await _ingredientRepository.GetByIdAsync(ingredientId);
            if (ing == null)
                throw new KeyNotFoundException($"Ingredient {ingredientId} was not found.");
            if (!ing.IsActive)
                throw new InvalidOperationException($"Ingredient '{ing.Name}' is inactive.");

            var combinedNote = string.Join("; ", rows.Select(r => r.LineNote).Where(s => !string.IsNullOrWhiteSpace(s))!);
            var lineNote = string.IsNullOrWhiteSpace(combinedNote) ? null : combinedNote.Trim();
            if (lineNote != null && lineNote.Length > 255)
                lineNote = lineNote[..255];

            lineEntities.Add(new IngredientIntakeProposalLine
            {
                Id = Guid.NewGuid(),
                ProposalId = proposalId,
                IngredientId = ingredientId,
                Quantity = qty,
                LineNote = lineNote
            });
        }

        var headerNote = string.IsNullOrWhiteSpace(req.HeaderNote) ? null : req.HeaderNote.Trim();
        if (headerNote != null && headerNote.Length > 500)
            headerNote = headerNote[..500];

        var proposal = new IngredientIntakeProposal
        {
            Id = proposalId,
            ProposalCode = BuildProposalCode(),
            Status = IntakeProposalStatus.Submitted,
            HeaderNote = headerNote,
            CreatedByUserId = request.ActorUserId,
            CreatedAt = DateTime.UtcNow,
            Lines = lineEntities
        };

        await _proposalRepository.CreateAsync(proposal, cancellationToken);

        var reloaded = await _proposalRepository.GetByIdWithDetailsAsync(proposal.Id, cancellationToken);
        if (reloaded == null)
            throw new InvalidOperationException("Failed to reload proposal after create.");

        _logger.LogInformation("Created intake proposal {Code} by user {UserId}", reloaded.ProposalCode, request.ActorUserId);

        return new CreateIngredientIntakeProposalResponse
        {
            Proposal = IngredientIntakeProposalMapping.ToDetail(reloaded)
        };
    }

    private static string BuildProposalCode()
    {
        return $"DXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
    }
}
