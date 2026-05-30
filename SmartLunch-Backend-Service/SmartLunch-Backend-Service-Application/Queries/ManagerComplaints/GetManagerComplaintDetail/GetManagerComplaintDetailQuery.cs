using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;

namespace SmartLunch.Backend.Service.Application.Queries.ManagerComplaints.GetManagerComplaintDetail;

public record GetManagerComplaintDetailQuery(int ComplaintId) : IRequest<ManagerComplaintDetailDto>;
