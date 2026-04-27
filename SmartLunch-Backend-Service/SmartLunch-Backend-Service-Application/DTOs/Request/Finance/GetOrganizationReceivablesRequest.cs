namespace SmartLunch.Backend.Service.Application.DTOs.Request.Finance;

public class GetOrganizationReceivablesRequest
{
    public int? OrganizationId { get; set; }

    /// <summary>Chỉ trả các tổ chức còn dư nợ (tổng đơn trừ đã thu còn dương).</summary>
    public bool OnlyWithOutstanding { get; set; } = true;
}
