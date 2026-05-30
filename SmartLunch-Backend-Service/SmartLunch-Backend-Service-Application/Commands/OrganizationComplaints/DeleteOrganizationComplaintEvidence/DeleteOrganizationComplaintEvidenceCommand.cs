using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationComplaints.DeleteOrganizationComplaintEvidence;

public record DeleteOrganizationComplaintEvidenceCommand(int UserId, int ComplaintId, int EvidenceId)
    : IRequest<OrganizationComplaintDetailDto>;
