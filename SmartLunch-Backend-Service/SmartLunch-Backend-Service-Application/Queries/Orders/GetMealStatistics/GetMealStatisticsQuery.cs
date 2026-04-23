using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Orders;

namespace SmartLunch.Backend.Service.Application.Queries.Orders.GetMealStatistics;

public class GetMealStatisticsQuery : IRequest<GetMealStatisticsResponse>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? UnitId { get; set; }

    public GetMealStatisticsQuery(GetMealStatisticsRequest request)
    {
        StartDate = request.StartDate;
        EndDate = request.EndDate;
        UnitId = request.UnitId;
    }
}
