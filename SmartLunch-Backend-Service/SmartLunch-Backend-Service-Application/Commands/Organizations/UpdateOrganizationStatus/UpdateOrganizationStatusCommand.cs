using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Organizations;

namespace SmartLunch.Backend.Service.Application.Commands.Organizations.UpdateOrganizationStatus;

public record UpdateOrganizationStatusCommand(int OrganizationId, bool IsActive) : IRequest<GetOrganizationResponse>;
