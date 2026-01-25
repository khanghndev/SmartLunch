namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Permissions;

public class GetPermissionsRequest
{
    public bool? IsActive { get; set; }
    public string? Resource { get; set; }
    public string? Action { get; set; }
}
