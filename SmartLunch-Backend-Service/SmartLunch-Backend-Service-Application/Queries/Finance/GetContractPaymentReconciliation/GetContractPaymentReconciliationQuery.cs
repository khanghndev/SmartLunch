using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Finance;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetContractPaymentReconciliation;

public record GetContractPaymentReconciliationQuery(
    int ContractId,
    GetContractPaymentReconciliationRequest Request) : IRequest<GetContractPaymentReconciliationResponse>;
