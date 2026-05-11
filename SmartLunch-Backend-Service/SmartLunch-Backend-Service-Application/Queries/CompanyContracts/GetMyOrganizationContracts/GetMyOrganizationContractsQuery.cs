using MediatR;

namespace SmartLunch.Backend.Service.Application.Queries.CompanyContracts.GetMyOrganizationContracts;

public record GetMyOrganizationContractsQuery(int UserId) : IRequest<GetMyOrganizationContractsResponse>;
