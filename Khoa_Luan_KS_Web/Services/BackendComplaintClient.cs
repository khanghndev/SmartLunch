using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Khoa_Luan_KS_Web.Services;

public class BackendComplaintClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IApiTokenService? _apiTokenService;

    private static readonly JsonSerializerOptions JsonPostOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public BackendComplaintClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IApiTokenService apiTokenService)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _apiTokenService = apiTokenService;
    }

    // ─── Organization ───────────────────────────────────────────────────────

    public Task<ComplaintEligibilityClientDto> GetEligibilityAsync(int orderId, string accessToken, CancellationToken ct = default)
        => GetAsync<ComplaintEligibilityClientDto>(
            $"/api/v1/organization/complaints/orders/{orderId}/eligibility", accessToken, ct);

    public Task<PaginationResponseClient<OrganizationComplaintSummaryClientDto>> ListOrganizationComplaintsAsync(
        string accessToken, int page = 1, int pageSize = 20, CancellationToken ct = default)
        => GetAsync<PaginationResponseClient<OrganizationComplaintSummaryClientDto>>(
            $"/api/v1/organization/complaints?page={page}&pageSize={pageSize}", accessToken, ct);

    public Task<OrganizationComplaintDetailClientDto> GetOrganizationComplaintAsync(
        int id, string accessToken, CancellationToken ct = default)
        => GetAsync<OrganizationComplaintDetailClientDto>(
            $"/api/v1/organization/complaints/{id}", accessToken, ct);

    public Task<OrganizationComplaintDetailClientDto> CreateOrganizationComplaintAsync(
        CreateOrganizationComplaintClientRequest request,
        string accessToken,
        CancellationToken ct = default)
        => PostAsync<OrganizationComplaintDetailClientDto>(
            "/api/v1/organization/complaints", request, accessToken, ct);

    public Task<OrganizationComplaintDetailClientDto> UploadEvidenceAsync(
        int complaintId,
        string kind,
        IFormFile file,
        string accessToken,
        CancellationToken ct = default)
    {
        if (file == null || file.Length <= 0)
            throw new InvalidOperationException("File is required");

        return SendWithRefreshAsync(accessToken, async (token, cancellationToken) =>
        {
            var client = CreateClient(token);
            using var form = new MultipartFormDataContent();
            await using var stream = file.OpenReadStream();
            using var fileContent = new StreamContent(stream);
            if (!string.IsNullOrWhiteSpace(file.ContentType))
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            form.Add(fileContent, "file", file.FileName);
            form.Add(new StringContent(kind), "kind");

            using var res = await client.PostAsync(
                $"/api/v1/organization/complaints/{complaintId}/evidence", form, cancellationToken);
            return await BackendApiAuthHelper.HandleResponseAsync<OrganizationComplaintDetailClientDto>(res, cancellationToken);
        }, ct);
    }

    public Task<OrganizationComplaintDetailClientDto> DeleteEvidenceAsync(
        int complaintId, int evidenceId, string accessToken, CancellationToken ct = default)
    {
        return SendWithRefreshAsync(accessToken, async (token, cancellationToken) =>
        {
            var client = CreateClient(token);
            using var res = await client.DeleteAsync(
                $"/api/v1/organization/complaints/{complaintId}/evidence/{evidenceId}", cancellationToken);
            return await BackendApiAuthHelper.HandleResponseAsync<OrganizationComplaintDetailClientDto>(res, cancellationToken);
        }, ct);
    }

    public Task<OrganizationComplaintDetailClientDto> SubmitOrganizationComplaintAsync(
        int id, string accessToken, CancellationToken ct = default)
        => PostAsync<OrganizationComplaintDetailClientDto>(
            $"/api/v1/organization/complaints/{id}/submit", new { }, accessToken, ct);

    public Task<OrganizationDeliveryOtpClientDto> GetDeliveryOtpAsync(
        int orderId, string accessToken, CancellationToken ct = default)
        => GetAsync<OrganizationDeliveryOtpClientDto>(
            $"/api/v1/organization/deliveries/orders/{orderId}/otp", accessToken, ct);

    // ─── Manager ──────────────────────────────────────────────────────────────

    public Task<GetManagerComplaintsClientResponse> GetManagerComplaintsAsync(
        string accessToken,
        int page = 1,
        int pageSize = 20,
        string? searchTerm = null,
        string? status = null,
        CancellationToken ct = default)
    {
        var q = $"?Page={page}&PageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchTerm))
            q += $"&SearchTerm={Uri.EscapeDataString(searchTerm)}";
        if (!string.IsNullOrWhiteSpace(status))
            q += $"&status={Uri.EscapeDataString(status)}";
        return GetAsync<GetManagerComplaintsClientResponse>(
            $"/api/v1/master-data/Complaint{q}", accessToken, ct);
    }

    public Task<ManagerComplaintDetailClientDto> GetManagerComplaintReviewAsync(
        int id, string accessToken, CancellationToken ct = default)
        => GetAsync<ManagerComplaintDetailClientDto>(
            $"/api/v1/master-data/Complaint/{id}/review", accessToken, ct);

    public Task<ManagerComplaintDetailClientDto> ResolveManagerComplaintAsync(
        int id,
        ResolveManagerComplaintClientRequest request,
        string accessToken,
        CancellationToken ct = default)
        => PatchAsync<ManagerComplaintDetailClientDto>(
            $"/api/v1/master-data/Complaint/{id}/resolve", request, accessToken, ct);

    // ─── HTTP helpers ─────────────────────────────────────────────────────────

    private HttpClient CreateClient(string accessToken)
    {
        var baseUrl = _configuration["BackendApi:BaseUrl"]?.TrimEnd('/');
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl!);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private Task<T> GetAsync<T>(string path, string accessToken, CancellationToken ct) =>
        SendWithRefreshAsync(accessToken, async (token, cancellationToken) =>
        {
            var client = CreateClient(token);
            using var res = await client.GetAsync(path, cancellationToken);
            return await BackendApiAuthHelper.HandleResponseAsync<T>(res, cancellationToken);
        }, ct);

    private Task<T> PostAsync<T>(string path, object payload, string accessToken, CancellationToken ct) =>
        SendWithRefreshAsync(accessToken, async (token, cancellationToken) =>
        {
            var client = CreateClient(token);
            var json = JsonSerializer.Serialize(payload, JsonPostOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var res = await client.PostAsync(path, content, cancellationToken);
            return await BackendApiAuthHelper.HandleResponseAsync<T>(res, cancellationToken);
        }, ct);

    private Task<T> PatchAsync<T>(string path, object payload, string accessToken, CancellationToken ct) =>
        SendWithRefreshAsync(accessToken, async (token, cancellationToken) =>
        {
            var client = CreateClient(token);
            var json = JsonSerializer.Serialize(payload, JsonPostOptions);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var req = new HttpRequestMessage(HttpMethod.Patch, path) { Content = content };
            using var res = await client.SendAsync(req, cancellationToken);
            return await BackendApiAuthHelper.HandleResponseAsync<T>(res, cancellationToken);
        }, ct);

    private Task<T> SendWithRefreshAsync<T>(
        string accessToken,
        Func<string, CancellationToken, Task<T>> send,
        CancellationToken ct) =>
        BackendApiAuthHelper.SendWithRefreshAsync(_apiTokenService, accessToken, send, ct);
}

// ─── DTOs ───────────────────────────────────────────────────────────────────

public class PaginationResponseClient<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class ComplaintEligibilityClientDto
{
    public int OrderId { get; set; }
    public bool CanComplain { get; set; }
    public string? BlockReason { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ComplaintDeadlineAt { get; set; }
    public int OrderedMainPortionCount { get; set; }
    public int ExistingComplaintCount { get; set; }
}

public class ComplaintEvidenceClientDto
{
    public int Id { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrganizationComplaintSummaryClientDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int? OrderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Resolution { get; set; }
    public decimal? FinalRefundAmount { get; set; }
    public int? RefundPaymentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ComplaintDeadlineAt { get; set; }
}

public class OrganizationComplaintDetailClientDto : OrganizationComplaintSummaryClientDto
{
    public string Description { get; set; } = string.Empty;
    public int? MissingPortionCount { get; set; }
    public int? RefundPortionCount { get; set; }
    public decimal? SuggestedRefundAmount { get; set; }
    public string? ResolutionNote { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public List<ComplaintEvidenceClientDto> Evidence { get; set; } = new();
}

public class DeliveryProofContextClientDto
{
    public int DeliveryId { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? ProofImageUrl { get; set; }
    public string? RecipientConfirmedName { get; set; }
    public DateTime? RecipientConfirmedAt { get; set; }
    public string? ShipperName { get; set; }
}

public class ManagerComplaintDetailClientDto : OrganizationComplaintDetailClientDto
{
    public string? OrganizationName { get; set; }
    public string? OrderInvoiceCode { get; set; }
    public decimal OrderTotalAmount { get; set; }
    public int MealCount { get; set; }
    public decimal UnitPricePerPortion { get; set; }
    public DeliveryProofContextClientDto? ShipperDelivery { get; set; }
    public string? ComplainantName { get; set; }
}

public class CreateOrganizationComplaintClientRequest
{
    public int OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? MissingPortionCount { get; set; }
}

public class ResolveManagerComplaintClientRequest
{
    public string Resolution { get; set; } = string.Empty;
    public string? ResolutionNote { get; set; }
    public decimal? FinalRefundAmount { get; set; }
    public int? RefundPortionCount { get; set; }
}

public class OrganizationDeliveryOtpClientDto
{
    public int OrderId { get; set; }
    public int DeliveryId { get; set; }
    public string? DeliveryOtp { get; set; }
    public DateTime? DeliveryOtpExpiresAt { get; set; }
    public string DeliveryStatus { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class GetManagerComplaintsClientResponse
{
    public List<ManagerComplaintListItemClientDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class ManagerComplaintListItemClientDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int UserId { get; set; }
    public int? OrderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Resolution { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ComplaintDeadlineAt { get; set; }
    public int? RefundPortionCount { get; set; }
    public decimal? SuggestedRefundAmount { get; set; }
    public decimal? FinalRefundAmount { get; set; }
    public int? RefundPaymentId { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
