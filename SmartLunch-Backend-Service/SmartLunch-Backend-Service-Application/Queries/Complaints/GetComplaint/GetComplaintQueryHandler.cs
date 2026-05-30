using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Complaints;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.Queries.Complaints.GetComplaint;

public class GetComplaintQueryHandler : IRequestHandler<GetComplaintQuery, GetComplaintResponse>
{
    private readonly IComplaintRepository _complaintRepository;
    private readonly ILogger<GetComplaintQueryHandler> _logger;

    public GetComplaintQueryHandler(IComplaintRepository complaintRepository, ILogger<GetComplaintQueryHandler> logger)
    {
        _complaintRepository = complaintRepository;
        _logger = logger;
    }

    public async Task<GetComplaintResponse> Handle(GetComplaintQuery request, CancellationToken cancellationToken)
    {
        var complaint = await _complaintRepository.GetByIdAsync(request.ComplaintId, cancellationToken);

        if (complaint == null)
        {
            _logger.LogWarning("Complaint not found with ID: {ComplaintId}", request.ComplaintId);
            return new GetComplaintResponse { Complaint = new ComplaintDto() };
        }

        return new GetComplaintResponse { Complaint = ToDto(complaint) };
    }

    private static ComplaintDto ToDto(Complaint complaint) => new()
    {
        Id = complaint.Id,
        Code = complaint.Code,
        UserId = complaint.UserId,
        OrderId = complaint.OrderId,
        Title = complaint.Title,
        Description = complaint.Description,
        Reason = complaint.Reason,
        Status = complaint.Status,
        Resolution = complaint.Resolution,
        AssignedTo = complaint.AssignedTo,
        CreatedAt = complaint.CreatedAt,
        SubmittedAt = complaint.SubmittedAt,
        ComplaintDeadlineAt = complaint.ComplaintDeadlineAt,
        RefundPortionCount = complaint.RefundPortionCount,
        SuggestedRefundAmount = complaint.SuggestedRefundAmount,
        FinalRefundAmount = complaint.FinalRefundAmount,
        RefundPaymentId = complaint.RefundPaymentId,
        ResolvedAt = complaint.ResolvedAt,
    };
}
