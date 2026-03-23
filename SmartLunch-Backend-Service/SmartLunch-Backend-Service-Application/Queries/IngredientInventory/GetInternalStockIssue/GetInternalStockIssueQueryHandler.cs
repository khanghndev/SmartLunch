using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetInternalStockIssue;

public class GetInternalStockIssueQueryHandler
    : IRequestHandler<GetInternalStockIssueQuery, InternalStockIssueDetailDto?>
{
    private readonly IInternalStockIssueRepository _repository;
    private readonly ILogger<GetInternalStockIssueQueryHandler> _logger;

    public GetInternalStockIssueQueryHandler(
        IInternalStockIssueRepository repository,
        ILogger<GetInternalStockIssueQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<InternalStockIssueDetailDto?> Handle(
        GetInternalStockIssueQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdWithLinesAsync(request.IssueId, cancellationToken);
        if (entity == null)
            return null;

        _logger.LogInformation("Loaded internal stock issue {IssueId}", request.IssueId);
        return MapDetail(entity);
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
