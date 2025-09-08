using Application.ServiceManager;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.ServiceManager
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _opt; private readonly ILogger<SmtpEmailSender> _log;
        public SmtpEmailSender(IOptions<SmtpOptions> opt, ILogger<SmtpEmailSender> log) { _opt = opt.Value; _log = log; }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(_opt.Host) || string.IsNullOrWhiteSpace(_opt.User) || string.IsNullOrWhiteSpace(_opt.Pass))
                throw new InvalidOperationException("SMTP settings missing (Host/User/Pass).");

            var from = _opt.From ?? _opt.User;
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_opt.FromDisplayName ?? "Ejada Portal", from));
            msg.To.Add(MailboxAddress.Parse(toEmail));
            msg.Subject = subject;
            msg.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_opt.Host, _opt.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_opt.User, _opt.Pass);
            await client.SendAsync(msg);
            await client.DisconnectAsync(true);
            _log.LogInformation("Email sent to {to}", toEmail);
        }
    }
}
