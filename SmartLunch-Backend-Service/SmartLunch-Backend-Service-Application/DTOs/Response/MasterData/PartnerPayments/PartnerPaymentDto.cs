namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.PartnerPayments;

public class PartnerPaymentDto
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public int PartnerId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
