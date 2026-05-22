using MediatR;
using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetSupplierContractsForPayment;

public sealed class GetSupplierContractsForPaymentQueryHandler
    : IRequestHandler<GetSupplierContractsForPaymentQuery, GetSupplierContractsForPaymentResponse>
{
    private readonly IContractRepository _contracts;
    private readonly IPartnerPaymentRepository _payments;

    public GetSupplierContractsForPaymentQueryHandler(IContractRepository contracts, IPartnerPaymentRepository payments)
    {
        _contracts = contracts;
        _payments = payments;
    }

    public async Task<GetSupplierContractsForPaymentResponse> Handle(
        GetSupplierContractsForPaymentQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PartnerId <= 0)
            throw new ArgumentException("PartnerId is required.");

        var (items, _) = await _contracts.GetContractsAsync(1, 200, partnerId: request.PartnerId);
        var active = items
            .Where(c => !string.Equals(c.Status, ContractStatus.Cancelled, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var ids = active.Select(c => c.Id).ToList();
        var paidByContract = await _payments.GetCompletedTotalsByContractIdsAsync(ids, cancellationToken);

        var dtos = active.Select(c =>
        {
            paidByContract.TryGetValue(c.Id, out var paid);
            var total = c.TotalValue ?? 0m;
            var remaining = total - paid;
            if (remaining < 0) remaining = 0;
            return new SupplierContractForPaymentDto
            {
                Id = c.Id,
                ContractNumber = c.ContractNumber,
                Status = c.Status,
                TotalValue = c.TotalValue,
                TotalPaidCompleted = paid,
                Remaining = remaining,
            };
        }).ToList();

        return new GetSupplierContractsForPaymentResponse
        {
            PartnerId = request.PartnerId,
            Contracts = dtos,
        };
    }
}
