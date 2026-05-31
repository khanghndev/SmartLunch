using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationMealContractOrders;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationMealContractOrders.GetMealPortionPrices;

public sealed record GetMealPortionPricesQuery : IRequest<GetMealPortionPricesResponse>;
