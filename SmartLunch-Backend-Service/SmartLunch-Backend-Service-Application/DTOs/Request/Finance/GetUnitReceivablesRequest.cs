namespace SmartLunch.Backend.Service.Application.DTOs.Request.Finance;

public class GetUnitReceivablesRequest
{
    public int? UnitId { get; set; }

    /// <summary>Chỉ trả các đơn vị còn dư nợ (tổng đơn trừ đã thu còn dương).</summary>
    public bool OnlyWithOutstanding { get; set; } = true;
}
