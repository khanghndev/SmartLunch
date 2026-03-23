using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetUnitReceivables;

public class GetUnitReceivablesQueryHandler : IRequestHandler<GetUnitReceivablesQuery, GetUnitReceivablesResponse>
{
    private readonly IDebtAnalyticsRepository _debtRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly ILogger<GetUnitReceivablesQueryHandler> _logger;

    public GetUnitReceivablesQueryHandler(
        IDebtAnalyticsRepository debtRepository,
        IUnitRepository unitRepository,
        ILogger<GetUnitReceivablesQueryHandler> logger)
    {
        _debtRepository = debtRepository;
        _unitRepository = unitRepository;
        _logger = logger;
    }

    public async Task<GetUnitReceivablesResponse> Handle(GetUnitReceivablesQuery request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var aggregates = await _debtRepository.GetUnitBilledAndPaidAsync(req.UnitId, cancellationToken);
        var names = await _unitRepository.GetNamesByIdsAsync(aggregates.Keys, cancellationToken);

        var lines = new List<UnitReceivableLineDto>();
        foreach (var (unitId, row) in aggregates)
        {
            var outstanding = row.Billed - row.Paid;
            if (req.OnlyWithOutstanding && outstanding <= 0.01m)
                continue;

            names.TryGetValue(unitId, out var unitName);
            lines.Add(new UnitReceivableLineDto
            {
                UnitId = unitId,
                UnitName = unitName ?? string.Empty,
                OrderCount = row.OrderCount,
                TotalBilled = row.Billed,
                TotalPaid = row.Paid,
                Outstanding = outstanding
            });
        }

        lines = lines.OrderByDescending(l => l.Outstanding).ToList();
        var grand = lines.Sum(l => l.Outstanding);

        _logger.LogInformation("Unit receivables: {Count} lines, grand outstanding {Grand}", lines.Count, grand);

        return new GetUnitReceivablesResponse
        {
            Lines = lines,
            GrandTotalOutstanding = grand
        };
    }
}
