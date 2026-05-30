using SmartLunch.Backend.Service.Application.Constants;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.OrganizationComplaints;

public static class ComplaintEvidenceValidator
{
    public static void ValidateForSubmit(Complaint complaint)
    {
        var evidence = complaint.Evidence?.ToList() ?? [];
        var hasReceiptPhoto = evidence.Any(e =>
            string.Equals(e.MediaType, "image", StringComparison.OrdinalIgnoreCase)
            && string.Equals(e.Kind, ComplaintEvidenceKind.ReceiptPhoto, StringComparison.OrdinalIgnoreCase));

        var hasVideo = evidence.Any(e =>
            string.Equals(e.MediaType, "video", StringComparison.OrdinalIgnoreCase));

        if (!hasReceiptPhoto)
            throw new InvalidOperationException("Cần ít nhất một ảnh nhận hàng (receipt_photo).");

        if (!hasVideo)
            throw new InvalidOperationException("Cần ít nhất một video bằng chứng (mở thùng / kiểm tra suất / tình trạng món).");

        if (string.Equals(complaint.Reason, ComplaintReason.MissingPortions, StringComparison.OrdinalIgnoreCase))
        {
            if (complaint.MissingPortionCount is not > 0)
                throw new InvalidOperationException("Vui lòng nhập số suất thiếu.");
        }
    }
}
