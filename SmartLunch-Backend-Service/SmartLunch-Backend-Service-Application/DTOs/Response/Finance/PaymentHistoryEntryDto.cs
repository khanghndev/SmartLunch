namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class PaymentHistoryEntryDto
{
    /// <summary>customer = thanh toán đơn hàng; supplier = thanh toán NCC.</summary>
    public string Source { get; set; } = string.Empty;
    public Guid EntryId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? OrderId { get; set; }
    public Guid? UnitId { get; set; }
    public string? UnitName { get; set; }
    public Guid? PartnerId { get; set; }
    public string? PartnerLegalName { get; set; }
    public Guid? ContractId { get; set; }
    public string? ContractNumber { get; set; }
}
