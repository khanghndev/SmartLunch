using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealContractOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealContractOrders;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealContractOrders.SubmitOrganizationMealWeeklySelection;

public sealed record SubmitOrganizationMealWeeklySelectionCommand(
    int UserId,
    int ContractId,
    SubmitOrganizationMealWeeklySelectionRequest Request)
    : IRequest<SubmitOrganizationMealWeeklySelectionResponse>;
