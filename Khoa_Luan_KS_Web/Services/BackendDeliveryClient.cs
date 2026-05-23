using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Khoa_Luan_KS_Web.Services;

public class BackendDeliveryClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public BackendDeliveryClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public Task<GetManagerDeliveriesClientResponse> GetDeliveriesAsync(
        string accessToken,
        int page = 1,
        int pageSize = 50,
        string? status = null,
        DateOnly? scheduledOn = null,
        string? searchTerm = null,
        bool? unassignedOnly = null,
        CancellationToken ct = default)
    {
        var q = $"?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(status)) q += $"&status={Uri.EscapeDataString(status)}";
        if (scheduledOn.HasValue) q += $"&scheduledOn={scheduledOn.Value:yyyy-MM-dd}";
        if (!string.IsNullOrWhiteSpace(searchTerm)) q += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
        if (unassignedOnly == true) q += "&unassignedOnly=true";
        return GetAsync<GetManagerDeliveriesClientResponse>($"/api/v1/manager/deliveries{q}", accessToken, ct);
    }

    public Task<GetManagerShippersClientResponse> GetShippersAsync(string accessToken, CancellationToken ct = default)
        => GetAsync<GetManagerShippersClientResponse>("/api/v1/manager/deliveries/shippers", accessToken, ct);

    public Task<ManagerDeliveryListItemClientDto> AssignAsync(
        int deliveryId,
        AssignManagerDeliveryClientRequest request,
        string accessToken,
        CancellationToken ct = default)
        => PutAsync<ManagerDeliveryListItemClientDto>($"/api/v1/manager/deliveries/{deliveryId}/assign", request, accessToken, ct);

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

public sealed class GetManagerDeliveriesClientResponse
{
    public List<ManagerDeliveryListItemClientDto> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public ManagerDeliveryStatsClientDto Stats { get; set; } = new();
}

public sealed class ManagerDeliveryStatsClientDto
{
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }
    public int Unassigned { get; set; }
}

public sealed class ManagerDeliveryListItemClientDto
{
    public int DeliveryId { get; set; }
    public string? DeliveryCode { get; set; }
    public int OrderId { get; set; }
    public string? OrderCode { get; set; }
    public string? InvoiceCode { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryStatus { get; set; } = string.Empty;
    public string DeliveryStatusLabel { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string? PreferredDeliveryTime { get; set; }
    public int MealCount { get; set; }
    public int? AssignedStaffId { get; set; }
    public string? AssignedStaffName { get; set; }
    public string? AssignedStaffPhone { get; set; }
    public string? Notes { get; set; }
    public string? ProofImageUrl { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class GetManagerShippersClientResponse
{
    public List<ManagerShipperOptionClientDto> Shippers { get; set; } = new();
}

public sealed class ManagerShipperOptionClientDto
{
    public int UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
}

public sealed class AssignManagerDeliveryClientRequest
{
    public int ShipperUserId { get; set; }
    public string? Notes { get; set; }
}
