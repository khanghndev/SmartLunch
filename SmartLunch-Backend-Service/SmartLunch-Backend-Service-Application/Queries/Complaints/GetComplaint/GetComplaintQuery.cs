using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Complaints;

namespace SmartLunch.Backend.Service.Application.Queries.Complaints.GetComplaint;

public class GetComplaintQuery : IRequest<GetComplaintResponse>
{
    public Guid ComplaintId { get; set; }

    public GetComplaintQuery(Guid complaintId)
    {
        ComplaintId = complaintId;
    }
}
