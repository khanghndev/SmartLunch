using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Complaints;

namespace SmartLunch.Backend.Service.Application.Queries.Complaints.GetComplaint;

public class GetComplaintQuery : IRequest<GetComplaintResponse>
{
    public int ComplaintId { get; set; }

    public GetComplaintQuery(int complaintId)
    {
        ComplaintId = complaintId;
    }
}
