using Khoa_Luan_KS_Web.Services;

using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Models;

public sealed class DishGalleryViewModel
{
    public GetPublicDishesBrowseClientResponse Browse { get; set; } = new();
    public GetOrganizationDishCategoriesClientResponse Categories { get; set; } = new();
    public int? SelectedCategoryId { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public string? LoadError { get; set; }
}

public sealed class DishDetailPageViewModel
{
    public DishDetailResponse? Detail { get; set; }
    public string? LoadError { get; set; }
    /// <summary>Khi mở từ thực đơn tuần (menuId + scheduleId).</summary>
    public DishDetailMenuContext? MenuContext { get; set; }
    public List<PublicDishBrowseItemClientDto> RelatedDishes { get; set; } = new();
    public int? RelatedCategoryId { get; set; }
}

public sealed class DishDetailMenuContext
{
    public int MenuId { get; set; }
    public int ScheduleId { get; set; }
    public DateTime MenuStartDate { get; set; }
    public DateTime MenuEndDate { get; set; }
    public DateTime ScheduleDate { get; set; }
    public string? MealSlot { get; set; }
}
