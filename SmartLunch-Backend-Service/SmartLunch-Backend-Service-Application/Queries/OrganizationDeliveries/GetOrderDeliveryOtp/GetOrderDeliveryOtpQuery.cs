using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationDeliveries;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationDeliveries.GetOrderDeliveryOtp;

public record GetOrderDeliveryOtpQuery(int UserId, int OrderId) : IRequest<OrganizationDeliveryOtpResponse>;
