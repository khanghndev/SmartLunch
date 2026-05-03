namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Dishes;

public class UpdateDishRequest
{
    public string Name { get; set; } = string.Empty;
    /// <summary>Nếu null trong JSON: giữ nguyên giá trị DB.</summary>
    public string? NameEnglish { get; set; }
    public string? Description { get; set; }
    /// <summary>Nếu null: không đổi junction. Nếu gửi (kể cả rỗng): thay toàn bộ slot.</summary>
    public List<string>? DishSlotCategoryCodes { get; set; }
    /// <summary>Nếu null: giữ nguyên DB. Chuỗi rỗng: xóa FK. Giá trị: MethodKey (cooking_methods).</summary>
    public string? CookingMethod { get; set; }
    public decimal Price { get; set; }
    public string? DietaryLabel { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Fat { get; set; }
    public decimal? Carbs { get; set; }

    /// <summary>Ẩn món: đặt false (không xóa bản ghi).</summary>
    public bool IsActive { get; set; } = true;

    public List<UpdateDishImageItemRequest>? Images { get; set; }
}

public class UpdateDishImageItemRequest
{
    public int MediaFileId { get; set; }
    public string Role { get; set; } = "gallery";
    public int? SortOrder { get; set; }
}
