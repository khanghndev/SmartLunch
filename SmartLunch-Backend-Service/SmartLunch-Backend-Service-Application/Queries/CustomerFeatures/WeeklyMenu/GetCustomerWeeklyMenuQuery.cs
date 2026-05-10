using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.Customer.WeeklyMenu;

namespace SmartLunch.Backend.Service.Application.Queries.CustomerFeatures.WeeklyMenu;

public class GetCustomerWeeklyMenuQuery : IRequest<GetCustomerWeeklyMenuResponse>
{
    public DateTime? Date { get; }
    public int? CustomerTypeId { get; }

    public GetCustomerWeeklyMenuQuery(DateTime? date = null, int? customerTypeId = null)
    {
        Date = date;
        CustomerTypeId = customerTypeId;
    }
}
