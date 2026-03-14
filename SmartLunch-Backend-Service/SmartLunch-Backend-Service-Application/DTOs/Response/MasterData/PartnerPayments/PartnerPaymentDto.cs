namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.PartnerPayments;

public class PartnerPaymentDto
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public Guid PartnerId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
