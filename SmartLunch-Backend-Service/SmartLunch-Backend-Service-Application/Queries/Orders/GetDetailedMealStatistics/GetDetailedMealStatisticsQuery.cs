using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.Application.Queries.Orders.GetDetailedMealStatistics;

public class GetDetailedMealStatisticsQuery : IRequest<GetDetailedMealStatisticsResponse>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? OrganizationId { get; set; }

    public GetDetailedMealStatisticsQuery(GetMealStatisticsRequest request)
    {
        StartDate = request.StartDate;
        EndDate = request.EndDate;
        OrganizationId = request.OrganizationId;
    }
}
