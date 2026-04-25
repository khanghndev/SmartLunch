using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Commands.IngredientInventory.CreateInternalStockIssue;

public class CreateInternalStockIssueCommandHandler
    : IRequestHandler<CreateInternalStockIssueCommand, CreateInternalStockIssueResponse>
{
    private readonly IInternalStockIssueRepository _repository;
    private readonly ILogger<CreateInternalStockIssueCommandHandler> _logger;

    public CreateInternalStockIssueCommandHandler(
        IInternalStockIssueRepository repository,
        ILogger<CreateInternalStockIssueCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CreateInternalStockIssueResponse> Handle(
        CreateInternalStockIssueCommand request,
        CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (req.Lines == null || req.Lines.Count == 0)
            throw new ArgumentException("At least one line is required.");

        var merged = req.Lines
            .GroupBy(l => l.IngredientId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        foreach (var (ingredientId, qty) in merged)
        {
            if (qty <= 0)
                throw new ArgumentException($"Quantity must be positive for ingredient {ingredientId}.");
        }

        var reason = string.IsNullOrWhiteSpace(req.Reason) ? null : req.Reason.Trim();
        if (reason != null && reason.Length > 500)
            reason = reason[..500];

        var issuedAt = req.IssuedAtUtc ?? DateTime.UtcNow;
        if (issuedAt.Kind == DateTimeKind.Unspecified)
            issuedAt = DateTime.SpecifyKind(issuedAt, DateTimeKind.Utc);

        var issue = new InternalStockIssue
        {

            IssueCode = BuildIssueCode(),
            IssuedAt = issuedAt,
            Reason = reason,
            CreatedByUserId = request.CreatedByUserId,
            CreatedAt = DateTime.UtcNow
        };

        var lineTuples = merged.Select(kv => (kv.Key, kv.Value)).ToList();
        var saved = await _repository.CreateAndDeductStockAsync(issue, lineTuples, cancellationToken);

        _logger.LogInformation("Created internal stock issue {IssueCode} with {LineCount} lines",
            saved.IssueCode, saved.Lines?.Count ?? 0);

        var detail = MapDetail(saved);
        return new CreateInternalStockIssueResponse { Issue = detail };
    }

    private static string BuildIssueCode()
    {
        return $"PXK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
    }

    private static InternalStockIssueDetailDto MapDetail(InternalStockIssue x)
    {
        return new InternalStockIssueDetailDto
        {
            Id = x.Id,
            IssueCode = x.IssueCode,
            IssuedAt = x.IssuedAt,
            Reason = x.Reason,
            CreatedByUserId = x.CreatedByUserId,
            CreatedByDisplayName = FormatUser(x.CreatedByUser),
            CreatedAt = x.CreatedAt,
            Lines = (x.Lines ?? Array.Empty<InternalStockIssueLine>())
                .OrderBy(l => l.Ingredient.Name)
                .Select(l => new InternalStockIssueLineDto
                {
                    Id = l.Id,
                    IngredientId = l.IngredientId,
                    IngredientName = l.Ingredient.Name,
                    Unit = l.Ingredient.Unit,
                    Quantity = l.Quantity
                })
                .ToList()
        };
    }

    private static string? FormatUser(User? u)
    {
        if (u == null)
            return null;
        var parts = new[] { u.FirstName, u.LastName }.Where(s => !string.IsNullOrWhiteSpace(s));
        var name = string.Join(' ', parts);
        return string.IsNullOrWhiteSpace(name) ? u.Username : name;
    }
}
