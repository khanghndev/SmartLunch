using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationComplaints.SubmitOrganizationComplaint;

public record SubmitOrganizationComplaintCommand(int UserId, int ComplaintId) : IRequest<OrganizationComplaintDetailDto>;
