namespace SmartLunch.Backend.Service.Application.DTOs.Request.Finance;

public class CreateSupplierPaymentRequest
{
    public int ContractId { get; set; }
    public int PartnerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Method { get; set; } = "bank_transfer";
    /// <summary>pending | completed | failed — mặc định completed khi ghi nhận chi thực tế.</summary>
    public string Status { get; set; } = "completed";
}
