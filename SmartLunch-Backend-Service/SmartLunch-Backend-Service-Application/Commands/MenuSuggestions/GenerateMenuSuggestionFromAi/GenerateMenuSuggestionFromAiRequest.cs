namespace SmartLunch.Backend.Service.Application.Commands.MenuSuggestions.GenerateMenuSuggestionFromAi;

/// <summary>
/// Request từ FE: chỉ cần gửi IDs của món ăn, không cần điền đầy đủ thông tin.
/// Backend sẽ tự fetch dữ liệu từ DB và tính popularity từ lịch sử đặt hàng.
/// </summary>
public class GenerateMenuSuggestionFromAiRequest
{
    public DateTime WeekStartUtc { get; set; }
    public string RulesKey { get; set; } = "industrial";
    public decimal BudgetPerServing { get; set; }
    public int TopK { get; set; } = 3;
    public decimal TimeLimitSeconds { get; set; } = 5;

    public List<string> Days { get; set; } = new();

    /// <summary>
    /// Khung bữa ăn theo enum AI Service: main | side | soup | vegetable | noodle_soup | dessert.
    /// FE có thể truyền tiếng Việt ("Cơm phần", "Canh", ...) hoặc enum tiếng Anh — BE sẽ tự map.
    /// </summary>
    public List<string> MealStructure { get; set; } = new();

    /// <summary>
    /// Danh sách ID món ăn (từ bảng dishes). BE sẽ fetch đầy đủ thông tin từ DB.
    /// </summary>
    public List<int> DishIds { get; set; } = new();
}
