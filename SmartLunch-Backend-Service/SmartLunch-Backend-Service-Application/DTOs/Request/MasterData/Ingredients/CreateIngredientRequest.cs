namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Ingredients;

public class CreateIngredientRequest
{
    public string Name { get; set; } = string.Empty;
    public string? NameEnglish { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? DefaultSupplierId { get; set; }
    public decimal? CostPerUnit { get; set; }
    public int? CategoryId { get; set; }
    public bool IsActive { get; set; } = true;
    /// <summary>Mức tồn cảnh báo khi tạo bản ghi kho (tùy chọn).</summary>
    public decimal? ReorderLevel { get; set; }
}
