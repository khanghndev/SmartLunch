using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Finance;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetContractPayments;

public record GetContractPaymentsQuery(int ContractId, GetContractPaymentsRequest Request)
    : IRequest<GetContractPaymentsResponse>;
