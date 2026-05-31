namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Orders;

public class GetOrdersRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    /// <summary>Filter by meal day (matches Order.ScheduledDate calendar day).</summary>
    public DateOnly? ScheduledOn { get; set; }

    /// <summary>Filter by order status (e.g. pending, confirmed, delivered).</summary>
    public string? Status { get; set; }

    /// <summary>Filter by payment status (e.g. unpaid, awaiting_payment, paid).</summary>
    public string? PaymentStatus { get; set; }
}
