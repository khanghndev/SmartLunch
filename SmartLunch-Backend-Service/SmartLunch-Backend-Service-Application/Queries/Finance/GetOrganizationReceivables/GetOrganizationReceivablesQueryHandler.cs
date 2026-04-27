using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetOrganizationReceivables;

public class GetOrganizationReceivablesQueryHandler : IRequestHandler<GetOrganizationReceivablesQuery, GetOrganizationReceivablesResponse>
{
    private readonly IDebtAnalyticsRepository _debtRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ILogger<GetOrganizationReceivablesQueryHandler> _logger;

    public GetOrganizationReceivablesQueryHandler(
        IDebtAnalyticsRepository debtRepository,
        IOrganizationRepository organizationRepository,
        ILogger<GetOrganizationReceivablesQueryHandler> logger)
    {
        _debtRepository = debtRepository;
        _organizationRepository = organizationRepository;
        _logger = logger;
    }

    public async Task<GetOrganizationReceivablesResponse> Handle(GetOrganizationReceivablesQuery request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var aggregates = await _debtRepository.GetOrganizationBilledAndPaidAsync(req.OrganizationId, cancellationToken);
        var names = await _organizationRepository.GetNamesByIdsAsync(aggregates.Keys, cancellationToken);

        var lines = new List<OrganizationReceivableLineDto>();
        foreach (var (organizationId, row) in aggregates)
        {
            var outstanding = row.Billed - row.Paid;
            if (req.OnlyWithOutstanding && outstanding <= 0.01m)
                continue;

            names.TryGetValue(organizationId, out var organizationName);
            lines.Add(new OrganizationReceivableLineDto
            {
                OrganizationId = organizationId,
                OrganizationName = organizationName ?? string.Empty,
                OrderCount = row.OrderCount,
                TotalBilled = row.Billed,
                TotalPaid = row.Paid,
                Outstanding = outstanding
            });
        }

        lines = lines.OrderByDescending(l => l.Outstanding).ToList();
        var grand = lines.Sum(l => l.Outstanding);

        _logger.LogInformation("Organization receivables: {Count} lines, grand outstanding {Grand}", lines.Count, grand);

        return new GetOrganizationReceivablesResponse
        {
            Lines = lines,
            GrandTotalOutstanding = grand
        };
    }
}
