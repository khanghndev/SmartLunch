using System.Text.Json;

namespace SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.MenuSuggestions;

/// <summary>
/// MenuSuggestion summary + full plans hierarchy — mirrors the AI Service response structure.
/// </summary>
public class MenuSuggestionDto
{
    public int Id { get; set; }
    public DateTime WeekStart { get; set; }
    public DateTime GeneratedAt { get; set; }
    public int Version { get; set; }
    public string? RulesKey { get; set; }
    public decimal? BudgetPerServing { get; set; }
    public int? TopK { get; set; }
    public decimal? TimeLimitSeconds { get; set; }
    public int? PlanCount { get; set; }
    public string SuggestionText { get; set; } = string.Empty;
    public string? AlgorithmVersion { get; set; }
    public int? CreatedBy { get; set; }

    /// <summary>Danh sách phương án (rank-ordered). Rỗng ở list endpoint.</summary>
    public List<MenuSuggestionPlanDto> Plans { get; set; } = new();
}

/// <summary>Một phương án thực đơn (1 trong topK).</summary>
public class MenuSuggestionPlanDto
{
    public int Id { get; set; }
    public int Rank { get; set; }
    public decimal PlanScore { get; set; }
    public decimal ObjectiveValue { get; set; }

    /// <summary>Lịch theo ngày (day-ordered).</summary>
    public List<MenuSuggestionPlanDayDto> Days { get; set; } = new();
}

/// <summary>Thực đơn 1 ngày trong phương án.</summary>
public class MenuSuggestionPlanDayDto
{
    public int Id { get; set; }

    /// <summary>0 = Thứ 2, 1 = Thứ 3, ..., 5 = Thứ 7</summary>
    public int DayIndex { get; set; }

    /// <summary>Tên ngày (ví dụ: "Thứ 2").</summary>
    public string DayName { get; set; } = string.Empty;

    /// <summary>Các slot món ăn trong ngày.</summary>
    public List<MenuSuggestionPlanItemDto> Items { get; set; } = new();
}

/// <summary>
/// Một slot món ăn — cấu trúc khớp với AI Service response (IndustrialMenuDayDish).
/// </summary>
public class MenuSuggestionPlanItemDto
{
    public int Id { get; set; }

    /// <summary>Danh mục slot: main | side | soup | vegetable | noodle_soup | dessert</summary>
    public string SlotCategory { get; set; } = string.Empty;

    /// <summary>Tên món ăn (tiếng Việt, để hiển thị).</summary>
    public string DishName { get; set; } = string.Empty;

    /// <summary>Danh mục gốc của món (nếu cover nhiều category).</summary>
    public string? DishSourceCategory { get; set; }

    /// <summary>Điểm AI (0–5).</summary>
    public decimal Score { get; set; }

    /// <summary>Chi phí nguyên liệu / suất (VND).</summary>
    public decimal CostPerServing { get; set; }

    /// <summary>
    /// Danh sách lý do lựa chọn — deserialized từ JSON column trong DB.
    /// Ví dụ: ["Món phổ biến", "Giá phù hợp ngân sách", "Đa dạng protein"]
    /// </summary>
    public List<string> Reasons { get; set; } = new();

    /// <summary>
    /// Helper: gán ReasonsJson (raw JSON string từ DB) và tự động deserialize sang Reasons.
    /// Được gọi bởi query handler khi map từ entity.
    /// </summary>
    public void SetReasonsFromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            Reasons = new List<string>();
            return;
        }
        try
        {
            Reasons = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            Reasons = new List<string>();
        }
    }
}
