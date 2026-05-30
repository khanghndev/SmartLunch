using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationComplaints;

namespace SmartLunch.Backend.Service.Application.Queries.ManagerComplaints.GetManagerComplaintDetail;

public class GetManagerComplaintDetailQueryHandler : IRequestHandler<GetManagerComplaintDetailQuery, ManagerComplaintDetailDto>
{
    private readonly IComplaintRepository _complaints;
    private readonly IStorageService _storage;

    public GetManagerComplaintDetailQueryHandler(IComplaintRepository complaints, IStorageService storage)
    {
        _complaints = complaints;
        _storage = storage;
    }

    public async Task<ManagerComplaintDetailDto> Handle(GetManagerComplaintDetailQuery request, CancellationToken cancellationToken)
    {
        var complaint = await _complaints.GetByIdWithDetailsAsync(request.ComplaintId, cancellationToken);
        if (complaint == null)
            throw new KeyNotFoundException("Khiếu nại không tồn tại.");

        return await ComplaintMapper.ToManagerDetailAsync(complaint, _storage, cancellationToken);
    }
}
