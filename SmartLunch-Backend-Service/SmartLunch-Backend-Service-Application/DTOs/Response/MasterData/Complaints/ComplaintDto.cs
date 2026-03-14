namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Complaints;

public class ComplaintDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? OrderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
