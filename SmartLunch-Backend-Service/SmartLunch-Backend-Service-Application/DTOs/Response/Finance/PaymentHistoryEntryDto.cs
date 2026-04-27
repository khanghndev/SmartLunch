namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class PaymentHistoryEntryDto
{
    /// <summary>customer = thanh toán đơn hàng; supplier = thanh toán NCC.</summary>
    public string Source { get; set; } = string.Empty;
    public int EntryId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? OrderId { get; set; }
    public int? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public int? PartnerId { get; set; }
    public string? PartnerLegalName { get; set; }
    public int? ContractId { get; set; }
    public string? ContractNumber { get; set; }
}
