using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.ServiceManager
{
    public class GmailEmailProvider : IEmailProvider
    {
        private readonly SmtpOptions _opt;
        private readonly ILogger<GmailEmailProvider> _log;

        public GmailEmailProvider(IOptions<SmtpOptions> opt, ILogger<GmailEmailProvider> log)
        {
            _opt = opt.Value;
            _log = log;
        }

        public string Name => "Gmail";

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var msg = new MimeMessage();
            var from = _opt.From ?? _opt.User;
            msg.From.Add(new MailboxAddress(_opt.FromDisplayName ?? "Ejada Portal", from));
            msg.To.Add(MailboxAddress.Parse(toEmail));
            msg.Subject = subject;
            msg.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_opt.Host, _opt.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_opt.User, _opt.Pass);
            await client.SendAsync(msg);
            await client.DisconnectAsync(true);

            _log.LogInformation("Email sent via Gmail to {to}", toEmail);
        }
    }
}
