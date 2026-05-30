using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationComplaints.GetOrderComplaintEligibility;

public record GetOrderComplaintEligibilityQuery(int UserId, int OrderId) : IRequest<ComplaintEligibilityDto>;
