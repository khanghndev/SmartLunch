namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class GetOrganizationReceivablesResponse
{
    public List<OrganizationReceivableLineDto> Lines { get; set; } = new();
    public decimal GrandTotalOutstanding { get; set; }
}
