namespace SmartLunch.Backend.Service.Application.Constants;

public static class ComplaintReason
{
    public const string MissingPortions = "missing_portions";
    public const string SpoiledRice = "spoiled_rice";
    public const string WrongDish = "wrong_dish";
    public const string FoodQuality = "food_quality";

    public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
    {
        MissingPortions, SpoiledRice, WrongDish, FoodQuality
    };
}
