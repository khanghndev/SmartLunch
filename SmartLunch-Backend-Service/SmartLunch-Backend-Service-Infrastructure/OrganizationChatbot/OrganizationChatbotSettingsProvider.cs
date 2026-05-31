using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartLunch.Backend.Service.Application.DTOs.Request.OrganizationChatbot;
using SmartLunch.Backend.Service.Application.DTOs.Response.OrganizationChatbot;
using SmartLunch.Backend.Service.Application.OrganizationChatbot;
using SmartLunch.Backend.Service.Domain.Time;

namespace SmartLunch.Backend.Service.Infrastructure.OrganizationChatbot;

/// <summary>Lưu cấu hình chatbot vào App_Data — admin có thể đổi key/model không cần restart appsettings.</summary>
public sealed class OrganizationChatbotSettingsProvider
    : IOrganizationChatbotSettingsProvider, IHostedService
{
    private readonly IHostEnvironment _env;
    private readonly IOptions<OrganizationChatbotOptions> _bootstrap;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OrganizationChatbotSettingsProvider> _logger;
    private readonly object _lock = new();

    private OrganizationChatbotRuntimeState _state = new();
    private readonly string _filePath;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
    };

    public OrganizationChatbotSettingsProvider(
        IHostEnvironment env,
        IOptions<OrganizationChatbotOptions> bootstrap,
        IHttpClientFactory httpClientFactory,
        ILogger<OrganizationChatbotSettingsProvider> logger)
    {
        _env = env;
        _bootstrap = bootstrap;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _filePath = Path.Combine(_env.ContentRootPath, "App_Data", "organization-chatbot.settings.json");
    }

    public OrganizationChatbotOptions GetCurrent()
    {
        lock (_lock)
            return CloneOptions(_state.Options);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        await LoadAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task<OrganizationChatbotAdminConfigDto> GetAdminConfigAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
            return Task.FromResult(ToAdminDto(_state));
    }

    public async Task<OrganizationChatbotAdminConfigDto> SaveAsync(
        UpdateOrganizationChatbotConfigRequest request,
        int? updatedByUserId,
        CancellationToken cancellationToken = default)
    {
        OrganizationChatbotRuntimeState snapshot;
        lock (_lock)
        {
            var next = CloneOptions(_state.Options);
            next.Enabled = request.Enabled;
            next.Provider = NormalizeProvider(request.Provider);
            next.Model = string.IsNullOrWhiteSpace(request.Model) ? "gemini-2.5-flash" : request.Model.Trim();
            next.Temperature = Clamp(request.Temperature, 0, 1);
            next.MaxOutputTokens = ClampInt(request.MaxOutputTokens, 256, 4096);
            next.SystemPrompt = request.UseDefaultSystemPrompt
                ? null
                : string.IsNullOrWhiteSpace(request.SystemPrompt) ? null : request.SystemPrompt.Trim();
            next.CustomRules = string.IsNullOrWhiteSpace(request.CustomRules)
                ? null
                : request.CustomRules.Trim();

            if (ShouldReplaceApiKey(request.ApiKey))
                next.ApiKey = request.ApiKey!.Trim();

            _state.Options = next;
            _state.UpdatedAt = VietnamTime.Now;
            _state.UpdatedByUserId = updatedByUserId;
            _state.ConfigSource = "App_Data/organization-chatbot.settings.json";
            snapshot = CloneState(_state);
        }

        await PersistAsync(snapshot, cancellationToken);
        _logger.LogInformation("Organization chatbot settings updated by user {UserId}", updatedByUserId);

        lock (_lock)
            return ToAdminDto(_state);
    }

    public async Task<OrganizationChatbotTestResultDto> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        var options = GetCurrent();
        if (!options.IsConfigured)
        {
            return new OrganizationChatbotTestResultDto
            {
                Success = false,
                Message = "Chatbot chưa bật hoặc thiếu API Key.",
            };
        }

        if (!string.Equals(options.Provider, "Gemini", StringComparison.OrdinalIgnoreCase))
        {
            return new OrganizationChatbotTestResultDto
            {
                Success = false,
                Message = $"Provider '{options.Provider}' chưa hỗ trợ test tự động. Chỉ Gemini.",
            };
        }

        var sw = Stopwatch.StartNew();
        try
        {
            var model = string.IsNullOrWhiteSpace(options.Model) ? "gemini-2.5-flash" : options.Model;
            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/{Uri.EscapeDataString(model)}:generateContent?key={Uri.EscapeDataString(options.ApiKey!)}";

            var payload = new
            {
                contents = new[]
                {
                    new { role = "user", parts = new[] { new { text = "Trả lời đúng một từ: OK" } } },
                },
                generationConfig = new { temperature = 0, maxOutputTokens = 16 },
            };

            using var client = _httpClientFactory.CreateClient();
            using var res = await client.PostAsJsonAsync(url, payload, JsonOptions, cancellationToken);
            var body = await res.Content.ReadAsStringAsync(cancellationToken);
            sw.Stop();

            var success = res.IsSuccessStatusCode;
            var message = success
                ? "Kết nối Gemini thành công."
                : ExtractGeminiError(body) ?? $"Gemini trả về HTTP {(int)res.StatusCode}.";

            lock (_lock)
            {
                _state.LastTestLatencyMs = (int)sw.ElapsedMilliseconds;
                _state.LastTestMessage = message;
                _state.LastTestSuccess = success;
            }

            await PersistAsync(CloneState(_state), cancellationToken);

            return new OrganizationChatbotTestResultDto
            {
                Success = success,
                LatencyMs = (int)sw.ElapsedMilliseconds,
                Message = message,
                Model = model,
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            var message = $"Lỗi kết nối: {ex.Message}";
            lock (_lock)
            {
                _state.LastTestLatencyMs = (int)sw.ElapsedMilliseconds;
                _state.LastTestMessage = message;
                _state.LastTestSuccess = false;
            }
            await PersistAsync(CloneState(_state), cancellationToken);
            return new OrganizationChatbotTestResultDto
            {
                Success = false,
                LatencyMs = (int)sw.ElapsedMilliseconds,
                Message = message,
            };
        }
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        OrganizationChatbotRuntimeState loaded;
        if (File.Exists(_filePath))
        {
            try
            {
                await using var stream = File.OpenRead(_filePath);
                loaded = await JsonSerializer.DeserializeAsync<OrganizationChatbotRuntimeState>(stream, JsonOptions, cancellationToken)
                         ?? BootstrapFromConfiguration();
                loaded.ConfigSource ??= "App_Data/organization-chatbot.settings.json";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load chatbot settings file, using appsettings bootstrap");
                loaded = BootstrapFromConfiguration();
            }
        }
        else
        {
            loaded = BootstrapFromConfiguration();
            loaded.ConfigSource = "appsettings.json (bootstrap → App_Data)";
            await PersistAsync(loaded, cancellationToken);
        }

        lock (_lock)
            _state = loaded;
    }

    private OrganizationChatbotRuntimeState BootstrapFromConfiguration()
    {
        return new OrganizationChatbotRuntimeState
        {
            Options = CloneOptions(_bootstrap.Value),
            UpdatedAt = VietnamTime.Now,
            ConfigSource = "appsettings.json",
        };
    }

    private async Task PersistAsync(OrganizationChatbotRuntimeState state, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, state, JsonOptions, cancellationToken);
    }

    private static OrganizationChatbotAdminConfigDto ToAdminDto(OrganizationChatbotRuntimeState state)
    {
        var o = state.Options;
        var status = !o.Enabled
            ? "disabled"
            : o.IsConfigured
                ? state.LastTestSuccess == false ? "warning" : "online"
                : "offline";

        return new OrganizationChatbotAdminConfigDto
        {
            Enabled = o.Enabled,
            Provider = o.Provider,
            Model = o.Model,
            HasApiKey = !string.IsNullOrWhiteSpace(o.ApiKey),
            ApiKeyMasked = MaskApiKey(o.ApiKey),
            Temperature = o.Temperature,
            MaxOutputTokens = o.MaxOutputTokens,
            StoredSystemPrompt = o.SystemPrompt,
            DefaultSystemPrompt = OrganizationChatbotDefaultPrompts.SystemPrompt,
            UseDefaultSystemPrompt = o.UsesDefaultSystemPrompt,
            EffectiveSystemPrompt = o.ResolveSystemPrompt(),
            CustomRules = o.CustomRules,
            Status = status,
            LlmModeLabel = OrganizationChatbotAdminCatalog.ResolveLlmModeLabel(o),
            ConfigSource = state.ConfigSource ?? "App_Data/organization-chatbot.settings.json",
            UpdatedAt = state.UpdatedAt,
            LastTestLatencyMs = state.LastTestLatencyMs,
            LastTestMessage = state.LastTestMessage,
            ProcessingRules = OrganizationChatbotAdminCatalog.GetProcessingRules()
                .Select(r => new OrganizationChatbotRuleItemDto
                {
                    Title = r.Title,
                    Description = r.Description,
                    IsFixed = r.IsFixed,
                })
                .ToList(),
        };
    }

    private static bool ShouldReplaceApiKey(string? apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey)) return false;
        var t = apiKey.Trim();
        if (t.Contains("...", StringComparison.Ordinal)) return false;
        if (t.All(c => c == '*' || c == '•')) return false;
        return true;
    }

    public static string MaskApiKey(string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return string.Empty;
        if (key.Length <= 8) return "********";
        return key[..4] + "..." + key[^4..];
    }

    private static string NormalizeProvider(string? provider) =>
        provider?.Trim() switch
        {
            "Google (Gemini)" or "Gemini" or "google" => "Gemini",
            "OpenAI (ChatGPT)" or "OpenAI" or "openai" => "OpenAI",
            "None" or "none" => "None",
            _ => string.IsNullOrWhiteSpace(provider) ? "Gemini" : provider.Trim(),
        };

    private static double Clamp(double v, double min, double max) => Math.Max(min, Math.Min(max, v));
    private static int ClampInt(int v, int min, int max) => Math.Max(min, Math.Min(max, v));

    private static OrganizationChatbotOptions CloneOptions(OrganizationChatbotOptions src) => new()
    {
        Enabled = src.Enabled,
        Provider = src.Provider,
        ApiKey = src.ApiKey,
        Model = src.Model,
        BaseUrl = src.BaseUrl,
        Temperature = src.Temperature,
        MaxOutputTokens = src.MaxOutputTokens,
        SystemPrompt = src.SystemPrompt,
        CustomRules = src.CustomRules,
    };

    private static OrganizationChatbotRuntimeState CloneState(OrganizationChatbotRuntimeState src) => new()
    {
        Options = CloneOptions(src.Options),
        UpdatedAt = src.UpdatedAt,
        UpdatedByUserId = src.UpdatedByUserId,
        LastTestLatencyMs = src.LastTestLatencyMs,
        LastTestMessage = src.LastTestMessage,
        LastTestSuccess = src.LastTestSuccess,
        ConfigSource = src.ConfigSource,
    };

    private static string? ExtractGeminiError(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("error", out var err) &&
                err.TryGetProperty("message", out var msg))
                return msg.GetString();
        }
        catch { /* ignore */ }
        return null;
    }

    private sealed class OrganizationChatbotRuntimeState
    {
        public OrganizationChatbotOptions Options { get; set; } = new();
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedByUserId { get; set; }
        public int? LastTestLatencyMs { get; set; }
        public string? LastTestMessage { get; set; }
        public bool? LastTestSuccess { get; set; }
        public string? ConfigSource { get; set; }
    }
}
