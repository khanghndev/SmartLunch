using MediatR;
using Microsoft.Extensions.Logging;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Complaints;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Application.Queries.Complaints.GetComplaints;

public class GetComplaintsQueryHandler : IRequestHandler<GetComplaintsQuery, GetComplaintsResponse>
{
    private readonly IComplaintRepository _complaintRepository;
    private readonly ILogger<GetComplaintsQueryHandler> _logger;

    public GetComplaintsQueryHandler(IComplaintRepository complaintRepository, ILogger<GetComplaintsQueryHandler> logger)
    {
        _complaintRepository = complaintRepository;
        _logger = logger;
    }

    public async Task<GetComplaintsResponse> Handle(GetComplaintsQuery request, CancellationToken cancellationToken)
    {
        var (complaints, totalCount) = await _complaintRepository.GetComplaintsAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm);

        var complaintDtos = complaints.Select(complaint => new ComplaintDto
        {
                Id = complaint.Id,
                UserId = complaint.UserId,
                OrderId = complaint.OrderId,
                Title = complaint.Title,
                Description = complaint.Description,
                Status = complaint.Status,
                AssignedTo = complaint.AssignedTo,
                CreatedAt = complaint.CreatedAt,
                ResolvedAt = complaint.ResolvedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} complaints (Page {Page}, PageSize {PageSize})",
            complaintDtos.Count, request.Page, request.PageSize);

        return new GetComplaintsResponse
        {
            Data = complaintDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
