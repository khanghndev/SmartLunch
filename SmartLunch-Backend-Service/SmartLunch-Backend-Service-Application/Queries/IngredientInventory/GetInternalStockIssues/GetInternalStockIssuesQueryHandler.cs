using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.IngredientInventory;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Queries.IngredientInventory.GetInternalStockIssues;

public class GetInternalStockIssuesQueryHandler
    : IRequestHandler<GetInternalStockIssuesQuery, GetInternalStockIssuesResponse>
{
    private readonly IInternalStockIssueRepository _repository;
    private readonly ILogger<GetInternalStockIssuesQueryHandler> _logger;

    public GetInternalStockIssuesQueryHandler(
        IInternalStockIssueRepository repository,
        ILogger<GetInternalStockIssuesQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<GetInternalStockIssuesResponse> Handle(
        GetInternalStockIssuesQuery request,
        CancellationToken cancellationToken)
    {
        var req = request.Request;
        if (req.Page < 1 || req.PageSize < 1 || req.PageSize > 200)
            throw new ArgumentException("Invalid pagination.");

        DateTime? fromUtc = null;
        DateTime? toExUtc = null;
        if (req.IssuedFrom.HasValue)
            fromUtc = VietnamTime.CalendarDateMidnight(req.IssuedFrom.Value);
        if (req.IssuedTo.HasValue)
            toExUtc = VietnamTime.CalendarDateMidnight(req.IssuedTo.Value.AddDays(1));

        if (fromUtc.HasValue && toExUtc.HasValue && toExUtc.Value <= fromUtc.Value)
            throw new ArgumentException("'IssuedTo' must be on or after 'IssuedFrom'.");

        var (items, total) = await _repository.GetPagedAsync(
            req.Page,
            req.PageSize,
            fromUtc,
            toExUtc,
            cancellationToken);

        var data = items.Select(MapSummary).ToList();

        _logger.LogInformation("Listed internal stock issues: page {Page}, total {Total}", req.Page, total);

        return new GetInternalStockIssuesResponse
        {
            Data = data,
            TotalCount = total,
            Page = req.Page,
            PageSize = req.PageSize
        };
    }

    private static InternalStockIssueSummaryDto MapSummary(InternalStockIssue x)
    {
        return new InternalStockIssueSummaryDto
        {
            Id = x.Id,
            IssueCode = x.IssueCode,
            IssuedAt = x.IssuedAt,
            Reason = x.Reason,
            CreatedByUserId = x.CreatedByUserId,
            CreatedByDisplayName = FormatUser(x.CreatedByUser),
            CreatedAt = x.CreatedAt,
            LineCount = x.Lines?.Count ?? 0
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
