using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealOrders.PrepareOrganizationMealContract;

public sealed record PrepareOrganizationMealContractCommand(
    int UserId,
    PrepareOrganizationMealContractRequest Request) : IRequest<PrepareOrganizationMealContractResponse>;
