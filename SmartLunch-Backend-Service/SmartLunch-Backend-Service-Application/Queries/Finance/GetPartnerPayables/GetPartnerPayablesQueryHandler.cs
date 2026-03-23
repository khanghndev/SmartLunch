using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetPartnerPayables;

public class GetPartnerPayablesQueryHandler : IRequestHandler<GetPartnerPayablesQuery, GetPartnerPayablesResponse>
{
    private readonly IDebtAnalyticsRepository _debtRepository;
    private readonly IPartnerRepository _partnerRepository;
    private readonly ILogger<GetPartnerPayablesQueryHandler> _logger;

    public GetPartnerPayablesQueryHandler(
        IDebtAnalyticsRepository debtRepository,
        IPartnerRepository partnerRepository,
        ILogger<GetPartnerPayablesQueryHandler> logger)
    {
        _debtRepository = debtRepository;
        _partnerRepository = partnerRepository;
        _logger = logger;
    }

    public async Task<GetPartnerPayablesResponse> Handle(GetPartnerPayablesQuery request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var aggregates = await _debtRepository.GetPartnerContractAndPaidAsync(req.PartnerId, cancellationToken);
        var names = await _partnerRepository.GetLegalNamesByIdsAsync(aggregates.Keys, cancellationToken);

        var lines = new List<PartnerPayableLineDto>();
        foreach (var (partnerId, row) in aggregates)
        {
            var outstanding = row.ContractValue - row.PaidOut;
            if (req.OnlyWithOutstanding && outstanding <= 0.01m)
                continue;

            names.TryGetValue(partnerId, out var legalName);
            lines.Add(new PartnerPayableLineDto
            {
                PartnerId = partnerId,
                PartnerLegalName = legalName ?? string.Empty,
                TotalContractValue = row.ContractValue,
                TotalPaid = row.PaidOut,
                Outstanding = outstanding
            });
        }

        lines = lines.OrderByDescending(l => l.Outstanding).ToList();
        var grand = lines.Sum(l => l.Outstanding);

        _logger.LogInformation("Partner payables: {Count} lines, grand outstanding {Grand}", lines.Count, grand);

        return new GetPartnerPayablesResponse
        {
            Lines = lines,
            GrandTotalOutstanding = grand
        };
    }
}
