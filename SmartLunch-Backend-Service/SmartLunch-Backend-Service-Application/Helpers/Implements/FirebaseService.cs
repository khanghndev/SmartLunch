using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using SmartLunch.Backend.Service.Application.Helpers.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SmartLunch.Backend.Service.Application.Helpers.Implements;

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

                if (!string.IsNullOrEmpty(firebaseConfigPath) && File.Exists(firebaseConfigPath))
                {
                    // Initialize with service account JSON file
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(firebaseConfigPath),
                        ProjectId = firebaseProjectId
                    });
                    _logger.LogInformation("Firebase initialized with service account file: {ConfigPath}", firebaseConfigPath);
                }
                else if (!string.IsNullOrEmpty(firebaseProjectId))
                {
                    // Initialize with default credentials (for cloud environments)
                    FirebaseApp.Create(new AppOptions()
                    {
                        ProjectId = firebaseProjectId
                    });
                    _logger.LogInformation("Firebase initialized with project ID: {ProjectId}", firebaseProjectId);
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
                        _logger.LogWarning(defaultCredEx, "Firebase default credentials initialization failed. Firebase login will not work until configured.");
                        // Don't throw - allow the service to be created but Firebase operations will fail gracefully
                    }
                }
            }

            _firebaseAuth = FirebaseAuth.DefaultInstance;
            if (_firebaseAuth != null)
            {
                _logger.LogInformation("Firebase Auth initialized successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Firebase. Firebase login will not work until configured.");
            // Don't throw - allow the service to be created but Firebase operations will fail gracefully
        }
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

            return new FirebaseUserInfo
            {
                Uid = user.Uid,
                Email = user.Email ?? string.Empty,
                DisplayName = user.DisplayName,
                PhotoUrl = user.PhotoUrl,
                EmailVerified = user.EmailVerified,
                PhoneNumber = user.PhoneNumber
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
