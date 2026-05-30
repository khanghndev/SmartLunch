namespace SmartLunch.Backend.Service.Application.Constants;

public static class ComplaintEvidenceKind
{
    public const string ReceiptPhoto = "receipt_photo";
    public const string UnboxingVideo = "unboxing_video";
    public const string PortionCountVideo = "portion_count_video";
    public const string FoodConditionVideo = "food_condition_video";
    public const string Other = "other";

    public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
    {
        ReceiptPhoto, UnboxingVideo, PortionCountVideo, FoodConditionVideo, Other
    };
}
