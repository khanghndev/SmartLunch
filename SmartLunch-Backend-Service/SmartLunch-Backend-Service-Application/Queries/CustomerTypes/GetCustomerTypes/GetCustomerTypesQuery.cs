using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.CustomerTypes;

namespace SmartLunch.Backend.Service.Application.Queries.CustomerTypes.GetCustomerTypes;

public record GetCustomerTypesQuery() : IRequest<GetCustomerTypesResponse>;

