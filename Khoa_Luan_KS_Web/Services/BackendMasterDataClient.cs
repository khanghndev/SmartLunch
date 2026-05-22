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

    public async Task<AdminGetUsersResponse> GetUsersAsync(
        string accessToken,
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? roleName = null,
        bool? staffOnly = null,
        CancellationToken ct = default)
    {
        var query = $"?Page={page}&PageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchTerm)) query += $"&SearchTerm={Uri.EscapeDataString(searchTerm)}";
        if (!string.IsNullOrWhiteSpace(roleName)) query += $"&RoleName={Uri.EscapeDataString(roleName)}";
        if (staffOnly == true) query += "&StaffOnly=true";

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
                var updateRequest = new { id = existing.Id, isActive = true };
                var client = CreateClient(accessToken);
                var json = JsonSerializer.Serialize(updateRequest, JsonPostOptions);
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

    public async Task<GetSystemBackupsResponse> GetSystemBackupsAsync(
        string accessToken,
        int page = 1,
        int pageSize = 10,
        bool includeDeleted = false,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken ct = default)
    {
        var q = $"/api/v1/master-data/System/backup?page={page}&pageSize={pageSize}&includeDeleted={includeDeleted}";
        if (from.HasValue) q += $"&from={Uri.EscapeDataString(from.Value.ToString("o"))}";
        if (to.HasValue) q += $"&to={Uri.EscapeDataString(to.Value.ToString("o"))}";
        return await GetAsync<GetSystemBackupsResponse>(q, accessToken, ct);
    }

    public async Task<BackupScheduleDto> GetBackupScheduleAsync(string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<BackupScheduleDto>("/api/v1/master-data/System/backup/schedule", accessToken, ct);
    }

    public async Task<BackupScheduleDto> UpdateBackupScheduleAsync(UpdateBackupScheduleRequest request, string accessToken, CancellationToken ct = default)
    {
        return await PutAsync<BackupScheduleDto>("/api/v1/master-data/System/backup/schedule", request, accessToken, ct);
    }

    public async Task<string> GetSystemBackupDownloadUrlAsync(int backupId, string accessToken, CancellationToken ct = default)
    {
        var res = await GetAsync<SystemBackupDownloadResponse>($"/api/v1/master-data/System/backup/{backupId}/url", accessToken, ct);
        return res.DownloadUrl;
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

    public async Task<GetUnitsResponse> GetUnitsAsync(string accessToken, int page = 1, int pageSize = 10, string? searchTerm = null, bool? isActive = null, CancellationToken ct = default)
    {
        var query = $"?Page={page}&PageSize={pageSize}";
        if (!string.IsNullOrEmpty(searchTerm)) query += $"&SearchTerm={Uri.EscapeDataString(searchTerm)}";
        if (isActive.HasValue) query += $"&IsActive={(isActive.Value ? "true" : "false")}";
        return await GetAsync<GetUnitsResponse>($"/api/v1/master-data/Organization{query}", accessToken, ct);
    }

    public async Task<GetUnitResponse> GetUnitAsync(int id, string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<GetUnitResponse>($"/api/v1/master-data/Organization/{id}", accessToken, ct);
    }

    public async Task<GetUnitResponse> UpdateOrganizationStatusAsync(int id, bool isActive, string accessToken, CancellationToken ct = default)
    {
        return await PatchAsync<GetUnitResponse>($"/api/v1/master-data/Organization/{id}/status", new { isActive }, accessToken, ct);
    }

    public async Task<PartnerDto> CreatePartnerAsync(CreatePartnerRequest payload, string accessToken, CancellationToken ct = default)
    {
        return await PostAsync<PartnerDto>("/api/v1/master-data/Partner", payload, accessToken, ct);
    }

    // ─── Contracts (manager / master-data) ─────────────────────────────────────
    public async Task<GetContractsClientResponse> GetContractsAsync(
        string accessToken,
        int page = 1,
        int pageSize = 12,
        string? searchTerm = null,
        int? partnerId = null,
        CancellationToken ct = default)
    {
        var query = $"?Page={page}&PageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchTerm)) query += $"&SearchTerm={Uri.EscapeDataString(searchTerm)}";
        if (partnerId.HasValue) query += $"&PartnerId={partnerId.Value}";
        return await GetAsync<GetContractsClientResponse>($"/api/v1/master-data/Contract{query}", accessToken, ct);
    }

    public async Task<GetManagerContractResponse> GetManagerContractAsync(int id, string accessToken, CancellationToken ct = default)
        => await GetAsync<GetManagerContractResponse>($"/api/v1/master-data/Contract/{id}", accessToken, ct);

    public async Task<GetManagerContractResponse> CreateContractAsync(CreateContractClientRequest payload, string accessToken, CancellationToken ct = default)
        => await PostAsync<GetManagerContractResponse>("/api/v1/master-data/Contract", payload, accessToken, ct);

    // ─── Finance (công nợ / thu chi) ─────────────────────────────────────────────
    public async Task<GetOrganizationReceivablesClientResponse> GetOrganizationReceivablesAsync(
        string accessToken,
        bool onlyWithOutstanding = true,
        int? organizationId = null,
        CancellationToken ct = default)
    {
        var query = $"?OnlyWithOutstanding={(onlyWithOutstanding ? "true" : "false")}";
        if (organizationId.HasValue) query += $"&OrganizationId={organizationId.Value}";
        return await GetAsync<GetOrganizationReceivablesClientResponse>($"/api/v1/finance/organization-receivables{query}", accessToken, ct);
    }

    public async Task<GetPaymentHistoryClientResponse> GetPaymentHistoryAsync(
        string accessToken,
        DateOnly from,
        DateOnly to,
        string scope = "Customer",
        int? organizationId = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = $"?From={from:yyyy-MM-dd}&To={to:yyyy-MM-dd}&Scope={Uri.EscapeDataString(scope)}&Page={page}&PageSize={pageSize}";
        if (organizationId.HasValue) query += $"&OrganizationId={organizationId.Value}";
        return await GetAsync<GetPaymentHistoryClientResponse>($"/api/v1/finance/payment-history{query}", accessToken, ct);
    }

    public async Task<GetDishesResponse> GetDishesAsync(string accessToken, int page = 1, int pageSize = 20, string? searchTerm = null, string? category = null, bool? isActive = null, CancellationToken ct = default)
    {
        var query = $"?Page={page}&PageSize={pageSize}";
        if (!string.IsNullOrEmpty(searchTerm)) query += $"&SearchTerm={Uri.EscapeDataString(searchTerm)}";
        if (!string.IsNullOrEmpty(category)) query += $"&Category={Uri.EscapeDataString(category)}";
        if (isActive.HasValue) query += $"&IsActive={isActive.Value.ToString().ToLowerInvariant()}";
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

    // --- WeeklyMenu ---
    public async Task<GetWeeklyMenusClientResponse> GetWeeklyMenusAsync(string accessToken, int page = 1, int pageSize = 10, string? searchTerm = null, int? customerTypeId = null, string? customerProfileKey = null, CancellationToken ct = default)
    {
        var query = $"?Page={page}&PageSize={pageSize}";
        if (!string.IsNullOrEmpty(searchTerm)) query += $"&SearchTerm={Uri.EscapeDataString(searchTerm)}";
        if (customerTypeId.HasValue) query += $"&CustomerTypeId={customerTypeId.Value}";
        if (!string.IsNullOrWhiteSpace(customerProfileKey)) query += $"&CustomerProfileKey={Uri.EscapeDataString(customerProfileKey)}";
        return await GetAsync<GetWeeklyMenusClientResponse>($"/api/v1/master-data/WeeklyMenu{query}", accessToken, ct);
    }

    public async Task<GetCustomerTypesClientResponse> GetCustomerTypesAsync(string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<GetCustomerTypesClientResponse>("/api/v1/master-data/customer-types", accessToken, ct);
    }

    public async Task<GetOrdersClientResponse> GetOrdersAsync(
        string accessToken,
        int page = 1,
        int pageSize = 20,
        string? status = null,
        string? searchTerm = null,
        DateOnly? scheduledOn = null,
        CancellationToken ct = default)
    {
        var q = $"?Page={page}&PageSize={pageSize}";
        if (!string.IsNullOrEmpty(status)) q += $"&Status={Uri.EscapeDataString(status)}";
        if (!string.IsNullOrWhiteSpace(searchTerm)) q += $"&SearchTerm={Uri.EscapeDataString(searchTerm)}";
        if (scheduledOn.HasValue) q += $"&ScheduledOn={scheduledOn.Value:yyyy-MM-dd}";
        return await GetAsync<GetOrdersClientResponse>($"/api/v1/master-data/Order{q}", accessToken, ct);
    }

    public async Task<GetOrderClientResponse> UpdateOrderStatusAsync(int orderId, string status, string accessToken, CancellationToken ct = default)
    {
        return await PatchAsync<GetOrderClientResponse>(
            $"/api/v1/master-data/Order/{orderId}/status",
            new { status },
            accessToken,
            ct);
    }

    public async Task<GetOrderClientResponse> GetOrderAsync(int id, string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<GetOrderClientResponse>($"/api/v1/master-data/Order/{id}", accessToken, ct);
    }

    public async Task<GetOrderClientResponse> CreateCustomerMealOrderAsync(CreateCustomerMealOrderApiRequest request, string accessToken, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var client = CreateClient(accessToken);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var res = await client.PostAsync("/api/v1/master-data/Order/customer", content, ct);
        return await HandleResponse<GetOrderClientResponse>(res, ct);
    }

    public async Task<byte[]> GetOrderAnnexPreviewPdfAsync(int orderId, string accessToken, CancellationToken ct = default)
    {
        var client = CreateClient(accessToken);
        using var res = await client.GetAsync($"/api/v1/master-data/Order/{orderId}/annex-preview", ct);
        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Không tải được preview PDF phụ lục ({(int)res.StatusCode}): {body}");
        }

        return await res.Content.ReadAsByteArrayAsync(ct);
    }

    public async Task<GetOrderClientResponse> SignOrderAnnexAsync(int orderId, SignOrderAnnexApiRequest request, string accessToken, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var client = CreateClient(accessToken);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var res = await client.PostAsync($"/api/v1/master-data/Order/{orderId}/sign-annex", content, ct);
        return await HandleResponse<GetOrderClientResponse>(res, ct);
    }

    public async Task<GetWeeklyMenuClientResponse> GetWeeklyMenuAsync(int id, string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<GetWeeklyMenuClientResponse>($"/api/v1/master-data/WeeklyMenu/{id}", accessToken, ct);
    }

    public async Task<GetWeeklyMenuDetailClientResponse> GetWeeklyMenuDetailAsync(int id, string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<GetWeeklyMenuDetailClientResponse>($"/api/v1/master-data/WeeklyMenu/{id}/detail", accessToken, ct);
    }

    /// <summary>Danh sách hợp đồng theo tổ chức (role Company / Organization).</summary>
    public async Task<GetMyOrganizationContractsClientResponse> GetMyOrganizationContractsAsync(string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<GetMyOrganizationContractsClientResponse>("/api/v1/company/contracts", accessToken, ct);
    }

    public async Task<GetContractClientResponse> GetCompanyContractAsync(int contractId, string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<GetContractClientResponse>($"/api/v1/company/contracts/{contractId}", accessToken, ct);
    }

    public async Task<GetContractClientResponse> SignCompanyContractAsync(int contractId, SignCompanyContractRequest request, string accessToken, CancellationToken ct = default)
    {
        return await PostAsync<GetContractClientResponse>($"/api/v1/company/contracts/{contractId}/sign", request, accessToken, ct);
    }

    // --- Promotions ---
    public async Task<GetPromotionsClientResponse> GetPromotionsAsync(
        string accessToken, int page = 1, int pageSize = 20, string? searchTerm = null, bool? isActive = null, string? scopeType = null, CancellationToken ct = default)
    {
        var q = $"?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchTerm)) q += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
        if (isActive.HasValue) q += $"&isActive={isActive.Value.ToString().ToLowerInvariant()}";
        if (!string.IsNullOrWhiteSpace(scopeType)) q += $"&scopeType={Uri.EscapeDataString(scopeType)}";
        return await GetAsync<GetPromotionsClientResponse>($"/api/v1/master-data/Promotion{q}", accessToken, ct);
    }

    public async Task<GetPromotionClientResponse> GetPromotionAsync(int id, string accessToken, CancellationToken ct = default)
        => await GetAsync<GetPromotionClientResponse>($"/api/v1/master-data/Promotion/{id}", accessToken, ct);

    public async Task<GetPromotionClientResponse> CreatePromotionAsync(UpsertPromotionClientRequest request, string accessToken, CancellationToken ct = default)
        => await PostAsync<GetPromotionClientResponse>("/api/v1/master-data/Promotion", request, accessToken, ct);

    public async Task<GetPromotionClientResponse> UpdatePromotionAsync(int id, UpsertPromotionClientRequest request, string accessToken, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var client = CreateClient(accessToken);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var res = await client.PutAsync($"/api/v1/master-data/Promotion/{id}", content, ct);
        return await HandleResponse<GetPromotionClientResponse>(res, ct);
    }

    public async Task DeletePromotionAsync(int id, string accessToken, CancellationToken ct = default)
    {
        var client = CreateClient(accessToken);
        using var res = await client.DeleteAsync($"/api/v1/master-data/Promotion/{id}", ct);
        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(ct);
            var msg = TryExtractBackendMessage(body) ?? $"Backend request failed ({(int)res.StatusCode})";
            throw new InvalidOperationException(msg);
        }
    }

    public async Task<PreviewPromotionClientResponse> PreviewPromotionAsync(PreviewPromotionClientRequest request, string accessToken, CancellationToken ct = default)
        => await PostAsync<PreviewPromotionClientResponse>("/api/v1/organization/meal-order/promotions/preview", request, accessToken, ct);

    public async Task<ListEligiblePromotionsClientResponse> ListEligiblePromotionsAsync(
        PreviewPromotionClientRequest request,
        string accessToken,
        CancellationToken ct = default)
        => await PostAsync<ListEligiblePromotionsClientResponse>("/api/v1/organization/meal-order/promotions/eligible", request, accessToken, ct);

    // --- Organization meal order (B2B đặt suất theo đơn vị) ---
    public async Task<GetOrganizationDishCategoriesClientResponse> GetOrganizationDishCategoriesAsync(string accessToken, CancellationToken ct = default)
    {
        return await GetAsync<GetOrganizationDishCategoriesClientResponse>("/api/v1/organization/meal-order/dish-category", accessToken, ct);
    }

    public async Task<GetOrganizationDishesByCategoryClientResponse> GetOrganizationDishesByCategoryAsync(
        int categoryId, string accessToken, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        return await GetAsync<GetOrganizationDishesByCategoryClientResponse>(
            $"/api/v1/organization/meal-order/dish/category?categoryId={categoryId}&page={page}&pageSize={pageSize}",
            accessToken, ct);
    }

    public async Task<PrepareOrganizationMealContractClientResponse> PrepareOrganizationMealContractAsync(
        PrepareOrganizationMealContractClientRequest request, string accessToken, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var client = CreateClient(accessToken);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var res = await client.PostAsync("/api/v1/organization/meal-order/contract", content, ct);
        return await HandleResponse<PrepareOrganizationMealContractClientResponse>(res, ct);
    }

    public async Task<CheckoutOrganizationMealClientResponse> CheckoutOrganizationMealAsync(
        CheckoutOrganizationMealClientRequest request, string accessToken, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var client = CreateClient(accessToken);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var res = await client.PostAsync("/api/v1/organization/meal-order/checkout", content, ct);
        return await HandleResponse<CheckoutOrganizationMealClientResponse>(res, ct);
    }

    public async Task<InitiateOrganizationMealPaymentClientResponse> InitiateOrganizationMealPaymentAsync(
        InitiateOrganizationMealPaymentClientRequest request, string accessToken, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var client = CreateClient(accessToken);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var res = await client.PostAsync("/api/v1/organization/meal-order/pay", content, ct);
        return await HandleResponse<InitiateOrganizationMealPaymentClientResponse>(res, ct);
    }

    // --- Company public documents (hồ sơ công khai / Giới thiệu) ---
    public async Task<CompanyPublicDocumentListClientResponse> GetPublicCompanyDocumentsAsync(CancellationToken ct = default)
    {
        var client = CreateAnonymousClient();
        using var res = await client.GetAsync("/api/v1/company-public-documents/public", ct);
        return await HandleResponse<CompanyPublicDocumentListClientResponse>(res, ct);
    }

    public async Task<CompanyPublicDocumentListClientResponse> GetCompanyDocumentsAsync(string accessToken, CancellationToken ct = default)
        => await GetAsync<CompanyPublicDocumentListClientResponse>("/api/v1/company-public-documents", accessToken, ct);

    public async Task<CompanyPublicDocumentClientDto> UploadCompanyDocumentAsync(
        IFormFile file,
        string accessToken,
        string title,
        string documentType,
        string? description = null,
        DateTime? issuedDate = null,
        DateTime? expiryDate = null,
        bool isPublished = true,
        int sortOrder = 0,
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
        form.Add(new StringContent(isPublished ? "true" : "false"), "isPublished");
        form.Add(new StringContent(sortOrder.ToString()), "sortOrder");

        using var res = await client.PostAsync("/api/v1/company-public-documents", form, ct);
        return await HandleResponse<CompanyPublicDocumentClientDto>(res, ct);
    }

    public async Task<CompanyPublicDocumentClientDto> UpdateCompanyDocumentAsync(
        int id,
        UpdateCompanyPublicDocumentClientRequest request,
        string accessToken,
        CancellationToken ct = default)
        => await PutAsync<CompanyPublicDocumentClientDto>($"/api/v1/company-public-documents/{id}", request, accessToken, ct);

    public async Task DeleteCompanyDocumentAsync(int id, string accessToken, CancellationToken ct = default)
    {
        var client = CreateClient(accessToken);
        using var res = await client.DeleteAsync($"/api/v1/company-public-documents/{id}", ct);
        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(ct);
            var msg = TryExtractBackendMessage(body) ?? $"Backend request failed ({(int)res.StatusCode})";
            throw new InvalidOperationException(msg);
        }
    }

    private HttpClient CreateAnonymousClient()
    {
        var baseUrl = _configuration["BackendApi:BaseUrl"]?.TrimEnd('/');
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl!);
        return client;
    }

    // --- Customer reviews / feedback ---
    public async Task<GetPublicReviewsClientResponse> GetPublicReviewsAsync(int page = 1, int pageSize = 50, CancellationToken ct = default)
    {
        var client = CreateAnonymousClient();
        using var res = await client.GetAsync($"/api/v1/customer-reviews/public?page={page}&pageSize={pageSize}", ct);
        return await HandleResponse<GetPublicReviewsClientResponse>(res, ct);
    }

    public async Task<GetReviewMeContextClientResponse> GetReviewMeContextAsync(string accessToken, CancellationToken ct = default)
        => await GetAsync<GetReviewMeContextClientResponse>("/api/v1/customer-reviews/me", accessToken, ct);

    public async Task<CreateCustomerReviewClientResponse> CreateCustomerReviewAsync(
        CreateCustomerReviewClientRequest request,
        string accessToken,
        CancellationToken ct = default)
        => await PostAsync<CreateCustomerReviewClientResponse>("/api/v1/customer-reviews", request, accessToken, ct);

    public async Task<GetManagerReviewsClientResponse> GetManagerReviewsAsync(
        string accessToken,
        int page = 1,
        int pageSize = 20,
        string? searchTerm = null,
        int? maxRating = null,
        CancellationToken ct = default)
    {
        var q = $"?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchTerm)) q += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
        if (maxRating.HasValue) q += $"&maxRating={maxRating.Value}";
        return await GetAsync<GetManagerReviewsClientResponse>($"/api/v1/customer-reviews/manager{q}", accessToken, ct);
    }

    public async Task<ManagerReviewListItemClientDto> ReplyToReviewAsync(
        int reviewId,
        string reply,
        string accessToken,
        CancellationToken ct = default)
        => await PostAsync<ManagerReviewListItemClientDto>(
            $"/api/v1/customer-reviews/{reviewId}/reply",
            new { reply },
            accessToken,
            ct);

    private async Task<T> GetAsync<T>(string path, string accessToken, CancellationToken ct)
    {
        var client = CreateClient(accessToken);
        using var res = await client.GetAsync(path, ct);
        return await HandleResponse<T>(res, ct);
    }

    private static readonly JsonSerializerOptions JsonPostOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private async Task<T> PostAsync<T>(string path, object payload, string accessToken, CancellationToken ct)
    {
        var client = CreateClient(accessToken);
        var json = JsonSerializer.Serialize(payload, JsonPostOptions);
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

    private async Task<T> PatchAsync<T>(string path, object payload, string accessToken, CancellationToken ct)
    {
        var client = CreateClient(accessToken);
        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Patch, path) { Content = content };
        using var res = await client.SendAsync(request, ct);
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

public class GetPublicReviewsClientResponse
{
    public List<PublicReviewClientDto> Reviews { get; set; } = new();
    public double AverageRating { get; set; }
    public int TotalCount { get; set; }
}

public class PublicReviewClientDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public string? OrderCode { get; set; }
    public string? ManagerReply { get; set; }
    public DateTime? RepliedAt { get; set; }
}

public class GetReviewMeContextClientResponse
{
    public bool CanSubmitReview { get; set; }
    public bool IsEnterpriseMember { get; set; }
    public string? Message { get; set; }
    public List<ReviewableOrderClientDto> ReviewableOrders { get; set; } = new();
}

public class ReviewableOrderClientDto
{
    public int OrderId { get; set; }
    public string? OrderCode { get; set; }
    public string? InvoiceCode { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public bool AlreadyReviewed { get; set; }
}

public class CreateCustomerReviewClientRequest
{
    public int OrderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class CreateCustomerReviewClientResponse
{
    public object? Review { get; set; }
}

public class GetManagerReviewsClientResponse
{
    public List<ManagerReviewListItemClientDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public double AverageRating { get; set; }
}

public class ManagerReviewListItemClientDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public string? OrderCode { get; set; }
    public string? InvoiceCode { get; set; }
    public string? ManagerReply { get; set; }
    public DateTime? RepliedAt { get; set; }
    public bool IsReplied { get; set; }
}

public class CompanyPublicDocumentListClientResponse
{
    public List<CompanyPublicDocumentClientDto> Documents { get; set; } = new();
}

public class CompanyPublicDocumentClientDto
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
    public int SortOrder { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsExpired { get; set; }
}

public class UpdateCompanyPublicDocumentClientRequest
{
    public string? Title { get; set; }
    public string? DocumentType { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsPublished { get; set; }
    public DateTime? IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
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
    public string? ContactEmail { get; set; }
    public string? TaxCode { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public string? EducationLevel { get; set; }
    [JsonPropertyName("type")]
    public string UnitType { get; set; } = "Office";
    public bool IsSubscriptionActive { get; set; }
    public int DefaultDailyMeals { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class GetUnitsResponse : PaginationResponse<UnitDto> { }

public class GetUnitResponse
{
    [JsonPropertyName("organization")]
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
    public string StorageBucket { get; set; } = string.Empty;
    public string StorageObjectName { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string BackupSource { get; set; } = "Manual";
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RestoredAtUtc { get; set; }
    public bool IsDeleted { get; set; }
}

public class GetSystemBackupsResponse : PaginationResponse<SystemBackupDto> { }

public class BackupScheduleDto
{
    public bool IsEnabled { get; set; }
    public string ScheduleMode { get; set; } = "Daily";
    public string TimeOfDay { get; set; } = "03:00";
    public int? DayOfWeek { get; set; }
    public DateTime? OnceScheduledAt { get; set; }
    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LatestBackupAt { get; set; }
    public string? LatestBackupFileName { get; set; }
    public long TotalBackupBytes { get; set; }
    public int TotalBackupCount { get; set; }
    public string StorageProvider { get; set; } = "Appwrite";
}

public class UpdateBackupScheduleRequest
{
    public bool IsEnabled { get; set; }
    public string ScheduleMode { get; set; } = "Daily";
    public string? TimeOfDay { get; set; }
    public int? DayOfWeek { get; set; }
    public DateTime? OnceScheduledAt { get; set; }
}

public class SystemBackupDownloadResponse
{
    public int BackupId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
}

public class RestoreSystemRequest
{
    public int? BackupId { get; set; }
}

// ─── Dish / Menu DTOs ───────────────────────────────────────────────────────
public class DishDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameEnglish { get; set; }
    public string? Description { get; set; }

    [JsonPropertyName("primarySlotKey")]
    public string? PrimarySlotKey { get; set; }

    [JsonPropertyName("cookingMethod")]
    public string? CookingMethod { get; set; }

    [JsonPropertyName("dishSlotCategoryCodes")]
    public List<string> DishSlotCategoryCodes { get; set; } = new();

    /// <summary>Alias cho view cũ; backend trả về <c>primarySlotKey</c>.</summary>
    public string? Category => PrimarySlotKey;
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

// ─── WeeklyMenu DTOs ────────────────────────────────────────────────────────
public class WeeklyMenuClientDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<WeeklyMenuImageClientDto> Images { get; set; } = new();
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WeeklyMenuImageClientDto
{
    public int Id { get; set; }
    public int MediaFileId { get; set; }
    public string Role { get; set; } = "gallery";
    public int SortOrder { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class GetWeeklyMenusClientResponse : PaginationResponse<WeeklyMenuClientDto> { }

public class GetWeeklyMenuClientResponse
{
    [JsonPropertyName("weeklyMenu")]
    public WeeklyMenuClientDto WeeklyMenu { get; set; } = new();
}

public class GetWeeklyMenuDetailClientResponse
{
    [JsonPropertyName("weeklyMenu")]
    public WeeklyMenuClientDto WeeklyMenu { get; set; } = new();

    [JsonPropertyName("schedules")]
    public List<WeeklyMenuScheduleDetailClientDto> Schedules { get; set; } = new();
}

public class WeeklyMenuScheduleDetailClientDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int MenuId { get; set; }
    public DateTime Date { get; set; }
    public string MealSlot { get; set; } = string.Empty;
    public int DishId { get; set; }
    public DateTime CreatedAt { get; set; }
    public WeeklyMenuScheduleDishSummaryClientDto Dish { get; set; } = new();
}

public class WeeklyMenuScheduleDishSummaryClientDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}

// ─── Customer types / Orders (B2C) ───────────────────────────────────────────
public class GetCustomerTypesClientResponse
{
    public List<CustomerTypeClientDto> Data { get; set; } = new();
}

public class CustomerTypeClientDto
{
    public int Id { get; set; }
    public string? ProfileKey { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class GetOrdersClientResponse : PaginationResponse<OrderDetailClientDto> { }

public class GetOrderClientResponse
{
    [JsonPropertyName("order")]
    public OrderDetailClientDto Order { get; set; } = new();
}

public class OrderDetailClientDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal? SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public OrderPromotionSummaryClientDto? AppliedPromotion { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string? InvoiceCode { get; set; }
    public string? AnnexPdfUrl { get; set; }
    public DateTime? AnnexSignedAt { get; set; }
    public OrderContractSummaryClientDto? ContractSummary { get; set; }
    public List<OrderItemLineClientDto> Items { get; set; } = new();
}

public class OrderPromotionSummaryClientDto
{
    public int PromotionId { get; set; }
    public string? PromotionCode { get; set; }
    public string PromotionName { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
}

public class OrderContractSummaryClientDto
{
    public int Id { get; set; }
    public string? ContractNumber { get; set; }
    public string? ContractFileUrl { get; set; }
    public bool IsDigitallySigned { get; set; }
}

public class SignOrderAnnexApiRequest
{
    public string DigitalSignature { get; set; } = string.Empty;
}

public class OrderItemLineClientDto
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public string DishName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class CreateCustomerMealOrderApiRequest
{
    public DateOnly ScheduledDate { get; set; }
    public List<CreateCustomerMealOrderLineApi> Lines { get; set; } = new();
    public string? PromotionCode { get; set; }
}

public class CreateCustomerMealOrderLineApi
{
    public int DishId { get; set; }
    public int Quantity { get; set; } = 1;
}

// ─── Contracts (manager master-data) ─────────────────────────────────────────
public class ContractListDto
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public string? PartnerLegalName { get; set; }
    public int? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public int? SourceOrderId { get; set; }
    public string ContractType { get; set; } = "Framework";
    public string? ContractNumber { get; set; }
    public string? Description { get; set; }
    public string? SupplySchedule { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalValue { get; set; }
    public decimal? MealUnitPrice { get; set; }
    public decimal? DepositAmount { get; set; }
    public string? ContractFileUrl { get; set; }
    public bool IsDigitallySigned { get; set; }
    public DateTime? DigitallySignedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class GetContractsClientResponse : PaginationResponse<ContractListDto> { }

public class GetManagerContractResponse
{
    public ContractListDto Contract { get; set; } = new();
}

public class CreateContractClientRequest
{
    public int PartnerId { get; set; }
    public string? ContractNumber { get; set; }
    public string? Description { get; set; }
    public string? SupplySchedule { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalValue { get; set; }
    public decimal? DepositAmount { get; set; }
    public string Status { get; set; } = "active";
}

// ─── Finance (manager) ───────────────────────────────────────────────────────
public class OrganizationReceivableLineClientDto
{
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalBilled { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
}

public class GetOrganizationReceivablesClientResponse
{
    public List<OrganizationReceivableLineClientDto> Lines { get; set; } = new();
    public decimal GrandTotalOutstanding { get; set; }
}

public class PaymentHistoryEntryClientDto
{
    public string Source { get; set; } = string.Empty;
    public int EntryId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? OrderId { get; set; }
    public int? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
}

public class GetPaymentHistoryClientResponse
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public string Scope { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<PaymentHistoryEntryClientDto> Entries { get; set; } = new();
}

// ─── Company contracts (B2B self-service) ───────────────────────────────────
public class GetMyOrganizationContractsClientResponse
{
    public List<CustomerContractDto> Contracts { get; set; } = new();
}

public class GetContractClientResponse
{
    public CustomerContractDto Contract { get; set; } = new();
}

public class CustomerContractDto
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public string? PartnerLegalName { get; set; }
    public int? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public string? ContractNumber { get; set; }
    public string? Description { get; set; }
    public string? SupplySchedule { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalValue { get; set; }
    public decimal? DepositAmount { get; set; }
    public string? ContractFileUrl { get; set; }
    public bool IsDigitallySigned { get; set; }
    public DateTime? DigitallySignedAt { get; set; }
    public string? SignatureImage { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SignCompanyContractRequest
{
    public string DigitalSignature { get; set; } = string.Empty;
    public string? SignatureImageUrl { get; set; }
}

// ─── Organization meal order (B2B) ───────────────────────────────────────────
public class GetOrganizationDishCategoriesClientResponse
{
    public List<OrganizationDishCategoryClientDto> Categories { get; set; } = new();
    public DateOnly AllowedFirstServiceDate { get; set; }
    public DateOnly AllowedLastServiceDate { get; set; }
}

public class OrganizationDishCategoryClientDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string SlotKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class GetOrganizationDishesByCategoryClientResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<OrganizationDishListItemClientDto> Dishes { get; set; } = new();
}

public class OrganizationDishListItemClientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> SlotKeys { get; set; } = new();
}

public class PrepareOrganizationMealContractClientRequest
{
    public int OrganizationId { get; set; }
    public decimal Price { get; set; }
    public List<OrganizationMealDayClientRequest> MealDays { get; set; } = new();
    public string? PromotionCode { get; set; }
    public int? PromotionId { get; set; }
}

public class OrganizationMealDayClientRequest
{
    public string ServiceDate { get; set; } = string.Empty;
    public OrganizationMealPlanSlotsClientRequest MealPlan { get; set; } = new();
}

public class OrganizationMealPlanSlotsClientRequest
{
    public List<OrganizationMealLineClientRequest> Main { get; set; } = new();
    public List<OrganizationMealLineClientRequest> Side { get; set; } = new();
    public List<OrganizationMealLineClientRequest> Soup { get; set; } = new();
}

public class OrganizationMealLineClientRequest
{
    [JsonPropertyName("dishId")]
    public int DishId { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = 1;
}

public class PrepareOrganizationMealContractClientResponse
{
    public string DraftId { get; set; } = string.Empty;
    public int ContractId { get; set; }
    public string? ContractNumber { get; set; }
    public string? ContractFileUrl { get; set; }
    public DateOnly AllowedFirstServiceDate { get; set; }
    public DateOnly AllowedLastServiceDate { get; set; }
    public decimal PricePerPortion { get; set; }
    public int TotalMainQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public int? AppliedPromotionId { get; set; }
    public string? AppliedPromotionName { get; set; }
    public string? PromotionCode { get; set; }
    public List<OrganizationMealDraftLineSummaryClientDto> Lines { get; set; } = new();
    public string? PersistenceNotice { get; set; }
}

public class GetPromotionsClientResponse
{
    public List<PromotionClientDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class GetPromotionClientResponse
{
    public PromotionClientDto Promotion { get; set; } = new();
}

public class PromotionClientDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ScopeType { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public int Priority { get; set; }
    public string SelectionMode { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public int? MinOrderQuantity { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public TimeOnly? BookingTimeStart { get; set; }
    public TimeOnly? BookingTimeEnd { get; set; }
    public int? MaxTotalUses { get; set; }
    public int? MaxUsesPerUser { get; set; }
    public bool IsActive { get; set; }
    public List<PromotionTargetClientDto> Targets { get; set; } = new();
}

public class PromotionTargetClientDto
{
    public int Id { get; set; }
    public string TargetType { get; set; } = string.Empty;
    public int? TargetId { get; set; }
    public string? TargetKey { get; set; }
}

public class UpsertPromotionClientRequest
{
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ScopeType { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public int Priority { get; set; }
    public string SelectionMode { get; set; } = "best_discount";
    public string Channel { get; set; } = "all";
    public int? MinOrderQuantity { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public TimeOnly? BookingTimeStart { get; set; }
    public TimeOnly? BookingTimeEnd { get; set; }
    public int? MaxTotalUses { get; set; }
    public int? MaxUsesPerUser { get; set; }
    public bool IsActive { get; set; } = true;
    public List<PromotionTargetClientDto> Targets { get; set; } = new();
}

public class PreviewPromotionClientRequest
{
    public string Channel { get; set; } = "b2c";
    public int? OrganizationId { get; set; }
    public string? ContractType { get; set; }
    public string? PromotionCode { get; set; }
    public int? PromotionId { get; set; }
    public decimal Subtotal { get; set; }
    public int TotalQuantity { get; set; }
    public List<PreviewPromotionLineClientRequest> Lines { get; set; } = new();
}

public class PreviewPromotionLineClientRequest
{
    public int DishId { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

public class PreviewPromotionClientResponse
{
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAfter { get; set; }
    public bool Applied { get; set; }
    public int? PromotionId { get; set; }
    public string? PromotionCode { get; set; }
    public string? PromotionName { get; set; }
    public string? Message { get; set; }
}

public class EligiblePromotionItemClientDto
{
    public int PromotionId { get; set; }
    public string? PromotionCode { get; set; }
    public string PromotionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAfter { get; set; }
    public bool IsRecommended { get; set; }
}

public class ListEligiblePromotionsClientResponse
{
    public decimal Subtotal { get; set; }
    public List<EligiblePromotionItemClientDto> Items { get; set; } = new();
    public int? RecommendedPromotionId { get; set; }
    public string? Message { get; set; }
}

public class OrganizationMealDraftLineSummaryClientDto
{
    public DateOnly ServiceDate { get; set; }
    public string Slot { get; set; } = string.Empty;
    public int DishId { get; set; }
    public string DishName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class CheckoutOrganizationMealClientRequest
{
    public string DraftId { get; set; } = string.Empty;
    public int DepositPercent { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
}

public class CheckoutOrganizationMealClientResponse
{
    public GetOrderClientResponse Order { get; set; } = new();
    public int DepositPercent { get; set; }
    public int DepositAmountVnd { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? QrCode { get; set; }
    public string? PayOsStatus { get; set; }
    public string? PayOsMessage { get; set; }
}

public class InitiateOrganizationMealPaymentClientRequest
{
    public int OrderId { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
}

public class InitiateOrganizationMealPaymentClientResponse
{
    public int OrderId { get; set; }
    public int DepositAmountVnd { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? QrCode { get; set; }
    public string? PayOsStatus { get; set; }
    public string? PayOsMessage { get; set; }
}
