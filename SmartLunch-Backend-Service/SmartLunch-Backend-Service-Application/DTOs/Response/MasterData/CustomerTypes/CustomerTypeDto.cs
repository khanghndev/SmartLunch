namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.CustomerTypes;

public class CustomerTypeDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string ProfileKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

