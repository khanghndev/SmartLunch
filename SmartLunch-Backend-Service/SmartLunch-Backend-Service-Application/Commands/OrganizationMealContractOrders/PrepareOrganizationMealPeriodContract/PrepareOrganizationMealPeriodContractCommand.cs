using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealContractOrders;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealContractOrders.PrepareOrganizationMealPeriodContract;

public sealed record PrepareOrganizationMealPeriodContractCommand(
    int UserId,
    PrepareOrganizationMealPeriodContractRequest Request)
    : IRequest<PrepareOrganizationMealPeriodContractResponse>;
