using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.Application.Commands.MasterData.Orders.CreateSalesInvoice;

public record CreateSalesInvoiceCommand(
    CreateSalesInvoiceRequest Request,
    int SalesUserId) : IRequest<GetOrderResponse>;
