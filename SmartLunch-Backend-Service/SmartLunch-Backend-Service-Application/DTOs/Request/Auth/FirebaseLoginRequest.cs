using System.Text.Json.Serialization;

namespace SmartLunch.Backend.Service.Application.DTOs.Request.Auth;

public class FirebaseLoginRequest
{
    [JsonPropertyName("idToken")]
    public string IdToken { get; set; } = string.Empty;
}
