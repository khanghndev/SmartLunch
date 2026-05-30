namespace SmartLunch.Backend.Service.Application.Constants;

public static class ComplaintStatus
{
    public const string Draft = "draft";
    public const string PendingReview = "pending_review";
    public const string Resolved = "resolved";
    public const string Rejected = "rejected";

    /// <summary>Legacy seed values.</summary>
    public const string LegacyNew = "new";
    public const string LegacyInProgress = "in_progress";
}
