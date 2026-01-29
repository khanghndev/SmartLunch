using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Infrastructure.ExternalServices;

public class FirebaseService : IFirebaseService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FirebaseService> _logger;
    private FirebaseAuth? _firebaseAuth;

    public FirebaseService(IConfiguration configuration, ILogger<FirebaseService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        try
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                var firebaseConfigPath = _configuration["Firebase:ConfigPath"];
                var firebaseProjectId = _configuration["Firebase:ProjectId"];

                // Try to find the config file in multiple locations
                string? actualConfigPath = null;
                if (!string.IsNullOrEmpty(firebaseConfigPath))
                {
                    // Try absolute path first
                    if (Path.IsPathRooted(firebaseConfigPath) && File.Exists(firebaseConfigPath))
                    {
                        actualConfigPath = firebaseConfigPath;
                    }
                    else
                    {
                        // Try relative to current working directory
                        var workingDir = Directory.GetCurrentDirectory();
                        var relativePath = Path.Combine(workingDir, firebaseConfigPath);
                        if (File.Exists(relativePath))
                        {
                            actualConfigPath = relativePath;
                        }
                        else
                        {
                            // Try relative to application base directory
                            var baseDir = AppContext.BaseDirectory;
                            var basePath = Path.Combine(baseDir, firebaseConfigPath);
                            if (File.Exists(basePath))
                            {
                                actualConfigPath = basePath;
                            }
                            else
                            {
                                // Try going up from bin folder to find project root (for development)
                                // bin/Debug/net8.0 -> bin/Debug -> bin -> project root
                                var currentDir = baseDir;
                                for (int i = 0; i < 4 && !string.IsNullOrEmpty(currentDir); i++)
                                {
                                    var testPath = Path.Combine(currentDir, firebaseConfigPath);
                                    if (File.Exists(testPath))
                                    {
                                        actualConfigPath = testPath;
                                        break;
                                    }
                                    currentDir = Directory.GetParent(currentDir)?.FullName;
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(actualConfigPath) && File.Exists(actualConfigPath))
                {
                    // Initialize with service account JSON file
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(actualConfigPath),
                        ProjectId = firebaseProjectId ?? GetProjectIdFromConfigFile(actualConfigPath)
                    });
                    _logger.LogInformation("Firebase initialized with service account file: {ConfigPath}", actualConfigPath);
                }
                else if (!string.IsNullOrEmpty(firebaseProjectId))
                {
                    // Try to initialize with default credentials (for cloud environments like GCP)
                    // This only works if running on GCP or with GOOGLE_APPLICATION_CREDENTIALS env var set
                    try
                    {
                        FirebaseApp.Create(new AppOptions()
                        {
                            ProjectId = firebaseProjectId
                        });
                        _logger.LogInformation("Firebase initialized with project ID and default credentials: {ProjectId}", firebaseProjectId);
                    }
                    catch (Exception defaultCredEx)
                    {
                        _logger.LogWarning(defaultCredEx,
                            "Firebase initialization with ProjectId only failed. " +
                            "Please provide Firebase:ConfigPath pointing to service account JSON file. " +
                            "Error: {Error}", defaultCredEx.Message);
                        // Don't throw - allow the service to be created but Firebase operations will fail gracefully
                    }
                }
                else
                {
                    // Try to initialize with default credentials (for cloud environments like GCP)
                    try
                    {
                        FirebaseApp.Create();
                        _logger.LogInformation("Firebase initialized with default credentials");
                    }
                    catch (Exception defaultCredEx)
                    {
                        _logger.LogWarning(defaultCredEx,
                            "Firebase default credentials initialization failed. " +
                            "Please configure Firebase:ConfigPath or Firebase:ProjectId in appsettings.json. " +
                            "Error: {Error}", defaultCredEx.Message);
                    }
                }
            }

            _firebaseAuth = FirebaseAuth.DefaultInstance;
            if (_firebaseAuth != null)
            {
                _logger.LogInformation("Firebase Auth initialized successfully");
            }
            else
            {
                _logger.LogWarning("Firebase Auth is null. Firebase login will not work.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Firebase. Firebase login will not work until configured.");
            // Don't throw - allow the service to be created but Firebase operations will fail gracefully
        }
    }

    private string? GetProjectIdFromConfigFile(string configPath)
    {
        try
        {
            var jsonContent = File.ReadAllText(configPath);
            using var doc = System.Text.Json.JsonDocument.Parse(jsonContent);
            if (doc.RootElement.TryGetProperty("project_id", out var projectIdElement))
            {
                return projectIdElement.GetString();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read project_id from config file: {ConfigPath}", configPath);
        }
        return null;
    }

    public async Task<FirebaseUserInfo> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            if (_firebaseAuth == null)
            {
                _logger.LogError("Firebase Auth is not initialized. Please configure Firebase settings.");
                throw new InvalidOperationException("Firebase Auth is not initialized. Please configure Firebase:ConfigPath or Firebase:ProjectId in appsettings.json");
            }

            var decodedToken = await _firebaseAuth.VerifyIdTokenAsync(idToken);
            var user = await _firebaseAuth.GetUserAsync(decodedToken.Uid);

            // Extract provider from Firebase user's provider data
            // ProviderData contains the authentication providers (e.g., "google.com", "password", "facebook.com")
            var provider = user.ProviderData?.FirstOrDefault()?.ProviderId;

            return new FirebaseUserInfo
            {
                Uid = user.Uid,
                Email = user.Email ?? string.Empty,
                DisplayName = user.DisplayName,
                PhotoUrl = user.PhotoUrl,
                EmailVerified = user.EmailVerified,
                PhoneNumber = user.PhoneNumber,
                Provider = provider
            };
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogWarning(ex, "Firebase token verification failed");
            throw new UnauthorizedAccessException("Invalid Firebase ID token", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying Firebase token");
            throw;
        }
    }
}
