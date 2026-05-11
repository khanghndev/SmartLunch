using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.CreateCustomerMealOrder;

public record CreateCustomerMealOrderCommand(CreateCustomerMealOrderRequest Request, int CustomerUserId)
    : IRequest<GetOrderResponse>;
