namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;

public class GetMealStatisticsRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? OrganizationId { get; set; }
}
