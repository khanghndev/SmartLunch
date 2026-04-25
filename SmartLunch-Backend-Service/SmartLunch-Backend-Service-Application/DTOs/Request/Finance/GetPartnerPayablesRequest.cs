namespace SmartLunch.Backend.Service.Application.DTOs.Request.Finance;

public class GetPartnerPayablesRequest
{
    public int? PartnerId { get; set; }

    /// <summary>Chỉ trả NCC còn phải trả (giá trị hợp đồng trừ đã chi còn dương).</summary>
    public bool OnlyWithOutstanding { get; set; } = true;
}
