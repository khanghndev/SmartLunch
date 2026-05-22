using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Models;

public class AboutPageViewModel
{
    public List<CompanyPublicDocumentClientDto> Documents { get; set; } = new();
}
