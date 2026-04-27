namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.UserOrganizations;

public class CreateUserOrganizationResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int OrganizationId { get; set; }
    public string Message { get; set; } = "User organization created successfully";
}
