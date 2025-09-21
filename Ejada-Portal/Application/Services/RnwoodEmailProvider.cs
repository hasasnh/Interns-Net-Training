using System.Net;
using System.Net.Mail;
using Application.Services.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class RnwoodEmailProvider : IEmailProvider
    {
        private readonly IConfiguration _cfg;
        private readonly ILogger<RnwoodEmailProvider> _log;

        public RnwoodEmailProvider(IConfiguration cfg, ILogger<RnwoodEmailProvider> log)
        {
            _cfg = cfg;
            _log = log;
        }

        public string Name => "Rnwood";

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var host = _cfg["Smtp:RnwoodHost"] ?? "localhost";
            var port = int.TryParse(_cfg["Smtp:RnwoodPort"], out var p) ? p : 2525;
            var enableSsl = bool.TryParse(_cfg["Smtp:RnwoodEnableSsl"], out var ssl) && ssl;
            var user = _cfg["Smtp:RnwoodUser"];
            var pass = _cfg["Smtp:RnwoodPass"];
            var from = _cfg["Smtp:RnwoodFrom"] ?? "EjadaPortal@gmail.com";
            var fromName = _cfg["Smtp:RnwoodFromDisplayName"] ?? "Ejada Portal (Local)";

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = string.IsNullOrWhiteSpace(user),
                Credentials = string.IsNullOrWhiteSpace(user) ? null : new NetworkCredential(user, pass)
            };

            using var msg = new MailMessage
            {
                From = new MailAddress(from, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            msg.To.Add(toEmail);

            await client.SendMailAsync(msg);
            _log.LogInformation("Email sent via Rnwood to {to}", toEmail);
        }
    }
}
