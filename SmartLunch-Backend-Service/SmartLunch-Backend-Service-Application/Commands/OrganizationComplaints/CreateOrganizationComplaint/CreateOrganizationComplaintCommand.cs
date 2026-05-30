using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationComplaints.CreateOrganizationComplaint;

public record CreateOrganizationComplaintCommand(int UserId, CreateOrganizationComplaintRequest Request)
    : IRequest<OrganizationComplaintDetailDto>;
