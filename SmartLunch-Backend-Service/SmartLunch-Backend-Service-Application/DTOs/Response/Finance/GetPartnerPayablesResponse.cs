namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class GetPartnerPayablesResponse
{
    public List<PartnerPayableLineDto> Lines { get; set; } = new();
    public decimal GrandTotalOutstanding { get; set; }
}
