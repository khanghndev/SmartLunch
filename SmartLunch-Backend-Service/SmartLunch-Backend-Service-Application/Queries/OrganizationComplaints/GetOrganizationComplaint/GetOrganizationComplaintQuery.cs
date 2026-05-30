using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationComplaints.GetOrganizationComplaint;

public record GetOrganizationComplaintQuery(int UserId, int ComplaintId) : IRequest<OrganizationComplaintDetailDto>;
