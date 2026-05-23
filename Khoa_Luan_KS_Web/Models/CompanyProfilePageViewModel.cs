using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Models;

public sealed class CompanyProfilePageViewModel
{
    public UserProfileResponse? User { get; set; }
    public OrganizationProfileClientResponse? Organization { get; set; }
    public bool IsOrganizationAccount { get; set; }
    public string? LoadError { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}
