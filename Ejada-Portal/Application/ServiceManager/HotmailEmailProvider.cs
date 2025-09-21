using Application.ServiceManager;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

public class HotmailEmailProvider : IEmailProvider
{
    private readonly HotmailOptions _opt;
    private readonly ILogger<HotmailEmailProvider> _log;

    public HotmailEmailProvider(IOptions<HotmailOptions> opt, ILogger<HotmailEmailProvider> log)
    {
        _opt = opt.Value;
        _log = log;
    }

    public string Name => "Hotmail";

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(_opt.HotmailFromDisplayName ?? "Ejada Portal", _opt.HotmailFrom));
        msg.To.Add(MailboxAddress.Parse(toEmail));
        msg.Subject = subject;
        msg.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new MailKit.Net.Smtp.SmtpClient();
        try
        {
            await client.ConnectAsync(_opt.HotmailHost, _opt.HotmailPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_opt.HotmailUser, _opt.HotmailPass);
            await client.SendAsync(msg);
            await client.DisconnectAsync(true);

            _log.LogInformation("Email sent via Hotmail to {to}", toEmail);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to send email via Hotmail");
            throw;
        }
    }
}
