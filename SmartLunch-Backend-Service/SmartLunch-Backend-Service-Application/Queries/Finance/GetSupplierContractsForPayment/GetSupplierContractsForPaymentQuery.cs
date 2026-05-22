using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

namespace SmartLunch.Backend.Service.Application.Queries.Finance.GetSupplierContractsForPayment;

public sealed record GetSupplierContractsForPaymentQuery(int PartnerId) : IRequest<GetSupplierContractsForPaymentResponse>;
