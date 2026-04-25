namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserUnits;

public class CreateUserUnitResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int UnitId { get; set; }
    public string Message { get; set; } = "User unit created successfully";
}
