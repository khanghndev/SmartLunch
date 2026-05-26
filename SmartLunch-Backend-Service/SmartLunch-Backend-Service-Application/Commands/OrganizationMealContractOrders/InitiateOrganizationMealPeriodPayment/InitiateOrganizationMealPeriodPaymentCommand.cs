using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealContractOrders.InitiateOrganizationMealPeriodPayment;

public sealed record InitiateOrganizationMealPeriodPaymentCommand(
    int UserId,
    InitiateOrganizationMealPeriodPaymentRequest Request)
    : IRequest<InitiateOrganizationMealPaymentResponse>;
