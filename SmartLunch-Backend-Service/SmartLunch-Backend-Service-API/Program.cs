using SmartLunch.Backend.Service.Application.Mappings;
using SmartLunch.Backend.Service.Infrastructure.Data;
using SmartLunch.Shared.MessageQueue.Dotnet.Extensions;
using SmartLunch.Backend.Service.API.Authorization;
using SmartLunch.Backend.Service.API.Authorization.Role;
using SmartLunch.Backend.Service.API.Authorization.Permission;
using SmartLunch.Backend.Service.API.Extensions;
using SmartLunch.Backend.Service.API.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using System.Text;
using SmartLunch.Backend.Service.Infrastructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

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
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
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
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SmartLunch.Backend.Service.Application.Commands.Auth.LoginCommand).Assembly));

// Database Configuration
builder.Services.AddEnhancedDatabase(builder.Configuration);

// Add HttpContextAccessor for DBContext
builder.Services.AddHttpContextAccessor();

// Application Services - Register using Scrutor
SmartLunch.Backend.Service.Application.DependencyInjection.DependencyInjection.ConfigureServices(builder.Services);

// Infrastructure Services - Register using Scrutor
SmartLunch.Backend.Service.Infrastructure.DependencyInjection.DependencyInjection.ConfigureServices(builder.Services);

// Kafka Messaging
var kafkaBootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
    ?? throw new InvalidOperationException("Kafka BootstrapServers not configured");
var kafkaGroupId = builder.Configuration["Kafka:GroupId"]
    ?? "smartlunch-backend-service";

builder.Services.AddKafkaMessaging(kafkaBootstrapServers, kafkaGroupId);

builder.Services.AddLogging();

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

    // Important for SignalR over WebSockets:
    // browsers can't always send Authorization header in the WebSocket handshake,
    // so SignalR sends the token via query string `access_token`.
    options.Events = new JwtBearerEvents
    {
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
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Redirect root to Swagger in Development (before MapControllers)
if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.MapControllers();

// SignalR hubs
app.MapHub<ChatHub>("/hubs/chat");

app.Run();

