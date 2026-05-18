namespace SmartLunch.Backend.Service.Domain.Entities;

/// <summary>
/// Partner document attachment (business license, certificates, etc.)
/// </summary>
public class PartnerDocument
{
    public int Id { get; set; }
    public string? Code { get; set; }

    public int PartnerId { get; set; }
    public int MediaFileId { get; set; }

    /// <summary>
    /// business_license | tax_certificate | other
    /// </summary>
    public string DocumentType { get; set; } = "other";

    public bool IsVerified { get; set; } = false;

    public DateTime CreatedAt { get; set; } = VietnamTime.Now;
    public DateTime? UpdatedAt { get; set; }

    public virtual Partner Partner { get; set; } = null!;
    public virtual MediaFile MediaFile { get; set; } = null!;
}

