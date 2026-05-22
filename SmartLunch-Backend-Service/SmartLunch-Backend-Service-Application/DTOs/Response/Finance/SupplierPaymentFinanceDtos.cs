namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class CreateSupplierPaymentResponse
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int ContractId { get; set; }
    public int PartnerId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = "Đã ghi nhận thanh toán.";
}

public class SupplierContractForPaymentDto
{
    public int Id { get; set; }
    public string? ContractNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? TotalValue { get; set; }
    public decimal TotalPaidCompleted { get; set; }
    public decimal Remaining { get; set; }
}

public class GetSupplierContractsForPaymentResponse
{
    public int PartnerId { get; set; }
    public List<SupplierContractForPaymentDto> Contracts { get; set; } = new();
}
