using SmartLunch.Backend.Service.Application.Mappings;
using SmartLunch.Backend.Service.Infrastructure.Data;
using SmartLunch.Shared.MessageQueue.Dotnet.Extensions;
using SmartLunch.Backend.Service.API.Authorization;
using SmartLunch.Backend.Service.API.Authorization.Role;
using SmartLunch.Backend.Service.API.Authorization.Permission;
using SmartLunch.Backend.Service.API.Extensions;
using SmartLunch.Backend.Service.API.Hubs;
using SmartLunch.Backend.Service.Application.Interfaces;
using SmartLunch.Backend.Service.API.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using SmartLunch.Backend.Service.Application.Integration.Email;
using SmartLunch.Backend.Service.Application.Integration.PayOS;
using SmartLunch.Backend.Service.Infrastructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog from appsettings (Serilog section)
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container
builder.Services.AddControllers();

var redisConfiguration = builder.Configuration["Redis:Configuration"];
if (string.IsNullOrWhiteSpace(redisConfiguration))
{
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConfiguration;
        options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "smartlunch:";
    });
}

// API Versioning Configuration
builder.Services.AddApiVersioning(options =>
{
    // Specify the default API version
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;

    // Report API versions in response headers
    options.ReportApiVersions = true;

    // Support multiple versioning schemes
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),  // ?api-version=1.0
        new HeaderApiVersionReader("X-API-Version"),      // Header: X-API-Version: 1.0
        new UrlSegmentApiVersionReader()                  // /api/v1/controller
    );
});

// Add API versioning explorer for Swagger
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";  // Format: v1, v2, v3
    setup.SubstituteApiVersionInUrl = true; // Replace {version:apiVersion} in routes
});

builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with API versioning support
builder.Services.AddSwaggerGen(options =>
{
    // Use full type name for schema IDs to avoid conflicts (e.g. MediaFileDto in MasterData.MediaFiles vs Media)
    options.CustomSchemaIds(type => type.FullName);

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add JWT Bearer authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Swagger options for versioning - this will generate documents for each version
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SmartLunch.Backend.Service.Application.DependencyInjection.DependencyInjection).Assembly));

// Database Configuration
builder.Services.AddEnhancedDatabase(builder.Configuration);

// Add HttpContextAccessor for DBContext
builder.Services.AddHttpContextAccessor();

// Application Services - Register using Scrutor
SmartLunch.Backend.Service.Application.DependencyInjection.DependencyInjection.ConfigureServices(builder.Services);

// Infrastructure Services - Register using Scrutor
SmartLunch.Backend.Service.Infrastructure.DependencyInjection.DependencyInjection.ConfigureServices(builder.Services);

// PayOS (thanh toán hợp đồng / payment link) — bật và điền key trong cấu hình hoặc User Secrets
builder.Services.Configure<SmartLunch.Backend.Service.Application.OrganizationChatbot.OrganizationChatbotOptions>(
    builder.Configuration.GetSection(SmartLunch.Backend.Service.Application.OrganizationChatbot.OrganizationChatbotOptions.SectionKey));
builder.Services.AddHttpClient<SmartLunch.Backend.Service.Application.OrganizationChatbot.IOrganizationChatbotLlmClient,
    SmartLunch.Backend.Service.Infrastructure.ExternalServices.GeminiOrganizationChatbotLlmClient>();

builder.Services.Configure<PayOSOptions>(builder.Configuration.GetSection(PayOSOptions.SectionKey));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionKey));
builder.Services.AddHttpClient<IPayOSClient, PayOSClient>();

// RabbitMQ Messaging
var rabbitHost = builder.Configuration["RabbitMQ:HostName"] ?? "localhost";
var rabbitQueue = builder.Configuration["RabbitMQ:QueueName"] ?? "smartlunch.backend";
var rabbitPort = builder.Configuration.GetValue<int>("RabbitMQ:Port", 5672);
var rabbitVHost = builder.Configuration["RabbitMQ:VirtualHost"] ?? "/";
var rabbitUser = builder.Configuration["RabbitMQ:UserName"];
var rabbitPass = builder.Configuration["RabbitMQ:Password"];
var rabbitUri = builder.Configuration["RabbitMQ:Uri"];

if (!string.IsNullOrWhiteSpace(rabbitUri))
{
    builder.Services.AddRabbitMQMessaging(connectionUri: rabbitUri, queueName: rabbitQueue);
}
else
{
    builder.Services.AddRabbitMQMessaging(
        hostName: rabbitHost,
        queueName: rabbitQueue,
        port: rabbitPort,
        virtualHost: rabbitVHost,
        userName: rabbitUser,
        password: rabbitPass);
}

// RabbitMQ consumer background service (consumes events from AI service, etc.)
builder.Services.AddHostedService<SmartLunch.Backend.Service.API.Services.RabbitMQConsumerBackgroundService>();

// Scheduled DB backup service
builder.Services.AddHostedService<SmartLunch.Backend.Service.API.Services.DatabaseBackupHostedService>();
builder.Services.AddHostedService<SmartLunch.Backend.Service.API.Services.OrganizationPaymentReminderHostedService>();
builder.Services.AddHostedService<SmartLunch.Backend.Service.API.Services.OrganizationMealContractWeeklyHostedService>();

// SignalR (Real-time)
builder.Services.AddSignalR();

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var tokenService = context.HttpContext.RequestServices
                .GetRequiredService<IUserTokenRepository>();

            var jti = context.Principal?.FindFirst("jti")?.Value;

            if (!string.IsNullOrEmpty(jti))
            {
                var token = await tokenService.GetByJtiAsync(jti);
                if (token == null || !token.IsActive)
                {
                    context.Fail("Token revoked"); // => 401
                }
            }
        },

        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    // Add default policy
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // Fallback policy allows anonymous access (for Swagger, health checks, etc.)
    options.FallbackPolicy = null; // Allow anonymous by default, controllers can require auth
});

// Register Dynamic Authorization Policy Provider
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();

// Register Authorization Handlers
builder.Services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionRequirementHandler>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Rate limiting (global + named policy for heavy media uploads)
var rlSection = builder.Configuration.GetSection("RateLimiting");
var globalPermitLimit = rlSection.GetValue<int?>("GlobalPermitLimit") ?? 120;
var globalWindowSeconds = rlSection.GetValue<int?>("GlobalWindowSeconds") ?? 60;
var globalQueueLimit = rlSection.GetValue<int?>("GlobalQueueLimit") ?? 0;
var mediaUploadPermitLimit = rlSection.GetValue<int?>("MediaUploadPermitLimit") ?? 20;
var mediaUploadWindowSeconds = rlSection.GetValue<int?>("MediaUploadWindowSeconds") ?? 60;

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = string.IsNullOrWhiteSpace(userId) ? $"ip:{ip}" : $"user:{userId}";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: key,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = globalPermitLimit,
                Window = TimeSpan.FromSeconds(globalWindowSeconds),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = globalQueueLimit,
                AutoReplenishment = true
            });
    });

    options.AddFixedWindowLimiter("media-upload", limiterOptions =>
    {
        limiterOptions.PermitLimit = mediaUploadPermitLimit;
        limiterOptions.Window = TimeSpan.FromSeconds(mediaUploadWindowSeconds);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
        limiterOptions.AutoReplenishment = true;
    });
});

var app = builder.Build();

// Auto-migrate database if enabled
try
{
    await app.MigrateDatabaseAsync();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Failed to migrate database on startup");
    // Don't throw - allow app to start even if migration fails
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    // Configure Swagger UI with API versioning support
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<Microsoft.AspNetCore.Mvc.ApiExplorer.IApiVersionDescriptionProvider>();

    app.UseSwaggerUI(options =>
    {
        // Generate Swagger endpoints for each API version
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"SmartLunch Backend Service API {description.GroupName.ToUpperInvariant()}");
        }

        options.RoutePrefix = "swagger"; // Swagger UI will be available at /swagger
        options.DisplayRequestDuration(); // Show request duration in Swagger UI

        options.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });
}

// Global exception handling - must be early in pipeline to catch all downstream exceptions
app.UseGlobalExceptionHandling();

// Serilog HTTP request logging (method, path, status code, duration)
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
    };
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

// Redirect root to Swagger in Development (before MapControllers)
if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.MapControllers();

// SignalR hubs
app.MapHub<ChatHub>("/hubs/chat");

try
{
    Log.Information("SmartLunch Backend Service starting");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

