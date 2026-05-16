using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealOrders.InitiateOrganizationMealPayment;

public sealed record InitiateOrganizationMealPaymentCommand(int UserId, InitiateOrganizationMealPaymentRequest Request)
    : IRequest<InitiateOrganizationMealPaymentResponse>;
