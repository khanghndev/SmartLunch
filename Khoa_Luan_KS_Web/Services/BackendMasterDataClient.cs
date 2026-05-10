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

    // --- Role & Permission ---
    public async Task<GetRolesResponse> GetRolesAsync(string accessToken, bool? isActive = null, CancellationToken ct = default)
    {
        var query = isActive.HasValue ? $"?isActive={isActive.Value}" : "";
        return await GetAsync<GetRolesResponse>($"/api/v1/master-data/Role{query}", accessToken, ct);
    }

    public async Task<GetPermissionsResponse> GetPermissionsAsync(string accessToken, bool? isActive = null, CancellationToken ct = default)
    {
        var query = isActive.HasValue ? $"?isActive={isActive.Value}" : "";
        return await GetAsync<GetPermissionsResponse>($"/api/v1/master-data/Permission{query}", accessToken, ct);
    }

    public async Task<GetRolePermissionsResponse> GetRolePermissionsAsync(int roleId, string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<GetRolePermissionsResponse>($"/api/v1/master-data/RolePermission?roleId={roleId}&page=1&pageSize=1000", accessToken, ct);
    }

    public async Task GrantPermissionToRoleAsync(int roleId, int permissionId, string accessToken, CancellationToken ct = default)
    {
        // Check if role permission already exists (active or inactive)
        var allPermissions = await GetRolePermissionsAsync(roleId, accessToken, ct);
        var existing = allPermissions.Items.FirstOrDefault(p => p.PermissionId == permissionId);

        if (existing != null)
        {
            if (!existing.IsActive)
            {
                // Update existing record to be active
                var updateRequest = new { Id = existing.Id, IsActive = true };
                var client = CreateClient(accessToken);
                var json = System.Text.Json.JsonSerializer.Serialize(updateRequest);
                using var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                using var res = await client.PutAsync($"/api/v1/master-data/RolePermission/{existing.Id}", content, ct);
                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsStringAsync(ct);
                    var msg = TryExtractBackendMessage(body) ?? $"Backend request failed ({(int)res.StatusCode})";
                    throw new InvalidOperationException(msg);
                }
            }
            return;
        }

        // Create new
        var request = new { RoleId = roleId, PermissionId = permissionId };
        await PostAsync<object>("/api/v1/master-data/RolePermission", request, accessToken, ct);
    }

    public async Task RevokePermissionFromRoleAsync(int roleId, int permissionId, string accessToken, CancellationToken ct = default)
    {
        var allPermissions = await GetRolePermissionsAsync(roleId, accessToken, ct);
        var existing = allPermissions.Items.FirstOrDefault(p => p.PermissionId == permissionId);

        if (existing != null)
        {
            var client = CreateClient(accessToken);
            using var res = await client.DeleteAsync($"/api/v1/master-data/RolePermission/{existing.Id}", ct);
            if (!res.IsSuccessStatusCode)
            {
                var body = await res.Content.ReadAsStringAsync(ct);
                var msg = TryExtractBackendMessage(body) ?? $"Backend request failed ({(int)res.StatusCode})";
                throw new InvalidOperationException(msg);
            }
        }
    }


    public async Task ResetPasswordAsync(int userId, string newPassword, string accessToken, CancellationToken ct)
    {
        await PostAsync<object>($"/api/v1/master-data/User/{userId}/reset-password", newPassword, accessToken, ct);
    }

    public async Task<AdminUserDto> CreateUserAsync(CreateUserRequest request, string accessToken, CancellationToken ct = default)
    {
        return await PostAsync<AdminUserDto>("/api/v1/master-data/User", request, accessToken, ct);
    }

    public async Task<AdminUserDto> UpdateUserAsync(int id, UpdateUserRequest request, string accessToken, CancellationToken ct = default)
    {
        return await PutAsync<AdminUserDto>($"/api/v1/master-data/User/{id}", request, accessToken, ct);
    }

    // --- System Management ---
    public async Task<GetSystemLogsResponse> GetSystemLogsAsync(string accessToken, int page = 1, int pageSize = 50, CancellationToken ct = default)
    {
        return await GetAsync<GetSystemLogsResponse>($"/api/v1/master-data/System/log?page={page}&pageSize={pageSize}", accessToken, ct);
    }

    public async Task<GetSystemBackupsResponse> GetSystemBackupsAsync(string accessToken, int page = 1, int pageSize = 10, bool includeDeleted = false, CancellationToken ct = default)
    {
        return await GetAsync<GetSystemBackupsResponse>($"/api/v1/master-data/System/backup?page={page}&pageSize={pageSize}&includeDeleted={includeDeleted}", accessToken, ct);
    }

    public async Task<SystemBackupDto> BackupSystemAsync(string accessToken, CancellationToken ct = default)
    {
        // Notice it's a POST, with empty body.
        return await PostAsync<SystemBackupDto>("/api/v1/master-data/System/backup", new { }, accessToken, ct);
    }

    public async Task<object> RestoreSystemAsync(RestoreSystemRequest request, string accessToken, CancellationToken ct = default)
    {
        return await PostAsync<object>("/api/v1/master-data/System/restore", request, accessToken, ct);
    }

    public async Task<object> DeleteSystemBackupAsync(int backupId, bool deleteFile, string accessToken, CancellationToken ct = default)
    {
        var client = CreateClient(accessToken);
        using var res = await client.DeleteAsync($"/api/v1/master-data/System/backup/{backupId}?deleteFile={deleteFile}", ct);
        return await HandleResponse<object>(res, ct);
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
        return await GetAsync<GetUnitsResponse>($"/api/v1/master-data/Organization{query}", accessToken, ct);
    }

    public async Task<GetUnitResponse> GetUnitAsync(int id, string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<GetUnitResponse>($"/api/v1/master-data/Organization/{id}", accessToken, ct);
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
    [JsonPropertyName("type")]
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

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public int? InitialRoleId { get; set; }
}

public class UpdateUserRequest
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public string? NewPassword { get; set; }
}

// ─── System Logs & Backups ──────────────────────────────────────────────────
public class SystemLogDto
{
    public int Id { get; set; }
    public DateTime? Timestamp { get; set; }
    public string? Level { get; set; }
    public string? Template { get; set; }
    public string? Message { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }
}

public class GetSystemLogsResponse : PaginationResponse<SystemLogDto> { }

public class SystemBackupDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RestoredAtUtc { get; set; }
    public bool IsDeleted { get; set; }
}

public class GetSystemBackupsResponse : PaginationResponse<SystemBackupDto> { }

public class RestoreSystemRequest
{
    public int BackupId { get; set; }
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

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public bool IsSystemRole { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PermissionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class RolePermissionDto
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int PermissionId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetRolesResponse : PaginationResponse<RoleDto>
{
}

public class GetPermissionsResponse : PaginationResponse<PermissionDto>
{
}

public class GetRolePermissionsResponse : PaginationResponse<RolePermissionDto>
{
}
