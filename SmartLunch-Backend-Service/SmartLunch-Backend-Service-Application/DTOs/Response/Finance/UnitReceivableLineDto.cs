namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class UnitReceivableLineDto
{
    public Guid UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalBilled { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
}
