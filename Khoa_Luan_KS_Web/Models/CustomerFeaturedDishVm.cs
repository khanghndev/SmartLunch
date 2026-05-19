namespace Khoa_Luan_KS_Web.Models;

public sealed class CustomerFeaturedDishVm
{
    public int DishId { get; set; }
    public int? MenuId { get; set; }
    public int? ScheduleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    /// <summary>all | factory | school | office</summary>
    public string SegmentFilter { get; set; } = "all";
    public string SegmentLabel { get; set; } = string.Empty;
    public string BadgeClass { get; set; } = "bg-orange-500";
    public List<string> Tags { get; set; } = new();
    public string? ProfileKey { get; set; }
    public string DetailUrl { get; set; } = "/Menu";
}

public sealed class CustomerHomeIndexVm
{
    public List<CustomerFeaturedDishVm> FeaturedDishes { get; set; } = new();
    public bool RequiresLogin { get; set; }
    public string? LoadError { get; set; }
}
