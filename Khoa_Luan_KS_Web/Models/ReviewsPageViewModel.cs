using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Models;

public class ReviewsPageViewModel
{
    public List<PublicReviewClientDto> Reviews { get; set; } = new();
    public double AverageRating { get; set; }
    public int TotalCount { get; set; }
    public GetReviewMeContextClientResponse? Context { get; set; }
    public bool IsAuthenticated => Context != null || CanSubmitLoaded;
    public bool CanSubmitLoaded { get; set; }
}
