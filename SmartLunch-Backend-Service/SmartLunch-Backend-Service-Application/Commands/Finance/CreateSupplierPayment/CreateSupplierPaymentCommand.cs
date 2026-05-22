using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.Finance;
using SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

namespace SmartLunch.Backend.Service.Application.Commands.Finance.CreateSupplierPayment;

public sealed record CreateSupplierPaymentCommand(CreateSupplierPaymentRequest Request)
    : IRequest<CreateSupplierPaymentResponse>;
