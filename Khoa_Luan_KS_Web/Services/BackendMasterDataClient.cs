using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Khoa_Luan_KS_Web.Services;

public class BackendMasterDataClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public BackendMasterDataClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<AdminGetUsersResponse> GetUsersAsync(string accessToken, int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default)
    {
        var query = $"?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(searchTerm)) query += $"&searchTerm={searchTerm}";
        
        return await GetAsync<AdminGetUsersResponse>($"/api/v1/master-data/User{query}", accessToken, ct);
    }

    public async Task LockUserAsync(int userId, string accessToken, CancellationToken ct)
    {
        await PostAsync<object>($"/api/v1/master-data/User/{userId}/lock", new { }, accessToken, ct);
    }

    public async Task UnlockUserAsync(int userId, string accessToken, CancellationToken ct)
    {
        await PostAsync<object>($"/api/v1/master-data/User/{userId}/unlock", new { }, accessToken, ct);
    }

    public async Task ResetPasswordAsync(int userId, string newPassword, string accessToken, CancellationToken ct)
    {
        await PostAsync<object>($"/api/v1/master-data/User/{userId}/reset-password", newPassword, accessToken, ct);
    }

    public async Task<GetPartnersResponse> GetPartnersAsync(string accessToken, int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default)
    {
        var query = $"?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(searchTerm)) query += $"&searchTerm={searchTerm}";

        return await GetAsync<GetPartnersResponse>($"/api/v1/master-data/Partner{query}", accessToken, ct);
    }

    public async Task<GetPartnerResponse> GetPartnerAsync(int id, string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<GetPartnerResponse>($"/api/v1/master-data/Partner/{id}", accessToken, ct);
    }

    public async Task<GetUnitsResponse> GetUnitsAsync(string accessToken, int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken ct = default)
    {
        var query = $"?Page={page}&PageSize={pageSize}";
        if (!string.IsNullOrEmpty(searchTerm)) query += $"&SearchTerm={Uri.EscapeDataString(searchTerm)}";
        return await GetAsync<GetUnitsResponse>($"/api/v1/master-data/Unit{query}", accessToken, ct);
    }

    public async Task<GetUnitResponse> GetUnitAsync(int id, string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<GetUnitResponse>($"/api/v1/master-data/Unit/{id}", accessToken, ct);
    }

    public async Task<PartnerDto> CreatePartnerAsync(CreatePartnerRequest payload, string accessToken, CancellationToken ct = default)
    {
        return await PostAsync<PartnerDto>("/api/v1/master-data/Partner", payload, accessToken, ct);
    }

    public async Task<GetDishesResponse> GetDishesAsync(string accessToken, int page = 1, int pageSize = 20, string? searchTerm = null, string? category = null, CancellationToken ct = default)
    {
        var query = $"?Page={page}&PageSize={pageSize}";
        if (!string.IsNullOrEmpty(searchTerm)) query += $"&SearchTerm={Uri.EscapeDataString(searchTerm)}";
        if (!string.IsNullOrEmpty(category)) query += $"&Category={Uri.EscapeDataString(category)}";
        return await GetAsync<GetDishesResponse>($"/api/v1/master-data/Dish{query}", accessToken, ct);
    }

    public async Task<DishDetailResponse> GetDishAsync(int id, string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<DishDetailResponse>($"/api/v1/master-data/Dish/{id}?includeIngredientQuotas=true", accessToken, ct);
    }

    public async Task<DishDetailDto> CreateDishAsync(CreateDishRequest payload, string accessToken, CancellationToken ct = default)
    {
        return await PostAsync<DishDetailDto>("/api/v1/master-data/Dish", payload, accessToken, ct);
    }

    public async Task<DishDetailResponse> UpdateDishAsync(int id, UpdateDishRequest payload, string accessToken, CancellationToken ct = default)
    {
        return await PutAsync<DishDetailResponse>($"/api/v1/master-data/Dish/{id}", payload, accessToken, ct);
    }

    public async Task<MediaUploadResponse> UploadMediaAsync(
        IFormFile file,
        string accessToken,
        string mediaType = "image",
        string? purpose = null,
        bool isPublic = true,
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
        form.Add(new StringContent(mediaType), "mediaType");
        if (!string.IsNullOrWhiteSpace(purpose))
            form.Add(new StringContent(purpose), "purpose");
        form.Add(new StringContent(isPublic ? "true" : "false"), "isPublic");

        using var res = await client.PostAsync("/api/v1/Media/upload", form, ct);
        return await HandleResponse<MediaUploadResponse>(res, ct);
    }

    public async Task<int> AddDishImageAsync(int dishId, int mediaFileId, string role, int? sortOrder, string accessToken, CancellationToken ct = default)
    {
        var payload = new AddDishImageRequest
        {
            MediaFileId = mediaFileId,
            Role = role,
            SortOrder = sortOrder
        };

        var res = await PostAsync<AddDishImageResponse>($"/api/v1/master-data/Dish/{dishId}/images", payload, accessToken, ct);
        return res.Id;
    }

    private async Task<T> GetAsync<T>(string path, string accessToken, CancellationToken ct)
    {
        var client = CreateClient(accessToken);
        using var res = await client.GetAsync(path, ct);
        return await HandleResponse<T>(res, ct);
    }

    private async Task<T> PostAsync<T>(string path, object payload, string accessToken, CancellationToken ct)
    {
        var client = CreateClient(accessToken);
        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var res = await client.PostAsync(path, content, ct);
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
        var baseUrl = _configuration["BackendApi:BaseUrl"]?.TrimEnd('/');
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl!);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage res, CancellationToken ct)
    {
        var body = await res.Content.ReadAsStringAsync(ct);
        if (!res.IsSuccessStatusCode)
        {
            var msg = TryExtractBackendMessage(body) ?? $"Backend request failed ({(int)res.StatusCode})";
            throw new InvalidOperationException(msg);
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var envelope = JsonSerializer.Deserialize<BaseApiResponse<T>>(body, options);
        if (envelope == null || envelope.Data == null)
        {
            if (typeof(T) == typeof(object)) return (T)(object)new { };
            throw new InvalidOperationException("Invalid response from backend");
        }
        return envelope.Data;
    }

    private static string? TryExtractBackendMessage(string body)
    {
        try
        {
            var doc = JsonDocument.Parse(body);
            string? finalMsg = null;
            if (doc.RootElement.TryGetProperty("message", out var msg) && msg.ValueKind == JsonValueKind.String)
                finalMsg = msg.GetString();
            
            if (doc.RootElement.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0)
            {
                var errorList = errors.EnumerateArray().Select(e => e.GetString()).Where(e => !string.IsNullOrEmpty(e));
                var errorStr = string.Join(" | ", errorList);
                if (!string.IsNullOrEmpty(errorStr))
                    finalMsg = (finalMsg == null ? errorStr : $"{finalMsg} ({errorStr})");
            }
            return finalMsg;
        }
        catch { return null; }
    }
}

public class GetPartnerResponse
{
    public PartnerDto Partner { get; set; } = new();
    public List<PartnerContractSummaryDto> Contracts { get; set; } = new();
}

public class PartnerContractSummaryDto
{
    public int Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
}


public class UnitDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ContactPerson { get; set; }
    public string? TaxCode { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? LogoUrl { get; set; }
    public string UnitType { get; set; } = "Office";
    public bool IsSubscriptionActive { get; set; }
    public int DefaultDailyMeals { get; set; }
    public bool IsActive { get; set; }
}

public class GetUnitsResponse : PaginationResponse<UnitDto> { }

public class GetUnitResponse
{
    public UnitDto Unit { get; set; } = new();
}

public class PartnerDto
{
    public int Id { get; set; }
    public string LegalName { get; set; } = string.Empty;
    public string? BusinessRegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal? PerformanceRating { get; set; }
    public bool IsActive { get; set; }
}

public class CreatePartnerRequest
{
    public string LegalName { get; set; } = string.Empty;
    public string? BusinessRegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal? PerformanceRating { get; set; }
    public string? ComplianceInfo { get; set; }
    public string? FinancialTerms { get; set; }
}

public class GetPartnersResponse : PaginationResponse<PartnerDto> { }

public class AdminGetUsersResponse : PaginationResponse<AdminUserDto> { }

public class AdminUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> RoleNames { get; set; } = new();
}

// ─── Dish / Menu DTOs ───────────────────────────────────────────────────────
public class DishDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public string? DietaryLabel { get; set; }
    public string? ImageUrl { get; set; }
    public List<DishImageDto> Images { get; set; } = new();
    public decimal? Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Fat { get; set; }
    public decimal? Carbs { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class DishImageDto
{
    public int Id { get; set; }
    public int MediaFileId { get; set; }
    public string Role { get; set; } = "gallery";
    public int SortOrder { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class DishDetailDto : DishDto { }

public class GetDishesResponse : PaginationResponse<DishDto> { }

public class DishIngredientQuotaDto
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
}

public class DishDetailResponse
{
    public DishDto Dish { get; set; } = new();
    public List<DishIngredientQuotaDto> IngredientQuotas { get; set; } = new();
}

public class CreateDishRequest
{
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public string? DietaryLabel { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Fat { get; set; }
    public decimal? Carbs { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateDishRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public string? DietaryLabel { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Fat { get; set; }
    public decimal? Carbs { get; set; }
    public bool IsActive { get; set; } = true;

    public List<UpdateDishImageItemRequest>? Images { get; set; }
}

public class UpdateDishImageItemRequest
{
    public int MediaFileId { get; set; }
    public string Role { get; set; } = "gallery";
    public int? SortOrder { get; set; }
}

public sealed class UploadImageResponse
{
    public string ObjectName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}

public sealed class MediaUploadResponse
{
    public int MediaFileId { get; set; }
    public string ObjectName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public bool IsPublic { get; set; }
    public string Url { get; set; } = string.Empty;
}

public sealed class AddDishImageRequest
{
    public int MediaFileId { get; set; }
    public string Role { get; set; } = "gallery";
    public int? SortOrder { get; set; }
}

public sealed class AddDishImageResponse
{
    public int Id { get; set; }
}
