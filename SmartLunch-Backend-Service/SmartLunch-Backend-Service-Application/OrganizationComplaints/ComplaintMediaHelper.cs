using SmartLunch.Backend.Service.Application.Constants;

namespace SmartLunch.Backend.Service.Application.OrganizationComplaints;

public static class ComplaintMediaHelper
{
    private static readonly HashSet<string> ImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp"
    };

    private static readonly HashSet<string> VideoContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "video/mp4", "video/quicktime"
    };

    public static (string MediaType, string Extension) Resolve(string contentType, long fileSize)
    {
        var ct = contentType ?? string.Empty;
        if (ImageContentTypes.Contains(ct))
        {
            if (fileSize > ComplaintMediaLimits.ImageMaxBytes)
                throw new ArgumentException($"Ảnh tối đa {ComplaintMediaLimits.ImageMaxBytes / (1024 * 1024)}MB.");
            var ext = ct.ToLowerInvariant() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => ".jpg"
            };
            return ("image", ext);
        }

        if (VideoContentTypes.Contains(ct))
        {
            if (fileSize > ComplaintMediaLimits.VideoMaxBytes)
                throw new ArgumentException($"Video tối đa {ComplaintMediaLimits.VideoMaxBytes / (1024 * 1024)}MB.");
            var ext = ct.Equals("video/quicktime", StringComparison.OrdinalIgnoreCase) ? ".mov" : ".mp4";
            return ("video", ext);
        }

        throw new ArgumentException("Định dạng file không hỗ trợ. Ảnh: JPEG/PNG/WebP. Video: MP4/MOV.");
    }
}
