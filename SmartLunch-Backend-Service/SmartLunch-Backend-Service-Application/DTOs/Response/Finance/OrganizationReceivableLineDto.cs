namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class OrganizationReceivableLineDto
{
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalBilled { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
}
