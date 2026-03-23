namespace SmartLunch.Backend.Service.Application.DTOs.Response.Finance;

public class GetUnitReceivablesResponse
{
    public List<UnitReceivableLineDto> Lines { get; set; } = new();
    public decimal GrandTotalOutstanding { get; set; }
}
