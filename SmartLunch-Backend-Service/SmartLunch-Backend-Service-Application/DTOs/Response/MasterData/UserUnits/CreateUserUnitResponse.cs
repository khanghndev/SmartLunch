namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserUnits;

public class CreateUserUnitResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid UnitId { get; set; }
    public string Message { get; set; } = "User unit created successfully";
}
