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
}
