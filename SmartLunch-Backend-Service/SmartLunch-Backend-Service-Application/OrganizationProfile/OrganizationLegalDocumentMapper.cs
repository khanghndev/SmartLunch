using System.Net.Http;
using SmartLunch.Backend.Service.Application.DTOs.Response.CompanyProfile;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using SmartLunch.Backend.Service.Domain.Entities;

namespace SmartLunch.Backend.Service.Application.OrganizationProfile;

public static class OrganizationLegalDocumentMapper
{
    private static readonly Dictionary<string, string> TypeLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["business_license"] = "Giấy đăng ký kinh doanh",
        ["tax"] = "Giấy xác nhận mã số thuế",
        ["authorization"] = "Giấy ủy quyền / Quyết định bổ nhiệm",
        ["other"] = "Tài liệu khác",
    };

    public static string GetTypeLabel(string? documentType) =>
        TypeLabels.TryGetValue(documentType ?? "other", out var label) ? label : "Tài liệu";

    public static async Task<OrganizationLegalDocumentDto> ToDtoAsync(
        OrganizationLegalDocument doc,
        IStorageService storage,
        TimeSpan urlLifetime,
        CancellationToken cancellationToken = default)
    {
        var signed = await storage.CreateSignedUrlAsync(
            doc.MediaFile.ObjectName,
            HttpMethod.Get,
            doc.MediaFile.ContentType,
            urlLifetime);

        return new OrganizationLegalDocumentDto
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
            IssuedDate = doc.IssuedDate,
            ExpiryDate = doc.ExpiryDate,
            CreatedAt = doc.CreatedAt,
        };
    }
}
