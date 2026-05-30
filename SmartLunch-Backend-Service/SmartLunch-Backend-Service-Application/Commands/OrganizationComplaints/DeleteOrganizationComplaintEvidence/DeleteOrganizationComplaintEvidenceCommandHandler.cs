using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationComplaints;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Application.OrganizationComplaints;

namespace SmartLunch.Backend.Service.Application.Commands.OrganizationComplaints.DeleteOrganizationComplaintEvidence;

public class DeleteOrganizationComplaintEvidenceCommandHandler
    : IRequestHandler<DeleteOrganizationComplaintEvidenceCommand, OrganizationComplaintDetailDto>
{
    private readonly IComplaintRepository _complaints;
    private readonly IStorageService _storage;

    public DeleteOrganizationComplaintEvidenceCommandHandler(IComplaintRepository complaints, IStorageService storage)
    {
        _complaints = complaints;
        _storage = storage;
    }

    public async Task<OrganizationComplaintDetailDto> Handle(DeleteOrganizationComplaintEvidenceCommand command, CancellationToken cancellationToken)
    {
        var complaint = await _complaints.GetByIdWithDetailsAsync(command.ComplaintId, cancellationToken);
        if (complaint == null)
            throw new KeyNotFoundException("Khiếu nại không tồn tại.");
        if (complaint.UserId != command.UserId)
            throw new UnauthorizedAccessException("Không có quyền.");
        if (!OrganizationComplaintRules.IsEditable(complaint))
            throw new InvalidOperationException("Chỉ xóa bằng chứng khi khiếu nại còn ở trạng thái nháp.");

        var evidence = await _complaints.GetEvidenceAsync(command.EvidenceId, command.ComplaintId, cancellationToken);
        if (evidence == null)
            throw new KeyNotFoundException("Bằng chứng không tồn tại.");

        try
        {
            await _storage.DeleteObjectAsync(evidence.StorageObjectName);
        }
        catch
        {
            // Best-effort delete from storage
        }

        await _complaints.DeleteEvidenceAsync(evidence, cancellationToken);

        var reloaded = await _complaints.GetByIdWithDetailsAsync(command.ComplaintId, cancellationToken) ?? complaint;
        return await ComplaintMapper.ToDetailAsync(reloaded, _storage, cancellationToken);
    }
}
