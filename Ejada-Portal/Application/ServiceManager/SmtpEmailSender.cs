using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Application.ServiceManager
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _cfg;
        public SmtpEmailSender(IConfiguration cfg) => _cfg = cfg;

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var host = _cfg["Smtp:Host"];
            var port = int.Parse(_cfg["Smtp:Port"] ?? "2525");
            var enableSsl = bool.TryParse(_cfg["Smtp:EnableSsl"], out var ssl) ? ssl : false;
            var user = _cfg["Smtp:User"];
            var pass = _cfg["Smtp:Pass"];
            var from = _cfg["Smtp:From"] ?? "EjadaPortal_Admin@gmail.com";
            var fromName = _cfg["Smtp:FromDisplayName"] ?? from;

            using var client = new SmtpClient(host!, port)
            {
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
            if (!string.IsNullOrWhiteSpace(user))
                client.Credentials = new NetworkCredential(user, pass);

            using var msg = new MailMessage
            {
                From = new MailAddress(from, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            msg.To.Add(new MailAddress(toEmail));

            await client.SendMailAsync(msg);
        }
    }
}
