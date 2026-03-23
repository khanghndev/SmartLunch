namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Contracts;

public class GetContractsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

    /// <summary>Lọc hợp đồng theo đối tác cung cấp.</summary>
    public Guid? PartnerId { get; set; }
}
