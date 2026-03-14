namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserTokens;

public class GetUserTokensRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
}
