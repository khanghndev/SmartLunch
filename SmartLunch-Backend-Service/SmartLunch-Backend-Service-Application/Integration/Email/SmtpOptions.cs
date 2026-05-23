namespace SmartLunch.Backend.Service.Application.Integration.Email;

public sealed class SmtpOptions
{
    public const string SectionKey = "Smtp";

    public bool Enabled { get; set; } = true;
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    /// <summary>Legacy: true = StartTls (port 587). Ưu tiên <see cref="SecureSocketOptions"/> nếu có.</summary>
    public bool UseSsl { get; set; } = true;

    /// <summary>StartTls | SslOnConnect | Auto | None — Gmail: StartTls + port 587.</summary>
    public string SecureSocketOptions { get; set; } = "StartTls";
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromDisplayName { get; set; } = "HuitMeal SmartLunch";
    public int PaymentReminderIntervalHours { get; set; } = 24;
    public int PaymentReminderPollMinutes { get; set; } = 60;
}
