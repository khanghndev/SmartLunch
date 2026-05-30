namespace SmartLunch.Backend.Service.Application.DTOs.Response.Manager.Deliveries;

public sealed class ManagerDeliveryListItemDto
{
    public int DeliveryId { get; set; }
    public string? DeliveryCode { get; set; }
    public int OrderId { get; set; }
    public string? OrderCode { get; set; }
    public string? InvoiceCode { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryStatus { get; set; } = string.Empty;
    public string DeliveryStatusLabel { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string? PreferredDeliveryTime { get; set; }
    public int MealCount { get; set; }
    public int? AssignedStaffId { get; set; }
    public string? AssignedStaffName { get; set; }
    public string? AssignedStaffPhone { get; set; }
    public string? Notes { get; set; }
    public string? ProofImageUrl { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? RecipientConfirmedName { get; set; }
    public DateTime? RecipientConfirmedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class GetManagerDeliveriesResponse
{
    public List<ManagerDeliveryListItemDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public ManagerDeliveryStatsDto Stats { get; set; } = new();
}

public sealed class ManagerDeliveryStatsDto
{
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }
    public int Unassigned { get; set; }
}

public sealed class ManagerShipperOptionDto
{
    public int UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
}

public sealed class GetManagerShippersResponse
{
    public List<ManagerShipperOptionDto> Shippers { get; set; } = new();
}
