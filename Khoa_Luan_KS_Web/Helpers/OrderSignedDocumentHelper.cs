using Khoa_Luan_KS_Web.Services;

namespace Khoa_Luan_KS_Web.Helpers;

/// <summary>PDF hợp đồng / phụ lục đã ký gắn với đơn hàng.</summary>
public static class OrderSignedDocumentHelper
{
    public static string? GetSignedPdfUrl(OrderDetailClientDto order)
    {
        if (!string.IsNullOrWhiteSpace(order.AnnexPdfUrl))
            return order.AnnexPdfUrl.Trim();

        if (!string.IsNullOrWhiteSpace(order.ContractSummary?.ContractFileUrl))
            return order.ContractSummary.ContractFileUrl.Trim();

        return null;
    }
}
