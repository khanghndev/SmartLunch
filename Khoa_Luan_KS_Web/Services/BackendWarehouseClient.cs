using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Khoa_Luan_KS_Web.Services;

/// <summary>
/// HTTP client wrapper for the SmartLunch Backend warehouse / inventory / finance endpoints.
/// Covers: Ingredient, IngredientSource, Inventory, IngredientInventory, IngredientIntakeProposal, Finance.
/// </summary>
public class BackendWarehouseClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public BackendWarehouseClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    // ───────────────────────────── Ingredient (master data) ─────────────────────────────
    public Task<GetIngredientsClientResponse> GetIngredientsAsync(string accessToken, int page = 1, int pageSize = 20, string? searchTerm = null, bool? isActive = null, CancellationToken ct = default)
    {
        var q = $"?Page={page}&PageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchTerm)) q += $"&SearchTerm={Uri.EscapeDataString(searchTerm)}";
        if (isActive.HasValue) q += $"&IsActive={(isActive.Value ? "true" : "false")}";
        return GetAsync<GetIngredientsClientResponse>($"/api/v1/master-data/Ingredient{q}", accessToken, ct);
    }

    public Task<GetIngredientClientResponse> GetIngredientAsync(int id, string accessToken, CancellationToken ct = default)
        => GetAsync<GetIngredientClientResponse>($"/api/v1/master-data/Ingredient/{id}", accessToken, ct);

    // ───────────────────────────── IngredientSource (lô hàng) ───────────────────────────
    public Task<GetIngredientSourcesClientResponse> GetIngredientSourcesAsync(string accessToken, int page = 1, int pageSize = 20, string? searchTerm = null, int? partnerId = null, int? ingredientId = null, CancellationToken ct = default)
    {
        var q = $"?Page={page}&PageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchTerm)) q += $"&SearchTerm={Uri.EscapeDataString(searchTerm)}";
        if (partnerId.HasValue) q += $"&PartnerId={partnerId.Value}";
        if (ingredientId.HasValue) q += $"&IngredientId={ingredientId.Value}";
        return GetAsync<GetIngredientSourcesClientResponse>($"/api/v1/master-data/IngredientSource{q}", accessToken, ct);
    }

    public Task<GetIngredientSourceClientResponse> GetIngredientSourceAsync(int id, string accessToken, CancellationToken ct = default)
        => GetAsync<GetIngredientSourceClientResponse>($"/api/v1/master-data/IngredientSource/{id}", accessToken, ct);

    public Task<GetIngredientSourceClientResponse> CreateIngredientSourceAsync(CreateIngredientSourceClientRequest payload, string accessToken, CancellationToken ct = default)
        => PostAsync<GetIngredientSourceClientResponse>("/api/v1/master-data/IngredientSource", payload, accessToken, ct);

    public Task<GetIngredientSourceClientResponse> UpdateIngredientSourceAsync(int id, CreateIngredientSourceClientRequest payload, string accessToken, CancellationToken ct = default)
        => PutAsync<GetIngredientSourceClientResponse>($"/api/v1/master-data/IngredientSource/{id}", payload, accessToken, ct);

    public async Task DeleteIngredientSourceAsync(int id, string accessToken, CancellationToken ct = default)
    {
        var client = CreateClient(accessToken);
        using var res = await client.DeleteAsync($"/api/v1/master-data/IngredientSource/{id}", ct);
        await HandleResponse<object>(res, ct);
    }

    // ───────────────────────────── Inventory (master) ───────────────────────────────────
    public Task<GetInventoriesClientResponse> GetInventoriesAsync(string accessToken, int page = 1, int pageSize = 50, string? searchTerm = null, CancellationToken ct = default)
    {
        var q = $"?Page={page}&PageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchTerm)) q += $"&SearchTerm={Uri.EscapeDataString(searchTerm)}";
        return GetAsync<GetInventoriesClientResponse>($"/api/v1/master-data/Inventory{q}", accessToken, ct);
    }

    // ───────────────────────────── IngredientInventory ──────────────────────────────────
    public Task<GetLowStockAlertsClientResponse> GetLowStockAlertsAsync(string accessToken, CancellationToken ct = default)
        => GetAsync<GetLowStockAlertsClientResponse>("/api/v1/ingredient-inventory/low-stock-alerts", accessToken, ct);

    public Task<IngredientInventoryDetailClientDto> GetIngredientInventoryDetailAsync(int ingredientId, string accessToken, int recentBatches = 20, CancellationToken ct = default)
        => GetAsync<IngredientInventoryDetailClientDto>($"/api/v1/ingredient-inventory/detail/{ingredientId}?recentBatches={recentBatches}", accessToken, ct);

    public Task<GetInternalIssuesClientResponse> GetInternalIssuesAsync(string accessToken, int page = 1, int pageSize = 20, DateOnly? from = null, DateOnly? to = null, CancellationToken ct = default)
    {
        var q = $"?Page={page}&PageSize={pageSize}";
        if (from.HasValue) q += $"&IssuedFrom={from:yyyy-MM-dd}";
        if (to.HasValue) q += $"&IssuedTo={to:yyyy-MM-dd}";
        return GetAsync<GetInternalIssuesClientResponse>($"/api/v1/ingredient-inventory/internal-issues{q}", accessToken, ct);
    }

    public Task<InternalIssueDetailClientDto> GetInternalIssueAsync(int id, string accessToken, CancellationToken ct = default)
        => GetAsync<InternalIssueDetailClientDto>($"/api/v1/ingredient-inventory/internal-issues/{id}", accessToken, ct);

    public Task<CreateInternalIssueClientResponse> CreateInternalIssueAsync(CreateInternalIssueClientRequest payload, string accessToken, CancellationToken ct = default)
        => PostAsync<CreateInternalIssueClientResponse>("/api/v1/ingredient-inventory/internal-issues", payload, accessToken, ct);

    // ───────────────────────────── IngredientIntakeProposal ─────────────────────────────
    public Task<GetIntakeProposalsClientResponse> GetIntakeProposalsAsync(string accessToken, int page = 1, int pageSize = 20, CancellationToken ct = default)
        => GetAsync<GetIntakeProposalsClientResponse>($"/api/v1/ingredient-intake-proposals?Page={page}&PageSize={pageSize}", accessToken, ct);

    public Task<GetIntakeReviewHistoryClientResponse> GetIntakeProposalReviewHistoryAsync(string accessToken, int page = 1, int pageSize = 20, CancellationToken ct = default)
        => GetAsync<GetIntakeReviewHistoryClientResponse>($"/api/v1/ingredient-intake-proposals/review-history?Page={page}&PageSize={pageSize}", accessToken, ct);

    public Task<IntakeProposalDetailClientDto> GetIntakeProposalAsync(int id, string accessToken, CancellationToken ct = default)
        => GetAsync<IntakeProposalDetailClientDto>($"/api/v1/ingredient-intake-proposals/{id}", accessToken, ct);

    public Task<CreateIntakeProposalClientResponse> CreateIntakeProposalAsync(CreateIntakeProposalClientRequest payload, string accessToken, CancellationToken ct = default)
        => PostAsync<CreateIntakeProposalClientResponse>("/api/v1/ingredient-intake-proposals", payload, accessToken, ct);

    public Task<CreateActualReceiptClientResponse> CreateActualReceiptAsync(int proposalId, CreateActualReceiptClientRequest payload, string accessToken, CancellationToken ct = default)
        => PostAsync<CreateActualReceiptClientResponse>($"/api/v1/ingredient-intake-proposals/{proposalId}/actual-receipt", payload, accessToken, ct);

    // ───────────────────────────── Finance ──────────────────────────────────────────────
    public Task<SupplierPayablesClientResponse> GetSupplierPayablesAsync(string accessToken, bool onlyOutstanding = true, int? partnerId = null, CancellationToken ct = default)
    {
        var q = $"?OnlyWithOutstanding={(onlyOutstanding ? "true" : "false")}";
        if (partnerId.HasValue) q += $"&PartnerId={partnerId.Value}";
        return GetAsync<SupplierPayablesClientResponse>($"/api/v1/finance/supplier-payables{q}", accessToken, ct);
    }

    // ───────────────────────────── Plumbing ─────────────────────────────────────────────
    private async Task<T> GetAsync<T>(string path, string accessToken, CancellationToken ct)
    {
        var client = CreateClient(accessToken);
        using var res = await client.GetAsync(path, ct);
        return await HandleResponse<T>(res, ct);
    }

    private async Task<T> PostAsync<T>(string path, object payload, string accessToken, CancellationToken ct)
    {
        var client = CreateClient(accessToken);
        var json = JsonSerializer.Serialize(payload, JsonOpts);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var res = await client.PostAsync(path, content, ct);
        return await HandleResponse<T>(res, ct);
    }

    private async Task<T> PutAsync<T>(string path, object payload, string accessToken, CancellationToken ct)
    {
        var client = CreateClient(accessToken);
        var json = JsonSerializer.Serialize(payload, JsonOpts);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var res = await client.PutAsync(path, content, ct);
        return await HandleResponse<T>(res, ct);
    }

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private HttpClient CreateClient(string accessToken)
    {
        var baseUrl = _configuration["BackendApi:BaseUrl"]?.TrimEnd('/');
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl!);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private static async Task<T> HandleResponse<T>(HttpResponseMessage res, CancellationToken ct)
    {
        var body = await res.Content.ReadAsStringAsync(ct);
        if (!res.IsSuccessStatusCode)
        {
            var msg = TryExtractBackendMessage(body) ?? $"Backend request failed ({(int)res.StatusCode})";
            throw new InvalidOperationException(msg);
        }

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var envelope = JsonSerializer.Deserialize<BaseApiResponse<T>>(body, opts);
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
            string? final = null;
            if (doc.RootElement.TryGetProperty("message", out var msg) && msg.ValueKind == JsonValueKind.String)
                final = msg.GetString();
            if (doc.RootElement.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0)
            {
                var errs = errors.EnumerateArray().Select(e => e.GetString()).Where(e => !string.IsNullOrEmpty(e));
                var s = string.Join(" | ", errs);
                if (!string.IsNullOrEmpty(s))
                    final = final == null ? s : $"{final} ({s})";
            }
            return final;
        }
        catch { return null; }
    }
}

// ─── DTOs (client side) ────────────────────────────────────────────────────────────────
public sealed class IngredientClientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? DefaultSupplierId { get; set; }
    public decimal? CostPerUnit { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class GetIngredientsClientResponse : PaginationResponse<IngredientClientDto> { }

public sealed class GetIngredientClientResponse
{
    public IngredientClientDto Ingredient { get; set; } = new();
}

public sealed class IngredientSourceClientDto
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public string? IngredientName { get; set; }
    public int? PartnerId { get; set; }
    public string? PartnerLegalName { get; set; }
    public string? BatchNumber { get; set; }
    public string? OriginDetails { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Certification { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class GetIngredientSourcesClientResponse : PaginationResponse<IngredientSourceClientDto> { }

public sealed class GetIngredientSourceClientResponse
{
    public IngredientSourceClientDto IngredientSource { get; set; } = new();
}

public sealed class CreateIngredientSourceClientRequest
{
    public int IngredientId { get; set; }
    public int? PartnerId { get; set; }
    public string? BatchNumber { get; set; }
    public string? OriginDetails { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Certification { get; set; }
}

public sealed class InventoryClientDto
{
    public int IngredientId { get; set; }
    public decimal QuantityAvailable { get; set; }
    public decimal? ReorderLevel { get; set; }
    public DateTime LastUpdated { get; set; }
}

public sealed class GetInventoriesClientResponse : PaginationResponse<InventoryClientDto> { }

public sealed class LowStockAlertClientDto
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal QuantityAvailable { get; set; }
    public decimal? ReorderLevel { get; set; }
    public decimal? Shortage { get; set; }
    public DateTime LastUpdated { get; set; }
}

public sealed class GetLowStockAlertsClientResponse
{
    public List<LowStockAlertClientDto> Alerts { get; set; } = new();
    public int TotalCount { get; set; }
}

public sealed class IngredientSourceBatchClientDto
{
    public int Id { get; set; }
    public string? BatchNumber { get; set; }
    public string? OriginDetails { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? SupplierLegalName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class IngredientInventoryDetailClientDto
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public decimal? CostPerUnit { get; set; }
    public int? DefaultSupplierId { get; set; }
    public string? DefaultSupplierLegalName { get; set; }
    public decimal QuantityAvailable { get; set; }
    public decimal? ReorderLevel { get; set; }
    public bool IsLowStock { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<IngredientSourceBatchClientDto> RecentBatches { get; set; } = new();
}

public sealed class InternalIssueLineClientDto
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public sealed class InternalIssueSummaryClientDto
{
    public int Id { get; set; }
    public string IssueCode { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public string? Reason { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByDisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int LineCount { get; set; }
}

public sealed class InternalIssueDetailClientDto
{
    public int Id { get; set; }
    public string IssueCode { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public string? Reason { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByDisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<InternalIssueLineClientDto> Lines { get; set; } = new();
}

public sealed class GetInternalIssuesClientResponse
{
    public List<InternalIssueSummaryClientDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public sealed class CreateInternalIssueClientRequest
{
    public DateTime? IssuedAtUtc { get; set; }
    public string? Reason { get; set; }
    public List<CreateInternalIssueLineClientRequest> Lines { get; set; } = new();
}

public sealed class CreateInternalIssueLineClientRequest
{
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
}

public sealed class CreateInternalIssueClientResponse
{
    public InternalIssueDetailClientDto Issue { get; set; } = new();
}

// ─── Intake proposal DTOs ───────────────────────────────────────────────────────────
public sealed class IntakeProposalLineClientDto
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? LineNote { get; set; }
}

public sealed class IntakeProposalSummaryClientDto
{
    public int Id { get; set; }
    public string ProposalCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? HeaderNote { get; set; }
    public int CreatedByUserId { get; set; }
    public string? CreatedByDisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool HasActualReceipt { get; set; }
}

public sealed class IntakeProposalDetailClientDto
{
    public int Id { get; set; }
    public string ProposalCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? HeaderNote { get; set; }
    public int CreatedByUserId { get; set; }
    public string? CreatedByDisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ReviewedByUserId { get; set; }
    public string? ReviewedByDisplayName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNote { get; set; }
    public bool HasActualReceipt { get; set; }
    public string? ActualReceiptCode { get; set; }
    public List<IntakeProposalLineClientDto> Lines { get; set; } = new();
}

public sealed class GetIntakeProposalsClientResponse
{
    public List<IntakeProposalSummaryClientDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public sealed class IntakeReviewHistoryEntryClientDto
{
    public int ProposalId { get; set; }
    public string ProposalCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? HeaderNote { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public string? CreatedByDisplayName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public int? ReviewedByUserId { get; set; }
    public string? ReviewedByDisplayName { get; set; }
    public string? ReviewNote { get; set; }
}

public sealed class GetIntakeReviewHistoryClientResponse
{
    public List<IntakeReviewHistoryEntryClientDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public sealed class CreateIntakeProposalClientRequest
{
    public string? HeaderNote { get; set; }
    public List<CreateIntakeProposalLineClientRequest> Lines { get; set; } = new();
}

public sealed class CreateIntakeProposalLineClientRequest
{
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string? LineNote { get; set; }
}

public sealed class CreateIntakeProposalClientResponse
{
    public IntakeProposalDetailClientDto Proposal { get; set; } = new();
}

public sealed class CreateActualReceiptClientRequest
{
    public bool? ConfirmIngredientsMeetStandard { get; set; }
    public DateTime? ReceivedAtUtc { get; set; }
    public string? Note { get; set; }
}

public sealed class ActualReceiptLineClientDto
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public sealed class ActualReceiptDetailClientDto
{
    public int Id { get; set; }
    public string ReceiptCode { get; set; } = string.Empty;
    public int ProposalId { get; set; }
    public string ProposalCode { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public string? CreatedByDisplayName { get; set; }
    public DateTime ReceivedAt { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ActualReceiptLineClientDto> Lines { get; set; } = new();
}

public sealed class CreateActualReceiptClientResponse
{
    public ActualReceiptDetailClientDto Receipt { get; set; } = new();
}

// ─── Finance DTOs ──────────────────────────────────────────────────────────────────
public sealed class SupplierPayableLineClientDto
{
    public int PartnerId { get; set; }
    public string PartnerLegalName { get; set; } = string.Empty;
    public decimal TotalContractValue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
}

public sealed class SupplierPayablesClientResponse
{
    public List<SupplierPayableLineClientDto> Lines { get; set; } = new();
    public decimal GrandTotalOutstanding { get; set; }
}
