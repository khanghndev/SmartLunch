using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationMealOrders;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealOrders;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationMealOrders.CheckoutOrganizationMeal;

public sealed record CheckoutOrganizationMealCommand(int UserId, CheckoutOrganizationMealRequest Request)
    : IRequest<CheckoutOrganizationMealResponse>;
