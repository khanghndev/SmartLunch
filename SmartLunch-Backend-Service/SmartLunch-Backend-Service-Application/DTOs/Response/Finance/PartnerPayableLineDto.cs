namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class PartnerPayableLineDto
{
    public Guid PartnerId { get; set; }
    public string PartnerLegalName { get; set; } = string.Empty;
    public decimal TotalContractValue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
}
