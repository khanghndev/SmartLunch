namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Dishes;

public class DishDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameEnglish { get; set; }
    public string? Description { get; set; }
    /// <summary>Slot chính gửi AI-Services trường <c>category</c> (suy từ junction).</summary>
    public string? PrimarySlotKey { get; set; }
    /// <summary>Các khóa slot (dish_categories.SlotKey) gán cho món — AI covers_categories.</summary>
    public List<string> DishSlotCategoryCodes { get; set; } = new();
    /// <summary>MethodKey từ cooking_methods (enum AI: fried|stewed|…).</summary>
    public string? CookingMethod { get; set; }
    public decimal Price { get; set; }
    public string? DietaryLabel { get; set; }
    public string? ImageUrl { get; set; }
    public List<DishImageDto> Images { get; set; } = new();
    public decimal? Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Fat { get; set; }
    public decimal? Carbs { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class DishImageDto
{
    public int Id { get; set; }
    public int MediaFileId { get; set; }
    public string Role { get; set; } = "gallery";
    public int SortOrder { get; set; }
    public string Url { get; set; } = string.Empty;
}
