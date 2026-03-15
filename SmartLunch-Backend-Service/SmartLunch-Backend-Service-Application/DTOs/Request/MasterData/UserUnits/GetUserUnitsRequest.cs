namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.UserUnits;

public class GetUserUnitsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? UserId { get; set; }
    public Guid? UnitId { get; set; }
    public bool? IsActive { get; set; }
}
