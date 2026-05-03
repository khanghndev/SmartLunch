namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.WeeklyMenus;

public class WeeklyMenuDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<WeeklyMenuImageDto> Images { get; set; } = new();
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WeeklyMenuImageDto
{
    public int Id { get; set; }
    public int MediaFileId { get; set; }
    public string Role { get; set; } = "gallery";
    public int SortOrder { get; set; }
    public string Url { get; set; } = string.Empty;
}
