using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Khoa_Luan_KS_Web.Services;

public class BackendCompanyProfileClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public BackendCompanyProfileClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public Task<OrganizationProfileClientResponse> GetProfileAsync(string accessToken, CancellationToken ct = default)
        => GetAsync<OrganizationProfileClientResponse>("/api/v1/company/profile", accessToken, ct);

    public Task<OrganizationProfileClientResponse> UpdateProfileAsync(
        UpdateOrganizationProfileClientRequest request,
        string accessToken,
        CancellationToken ct = default)
        => PutAsync<OrganizationProfileClientResponse>("/api/v1/company/profile", request, accessToken, ct);

    public async Task<OrganizationLegalDocumentClientDto> UploadDocumentAsync(
        IFormFile file,
        string accessToken,
        string title,
        string documentType,
        string? description = null,
        DateTime? issuedDate = null,
        DateTime? expiryDate = null,
        CancellationToken ct = default)
    {
        if (file == null || file.Length <= 0)
            throw new InvalidOperationException("File is required");

        var client = CreateClient(accessToken);
        using var form = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();
        using var fileContent = new StreamContent(stream);
        if (!string.IsNullOrWhiteSpace(file.ContentType))
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        form.Add(fileContent, "file", file.FileName);
        form.Add(new StringContent(title), "title");
        form.Add(new StringContent(documentType), "documentType");
        if (!string.IsNullOrWhiteSpace(description))
            form.Add(new StringContent(description), "description");
        if (issuedDate.HasValue)
            form.Add(new StringContent(issuedDate.Value.ToString("yyyy-MM-dd")), "issuedDate");
        if (expiryDate.HasValue)
            form.Add(new StringContent(expiryDate.Value.ToString("yyyy-MM-dd")), "expiryDate");

        using var res = await client.PostAsync("/api/v1/company/profile/documents", form, ct);
        return await HandleResponse<OrganizationLegalDocumentClientDto>(res, ct);
    }

    public async Task<string> UploadLogoAsync(IFormFile file, string accessToken, CancellationToken ct = default)
    {
        if (file == null || file.Length <= 0)
            throw new InvalidOperationException("File is required");

        var client = CreateClient(accessToken);
        using var form = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();
        using var fileContent = new StreamContent(stream);
        if (!string.IsNullOrWhiteSpace(file.ContentType))
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        form.Add(fileContent, "file", file.FileName);

        using var res = await client.PostAsync("/api/v1/company/profile/logo", form, ct);
        var data = await HandleResponse<UploadOrganizationLogoClientResponse>(res, ct);
        return data.LogoUrl;
    }

    public async Task DeleteDocumentAsync(int id, string accessToken, CancellationToken ct = default)
    {
        var client = CreateClient(accessToken);
        using var res = await client.DeleteAsync($"/api/v1/company/profile/documents/{id}", ct);
        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Backend failed ({(int)res.StatusCode}): {body}");
        }
    }

    private async Task<T> GetAsync<T>(string path, string accessToken, CancellationToken ct)
    {
        var client = CreateClient(accessToken);
        using var res = await client.GetAsync(path, ct);
        return await HandleResponse<T>(res, ct);
    }

    private async Task<T> PutAsync<T>(string path, object payload, string accessToken, CancellationToken ct)
    {
        var client = CreateClient(accessToken);
        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var res = await client.PutAsync(path, content, ct);
        return await HandleResponse<T>(res, ct);
    }

    private HttpClient CreateClient(string accessToken)
    {
        var baseUrl = _configuration["BackendApi:BaseUrl"]?.TrimEnd('/')
            ?? throw new InvalidOperationException("BackendApi:BaseUrl is not configured");
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private static Task<T> HandleResponse<T>(HttpResponseMessage res, CancellationToken ct) =>
        BackendApiAuthHelper.HandleResponseAsync<T>(res, ct);
}

public sealed class OrganizationProfileClientResponse
{
    public int OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TaxCode { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? ContactPerson { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Website { get; set; }
    public string UnitType { get; set; } = "Office";
    public bool IsActive { get; set; }
    public string? LogoUrl { get; set; }
    public List<OrganizationLegalDocumentClientDto> Documents { get; set; } = new();
}

public sealed class UploadOrganizationLogoClientResponse
{
    public string LogoUrl { get; set; } = string.Empty;
}

public sealed class OrganizationLegalDocumentClientDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string DocumentType { get; set; } = "other";
    public string DocumentTypeLabel { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public string? OriginalFileName { get; set; }
    public long SizeBytes { get; set; }
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class UpdateOrganizationProfileClientRequest
{
    public string Name { get; set; } = string.Empty;
    public string? TaxCode { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? ContactPerson { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ContactEmail { get; set; }
    public string? Website { get; set; }
}
