using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Models;

public class CustomerContractsPageVm
{
    public List<CustomerContractDto> Contracts { get; set; } = new();

    /// <summary>Thông báo khi tài khoản không thuộc kênh doanh nghiệp.</summary>
    public string? InfoMessage { get; set; }

    public string? ApiError { get; set; }

    /// <summary>Banner sau checkout (đọc từ TempData một lần).</summary>
    public string? PostCheckoutBanner { get; set; }

    /// <summary>JSON object cho cổng ký phụ lục đơn (đơn chưa có AnnexPdfUrl).</summary>
    public string? PendingOrderAnnexJson { get; set; }

    /// <summary>Mã đơn vừa checkout (liên kết nhanh tới chi tiết đơn).</summary>
    public int? PostCheckoutOrderId { get; set; }
}
