using System.Net.Http;
using SmartLunch.Backend.Service.Application.DTOs.Response.CompanyPublicDocuments;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.CompanyPublicDocuments;

public static class CompanyPublicDocumentMapper
{
    private static readonly Dictionary<string, string> TypeLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["food_safety"] = "Chứng nhận VSATTP / ATTP",
        ["iso"] = "ISO / HACCP",
        ["business_license"] = "Giấy phép kinh doanh",
        ["inspection"] = "Biên bản kiểm tra",
        ["contract"] = "Hợp đồng / Khung thỏa thuận",
        ["other"] = "Chứng từ khác",
    };

    public static string GetTypeLabel(string? documentType) =>
        TypeLabels.TryGetValue(documentType ?? "other", out var label) ? label : "Chứng từ";

    public static async Task<CompanyPublicDocumentDto> ToDtoAsync(
        CompanyPublicDocument doc,
        IStorageService storage,
        TimeSpan urlLifetime,
        CancellationToken cancellationToken = default)
    {
        var signed = await storage.CreateSignedUrlAsync(
            doc.MediaFile.ObjectName,
            HttpMethod.Get,
            doc.MediaFile.ContentType,
            urlLifetime);

        return new CompanyPublicDocumentDto
        {
            Id = doc.Id,
            Title = doc.Title,
            DocumentType = doc.DocumentType,
            DocumentTypeLabel = GetTypeLabel(doc.DocumentType),
            Description = doc.Description,
            FileUrl = signed.Url,
            ContentType = doc.MediaFile.ContentType,
            OriginalFileName = doc.MediaFile.OriginalFileName,
            SizeBytes = doc.MediaFile.SizeBytes,
            SortOrder = doc.SortOrder,
            IsPublished = doc.IsPublished,
            IssuedDate = doc.IssuedDate,
            ExpiryDate = doc.ExpiryDate,
            CreatedAt = doc.CreatedAt,
        };
    }
}
