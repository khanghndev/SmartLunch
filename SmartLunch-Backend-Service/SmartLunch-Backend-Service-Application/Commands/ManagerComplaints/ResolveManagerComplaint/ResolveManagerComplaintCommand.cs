using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;

namespace SmartLunch.Backend.Service.Application.Commands.ManagerComplaints.ResolveManagerComplaint;

public record ResolveManagerComplaintCommand(int ManagerUserId, int ComplaintId, ResolveManagerComplaintRequest Request)
    : IRequest<ManagerComplaintDetailDto>;
