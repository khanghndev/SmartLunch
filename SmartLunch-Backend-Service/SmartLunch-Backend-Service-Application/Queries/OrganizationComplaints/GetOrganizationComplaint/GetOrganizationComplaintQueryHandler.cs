using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationComplaints;

namespace SmartLunch.Backend.Service.Application.Queries.OrganizationComplaints.GetOrganizationComplaint;

public class GetOrganizationComplaintQueryHandler : IRequestHandler<GetOrganizationComplaintQuery, OrganizationComplaintDetailDto>
{
    private readonly IComplaintRepository _complaints;
    private readonly IStorageService _storage;

    public GetOrganizationComplaintQueryHandler(IComplaintRepository complaints, IStorageService storage)
    {
        _complaints = complaints;
        _storage = storage;
    }

    public async Task<OrganizationComplaintDetailDto> Handle(GetOrganizationComplaintQuery request, CancellationToken cancellationToken)
    {
        var complaint = await _complaints.GetByIdWithDetailsAsync(request.ComplaintId, cancellationToken);
        if (complaint == null)
            throw new KeyNotFoundException("Khiếu nại không tồn tại.");
        if (complaint.UserId != request.UserId)
            throw new UnauthorizedAccessException("Không có quyền.");

        return await ComplaintMapper.ToDetailAsync(complaint, _storage, cancellationToken);
    }
}
