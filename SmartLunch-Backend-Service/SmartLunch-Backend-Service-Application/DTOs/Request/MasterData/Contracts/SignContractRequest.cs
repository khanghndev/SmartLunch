namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Contracts;

public class SignContractRequest
{
    /// <summary>Base64/signature blob or external signature string.</summary>
    public string DigitalSignature { get; set; } = string.Empty;

    /// <summary>Optional signature image URL (e.g. from Media upload).</summary>
    public string? SignatureImageUrl { get; set; }
}

