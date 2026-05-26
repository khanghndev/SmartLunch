using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealContractOrders;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealContractOrders.CheckoutOrganizationMealPeriodContract;

public sealed record CheckoutOrganizationMealPeriodContractCommand(
    int UserId,
    CheckoutOrganizationMealPeriodContractRequest Request)
    : IRequest<CheckoutOrganizationMealPeriodContractResponse>;
