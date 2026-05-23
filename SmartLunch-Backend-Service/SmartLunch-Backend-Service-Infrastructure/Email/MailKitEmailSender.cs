using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SmartLunch.Backend.Service.Application.Integration.Email;
using SmartLunch.Backend.Service.Application.Interfaces;

namespace SmartLunch.Backend.Service.Infrastructure.Email;

public sealed class MailKitEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;
    private readonly ILogger<MailKitEmailSender> _logger;

    public MailKitEmailSender(IOptions<SmtpOptions> options, ILogger<MailKitEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(
        string toEmail,
        string subject,
        string htmlBody,
        IReadOnlyList<EmailAttachment>? attachments = null,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("SMTP disabled; skip email to {To} subject {Subject}", toEmail, subject);
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.UserName) || string.IsNullOrWhiteSpace(_options.Password))
        {
            _logger.LogWarning("SMTP credentials missing; skip email to {To}", toEmail);
            return;
        }

        var from = string.IsNullOrWhiteSpace(_options.FromEmail) ? _options.UserName : _options.FromEmail;
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromDisplayName, from));
        message.To.Add(MailboxAddress.Parse(toEmail.Trim()));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlBody };
        if (attachments != null)
        {
            foreach (var att in attachments)
            {
                if (att.Content.Length == 0) continue;
                builder.Attachments.Add(att.FileName, att.Content, ContentType.Parse(att.ContentType));
            }
        }

        message.Body = builder.ToMessageBody();

        var socketOptions = ResolveSecureSocketOptions();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(
                _options.Host,
                _options.Port,
                socketOptions,
                cancellationToken);
            await client.AuthenticateAsync(_options.UserName, _options.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation(
                "Email sent to {To} subject {Subject} via {Host}:{Port} ({Socket})",
                toEmail,
                subject,
                _options.Host,
                _options.Port,
                socketOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "SMTP failed {Host}:{Port} ({Socket}). Inner: {Inner}",
                _options.Host,
                _options.Port,
                socketOptions,
                ex.InnerException?.Message ?? ex.Message);
            throw;
        }
    }

    private SecureSocketOptions ResolveSecureSocketOptions()
    {
        var configured = (_options.SecureSocketOptions ?? string.Empty).Trim();
        if (!string.IsNullOrEmpty(configured))
        {
            return configured.ToLowerInvariant() switch
            {
                "starttls" => MailKit.Security.SecureSocketOptions.StartTls,
                "sslonconnect" or "ssl" => MailKit.Security.SecureSocketOptions.SslOnConnect,
                "none" => MailKit.Security.SecureSocketOptions.None,
                "auto" => MailKit.Security.SecureSocketOptions.Auto,
                _ => MailKit.Security.SecureSocketOptions.StartTls,
            };
        }

        if (_options.Port == 465)
            return MailKit.Security.SecureSocketOptions.SslOnConnect;

        return _options.UseSsl
            ? MailKit.Security.SecureSocketOptions.StartTls
            : MailKit.Security.SecureSocketOptions.Auto;
    }
}
